using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Contentful.Essential.WebHooks
{
    public class WebHookRequestMessage
    {
        public WebHookRequestMessage(HttpRequestMessage request)
        {
            _request = request;
        }

        protected HttpRequestMessage _request;

        protected string ContentfulTopic
        {
            get
            {
                return _request.Headers.GetValues("X-Contentful-Topic")?.First();
            }
        }

        protected string ContentfulWebhookName
        {
            get
            {
                return _request.Headers.GetValues("X-Contentful-Webhook-Name")?.First();
            }
        }

        public string ContentfulType
        {
            get
            {
                //TODO: fix this duh.
                return ContentfulTopic.Split('.')[1];
            }
        }

        public string ContentfulAction
        {
            get
            {
                //TODO: fix this duh.
                return ContentfulTopic.Split('.')[2];
            }
        }

        public virtual JObject GetJsonObject()
        {
            string requestString = _request.Content.ReadAsStringAsync().Result;
            return JsonConvert.DeserializeObject<JObject>(requestString);
        }
    }
}
