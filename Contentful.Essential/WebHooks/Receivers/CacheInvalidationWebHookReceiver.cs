using Contentful.Core.Models;
using Contentful.Essential.Models;
using Newtonsoft.Json;
using System;

namespace Contentful.Essential.WebHooks.Receivers
{
    public class CacheInvalidationWebHookReceiver : IWebHookHandler
    {
        protected readonly IPurgeCachedContentRepository _purge;
        public CacheInvalidationWebHookReceiver(IPurgeCachedContentRepository purge)
        {
            _purge = purge;
        }

        public string[] ForActions
        {
            get
            {
                // TODO: consts for these values
                return new[] { "publish", "unpublish" };
            }
        }

        public string[] ForTypes
        {
            get
            {
                // TODO: consts for these values
                return new[] { "Entry" };
            }
        }


        public WebHookResponseMessage Process(WebHookRequestMessage request)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings();
            var jsonObject = request.GetJsonObject();

            Entry<dynamic> updatedEntry = jsonObject.ToObject<Entry<dynamic>>(JsonSerializer.Create(settings));
            _purge.PurgeCache(updatedEntry.SystemProperties.Id);

            return new WebHookResponseMessage($"Cache purged for {updatedEntry.SystemProperties.Id}");
        }
    }
}