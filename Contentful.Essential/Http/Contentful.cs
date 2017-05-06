using Contentful.Core;
using Contentful.Core.Configuration;
using Contentful.Core.Models;
using Contentful.Essential.Configuration;
using Contentful.Essential.Models;
using Contentful.Essential.Models.Configuration;
using Contentful.Essential.Utility;
using log4net.Core;
using Microsoft.Extensions.Options;
using Microsoft.Practices.ServiceLocation;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Contentful.Essential.Http
{
    public sealed class ContentDelivery
    {
        private static readonly ContentDelivery instance = new ContentDelivery();
        private static ContentfulClient client = new ContentfulClient(new HttpClient(), ConfigurationManager.Instance.ContentfulOptions);
        private ContentDelivery() { }

        public static ContentfulClient Instance { get { return client; } }
    }

    public class ContentManagement
    {
        private static readonly ContentManagement instance = new ContentManagement();
        private static readonly ContentManagementClient client = new ContentManagementClient(new HttpClient(), ConfigurationManager.Instance.ContentfulOptions);

        private ContentManagement() { }

        public static ContentManagementClient Instance { get { return client; } }
        public class ContentManagementClient : ContentfulManagementClient
        {
            internal JsonSerializer EntryDynamicSerializer => JsonSerializer.Create(EntryDynamicSerializerSettings);
            public JsonSerializerSettings EntryDynamicSerializerSettings { get; set; } = new JsonSerializerSettings();

            /// <summary>
            /// Initializes a new instance of the <see cref="ContentfulManagementClient"/> class. 
            /// The main class for interaction with the contentful deliver and preview APIs.
            /// </summary>
            /// <param name="httpClient">The HttpClient of your application.</param>
            /// <param name="options">The options object used to retrieve the <see cref="ContentfulOptions"/> for this client.</param>
            /// <exception cref="ArgumentException">The <see name="options">options</see> parameter was null or empty</exception>
            public ContentManagementClient(HttpClient httpClient, IOptions<ContentfulOptions> options) : base(httpClient, options)
            {
                EntryDynamicSerializerSettings.Converters.Add(new EntryDynamicTypeJsonConverter());
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="ContentfulManagementClient"/> class.
            /// </summary>
            /// <param name="httpClient">The HttpClient of your application.</param>
            /// <param name="options">The <see cref="ContentfulOptions"/> used for this client.</param>
            public ContentManagementClient(HttpClient httpClient, ContentfulOptions options) :
            this(httpClient, new OptionsWrapper<ContentfulOptions>(options))
            {
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="ContentfulManagementClient"/> class.
            /// </summary>
            /// <param name="httpClient">The HttpClient of your application.</param>
            /// <param name="managementApiKey">The management API key used when communicating with the Contentful API</param>
            /// <param name="spaceId">The id of the space to fetch content from.</param>
            public ContentManagementClient(HttpClient httpClient, string managementApiKey, string spaceId) :
            this(httpClient, new OptionsWrapper<ContentfulOptions>(new ContentfulOptions()
            {
                ManagementApiKey = managementApiKey,
                SpaceId = spaceId
            }))
            {
            }
            public virtual async Task<Entry<T>> ArchiveEntryAsync<T>(Entry<T> model, string locale = "", string spaceId = null, CancellationToken cancellationToken = default(CancellationToken)) where T : class, IContentType
            {
                return await ArchiveEntryAsync<T>(model.SystemProperties.Id, model.SystemProperties.Version ?? 0, spaceId, cancellationToken);
            }
            public virtual async Task<Entry<T>> ArchiveEntryAsync<T>(string entryId, int version, string spaceId = null, CancellationToken cancellationToken = default(CancellationToken)) where T : class, IContentType
            {
                Entry<dynamic> archived = await base.ArchiveEntryAsync(entryId, version, spaceId, cancellationToken);
                T fields = DeserializeResultFields<T>(archived.Fields);
                if (fields != default(T))
                {
                    return new Entry<T>
                    {
                        SystemProperties = archived.SystemProperties,
                        Fields = fields
                    };
                }
                // fall back behavior. request was successful, but unable to deserialize so let's use the CDN to get it.
                Entry<T> archivedContent = await ServiceLocator.Current.GetInstance<IContentRepository<T>>().Get(archived.SystemProperties.Id);
                return archivedContent;
            }
            public virtual async Task<Entry<T>> CreateEntryAsync<T>(T model, string locale = "", string spaceId = null, CancellationToken cancellationToken = default(CancellationToken)) where T : class, IContentType
            {
                Entry<dynamic> created = await base.CreateEntryAsync(model.GetDynamicEntry<T>(), model.ContentTypeId, spaceId, cancellationToken);
                T fields = DeserializeResultFields<T>(created.Fields);
                if (fields != default(T))
                {
                    return new Entry<T>
                    {
                        SystemProperties = created.SystemProperties,
                        Fields = fields
                    };
                }
                Entry<T> newContent = await ServiceLocator.Current.GetInstance<IContentRepository<T>>().Get(created.SystemProperties.Id);
                return newContent;
            }
            public virtual async Task<Entry<T>> CreateOrUpdateEntryAsync<T>(T model, string locale = "", string spaceId = null, int? version = null, CancellationToken cancellationToken = default(CancellationToken)) where T : class, IContentType
            {
                Entry<dynamic> created = await base.CreateOrUpdateEntryAsync(model.GetDynamicEntry<T>(), spaceId, model.ContentTypeId, version, cancellationToken);
                T fields = DeserializeResultFields<T>(created.Fields);
                if (fields != default(T))
                {
                    return new Entry<T>
                    {
                        SystemProperties = created.SystemProperties,
                        Fields = fields
                    };
                }
                // fall back behavior. request was successful, but unable to deserialize so let's use the CDN to get it. 
                // Note: Version won't be populated
                Entry<T> newContent = await ServiceLocator.Current.GetInstance<IContentRepository<T>>().Get(created.SystemProperties.Id);
                return newContent;
            }
            public virtual async Task<Entry<T>> GetEntryAsync<T>(string entryId, string locale = "", string spaceId = null, CancellationToken cancellationToken = default(CancellationToken)) where T : class, IContentType
            {
                Entry<dynamic> entry = await base.GetEntryAsync(entryId, spaceId, cancellationToken);
                T fields = DeserializeResultFields<T>(entry.Fields);
                if (fields != default(T))
                {
                    return new Entry<T>
                    {
                        SystemProperties = entry.SystemProperties,
                        Fields = fields
                    };
                }

                // fall back behavior. request was successful, but unable to deserialize so let's use the CDN to get it.
                Entry<T> content = await ServiceLocator.Current.GetInstance<IContentRepository<T>>().Get(entry.SystemProperties.Id);
                return content;
            }
            public virtual async Task<Entry<T>> PublishEntryAsync<T>(Entry<T> model, string locale = "", string spaceId = null, CancellationToken cancellationToken = default(CancellationToken)) where T : class, IContentType
            {
                return await PublishEntryAsync<T>(model.SystemProperties.Id, model.SystemProperties.Version ?? 0, spaceId, cancellationToken);
            }
            public virtual async Task<Entry<T>> PublishEntryAsync<T>(string entryId, int version, string spaceId = null, CancellationToken cancellationToken = default(CancellationToken)) where T : class, IContentType
            {
                Entry<dynamic> published = await PublishEntryAsync(entryId, version, spaceId, cancellationToken);
                T fields = DeserializeResultFields<T>(published.Fields);
                if (fields != default(T))
                {
                    return new Entry<T>
                    {
                        SystemProperties = published.SystemProperties,
                        Fields = fields
                    };
                }

                // fall back behavior. request was successful, but unable to deserialize so let's use the CDN to get it.
                Entry<T> publishedContent = await ServiceLocator.Current.GetInstance<IContentRepository<T>>().Get(published.SystemProperties.Id);
                return publishedContent;
            }
            public virtual async Task<Entry<T>> UnarchiveEntryAsync<T>(Entry<T> model, string locale = "", string spaceId = null, CancellationToken cancellationToken = default(CancellationToken)) where T : class, IContentType
            {
                return await UnarchiveEntryAsync<T>(model.SystemProperties.Id, model.SystemProperties.Version ?? 0, spaceId, cancellationToken);
            }
            public virtual async Task<Entry<T>> UnarchiveEntryAsync<T>(string entryId, int version, string spaceId = null, CancellationToken cancellationToken = default(CancellationToken)) where T : class, IContentType
            {
                Entry<dynamic> unarchived = await UnarchiveEntryAsync(entryId, version, spaceId, cancellationToken);
                T fields = DeserializeResultFields<T>(unarchived.Fields);
                if (fields != default(T))
                {
                    return new Entry<T>
                    {
                        SystemProperties = unarchived.SystemProperties,
                        Fields = fields
                    };
                }

                // fall back behavior. request was successful, but unable to deserialize so let's use the CDN to get it.
                Entry<T> unarchivedContent = await ServiceLocator.Current.GetInstance<IContentRepository<T>>().Get(unarchived.SystemProperties.Id);
                return unarchivedContent;
            }
            public virtual async Task<Entry<T>> UnpublishEntryAsync<T>(Entry<T> model, string locale = "", string spaceId = null, CancellationToken cancellationToken = default(CancellationToken)) where T : class, IContentType
            {
                return await UnpublishEntryAsync<T>(model.SystemProperties.Id, model.SystemProperties.Version ?? 0, spaceId, cancellationToken);
            }
            public virtual async Task<Entry<T>> UnpublishEntryAsync<T>(string entryId, int version, string spaceId = null, CancellationToken cancellationToken = default(CancellationToken)) where T : class, IContentType
            {
                Entry<dynamic> unpublished = await UnpublishEntryAsync(entryId, version, spaceId, cancellationToken);
                T fields = DeserializeResultFields<T>(unpublished.Fields);
                if (fields != default(T))
                {
                    return new Entry<T>
                    {
                        SystemProperties = unpublished.SystemProperties,
                        Fields = fields
                    };
                }

                // fall back behavior. request was successful, but unable to deserialize so let's use the CDN to get it.
                Entry<T> unpublishedContent = await ServiceLocator.Current.GetInstance<IContentRepository<T>>().Get(unpublished.SystemProperties.Id);
                return unpublishedContent;
            }
            protected virtual T DeserializeResultFields<T>(dynamic fields, string locale = "") where T : class, IContentType
            {
                JObject returnedFields = fields as JObject;
                if (returnedFields != null)
                {
                    try
                    {
                        Dictionary<string, T> allLocales = returnedFields.ToObject<Dictionary<string, T>>(EntryDynamicSerializer);
                        if (!allLocales.ContainsKey(locale))
                        {
                            SystemLog.Log(this, $"Entry for locale {locale} does not exist. Returning default instead", Level.Warn);
                            return allLocales.First().Value;
                        }
                        return allLocales[locale];
                    }
                    catch (Exception ex)
                    {
                        SystemLog.Log(this, $"Unable to deserialize CMA response to type {typeof(T)}", Level.Error, ex);
                    }
                }
                return default(T);
            }
        }
    }
}