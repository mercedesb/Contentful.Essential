using Contentful.Core;
using Contentful.Core.Configuration;
using Contentful.Core.Models;
using Contentful.Essential.Models;
using Contentful.Essential.Models.Configuration;
using log4net.Core;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Contentful.Essential
{
    public interface IContentManagementClient
    {
        ContentfulManagementClientWrapper Instance { get; }
    }

    public sealed class ContentManagement : IContentManagementClient
    {
        private static ContentfulManagementClientWrapper _instance = null;
        public ContentfulManagementClientWrapper Instance
        {
            get
            {
                return _instance;
            }
        }

        public ContentManagement(IContentfulOptions options)
        {
            if (_instance == null)
            {
                _instance = new ContentfulManagementClientWrapper(new HttpClient(), options.GetOptionsObject());
            }
        }
    }

    public class ContentfulManagementClientWrapper : ContentfulManagementClient
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
        public ContentfulManagementClientWrapper(HttpClient httpClient, IOptions<ContentfulOptions> options) : base(httpClient, options)
        {
            EntryDynamicSerializerSettings.Converters.Add(new EntryDynamicTypeJsonConverter());
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ContentfulManagementClient"/> class.
        /// </summary>
        /// <param name="httpClient">The HttpClient of your application.</param>
        /// <param name="options">The <see cref="ContentfulOptions"/> used for this client.</param>
        public ContentfulManagementClientWrapper(HttpClient httpClient, ContentfulOptions options) :
        this(httpClient, new OptionsWrapper<ContentfulOptions>(options))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ContentfulManagementClient"/> class.
        /// </summary>
        /// <param name="httpClient">The HttpClient of your application.</param>
        /// <param name="managementApiKey">The management API key used when communicating with the Contentful API</param>
        /// <param name="spaceId">The id of the space to fetch content from.</param>
        public ContentfulManagementClientWrapper(HttpClient httpClient, string managementApiKey, string spaceId) :
        this(httpClient, new OptionsWrapper<ContentfulOptions>(new ContentfulOptions()
        {
            ManagementApiKey = managementApiKey,
            SpaceId = spaceId
        }))
        {
        }

        public virtual async Task<Entry<T>> ArchiveEntryAsync<T>(string entryId, int version, string spaceId = null, CancellationToken cancellationToken = default(CancellationToken))
            where T : class, IManagementContentType
        {
            Entry<dynamic> archived = await base.ArchiveEntryAsync(entryId, version, spaceId, cancellationToken);

            T fields = archived.Fields.ToObject<T>();
            Entry<T> responseEntry = new Entry<T>();
            responseEntry.SystemProperties = archived.SystemProperties;
            responseEntry.Fields = fields;

            return responseEntry;
        }

        public virtual async Task<Entry<T>> CreateEntryAsync<T>(Entry<T> model, string contentTypeId, string spaceId = null, CancellationToken cancellationToken = default(CancellationToken))
            where T : class, IManagementContentType
        {
            Entry<dynamic> entry = new Entry<dynamic>();
            entry.SystemProperties = model.SystemProperties;
            entry.Fields = model.Fields;

            Entry<dynamic> created = await base.CreateEntryAsync(entry, contentTypeId, spaceId, cancellationToken);

            T fields = created.Fields.ToObject<T>();
            Entry<T> responseEntry = new Entry<T>();
            responseEntry.SystemProperties = created.SystemProperties;
            responseEntry.Fields = fields;

            return responseEntry;
        }

        public virtual async Task<Entry<T>> CreateOrUpdateEntryAsync<T>(Entry<T> model, string spaceId = null, string contentTypeId = null, int? version = null, CancellationToken cancellationToken = default(CancellationToken))
            where T : class, IManagementContentType
        {
            Entry<dynamic> entry = new Entry<dynamic>();
            entry.SystemProperties = model.SystemProperties;
            entry.Fields = model.Fields;

            Entry<dynamic> created = await base.CreateOrUpdateEntryAsync(entry, spaceId, contentTypeId, version, cancellationToken);

            T fields = created.Fields.ToObject<T>();
            Entry<T> responseEntry = new Entry<T>();
            responseEntry.SystemProperties = created.SystemProperties;
            responseEntry.Fields = fields;

            return responseEntry;
        }
        public virtual async Task<Entry<T>> GetEntryAsync<T>(string entryId, string spaceId = null, CancellationToken cancellationToken = default(CancellationToken))
            where T : class, IManagementContentType
        {
            Entry<dynamic> entry = await base.GetEntryAsync(entryId, spaceId, cancellationToken);

            T fields = entry.Fields.ToObject<T>();
            Entry<T> responseEntry = new Entry<T>();
            responseEntry.SystemProperties = entry.SystemProperties;
            responseEntry.Fields = fields;

            return responseEntry;
        }

        public virtual async Task<Entry<T>> PublishEntryAsync<T>(string entryId, int version, string spaceId = null, CancellationToken cancellationToken = default(CancellationToken))
            where T : class, IManagementContentType
        {
            Entry<dynamic> published = await PublishEntryAsync(entryId, version, spaceId, cancellationToken);

            T fields = published.Fields.ToObject<T>();
            Entry<T> responseEntry = new Entry<T>();
            responseEntry.SystemProperties = published.SystemProperties;
            responseEntry.Fields = fields;

            return responseEntry;
        }

        public virtual async Task<Entry<T>> UnarchiveEntryAsync<T>(string entryId, int version, string spaceId = null, CancellationToken cancellationToken = default(CancellationToken))
            where T : class, IManagementContentType
        {
            Entry<dynamic> unarchived = await UnarchiveEntryAsync(entryId, version, spaceId, cancellationToken);

            T fields = unarchived.Fields.ToObject<T>();
            Entry<T> responseEntry = new Entry<T>();
            responseEntry.SystemProperties = unarchived.SystemProperties;
            responseEntry.Fields = fields;

            return responseEntry;
        }

        public virtual async Task<Entry<T>> UnpublishEntryAsync<T>(string entryId, int version, string spaceId = null, CancellationToken cancellationToken = default(CancellationToken))
            where T : class, IManagementContentType
        {
            Entry<dynamic> unpublished = await UnpublishEntryAsync(entryId, version, spaceId, cancellationToken);

            T fields = unpublished.Fields.ToObject<T>();
            Entry<T> responseEntry = new Entry<T>();
            responseEntry.SystemProperties = unpublished.SystemProperties;
            responseEntry.Fields = fields;

            return responseEntry;
        }

        protected virtual Dictionary<string, T> DeserializeResultFields<T>(dynamic fields) where T : class, IContentType
        {
            JObject returnedFields = fields as JObject;
            if (returnedFields != null)
            {
                try
                {
                    Dictionary<string, T> allLocales = returnedFields.ToObject<Dictionary<string, T>>(EntryDynamicSerializer);
                    return allLocales;
                }
                catch (Exception ex)
                {
                    SystemLog.Log(this, $"Unable to deserialize CMA response to type {typeof(Dictionary<string, T>)}", Level.Error, ex);
                }
            }
            return default(Dictionary<string, T>);
        }

    }
}