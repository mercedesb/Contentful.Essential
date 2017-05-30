using Contentful.Core.Models;
using Contentful.Essential.Models;
using Newtonsoft.Json;
using System;

namespace Contentful.Essential.WebHooks.Receivers
{
    public class CacheInvalidationWebHookReceiver : BaseWebHookReceiver
    {
        protected readonly IPurgeCachedContentRepository _purge;
        public CacheInvalidationWebHookReceiver(IPurgeCachedContentRepository purge)
        {
            _purge = purge;
        }

        public override string[] ForActions
        {
            get
            {
                return new[] { WebHookActions.Publish, WebHookActions.Unpublish };
            }
        }

        public override string[] ForTypes
        {
            get
            {
                return new[] { WebHookTypes.Entry };
            }
        }


        public override WebHookResponseMessage Process(WebHookRequestMessage request)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings();
            var jsonObject = request.GetJsonObject();

            Entry<dynamic> updatedEntry = jsonObject.ToObject<Entry<dynamic>>(JsonSerializer.Create(settings));
            _purge.PurgeCache(updatedEntry.SystemProperties.Id, updatedEntry.SystemProperties.ContentType.SystemProperties.Id);

            return new WebHookResponseMessage($"Cache purged for {updatedEntry.SystemProperties.Id}");
        }
    }
}