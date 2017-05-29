using Contentful.Core;
using Contentful.Essential.Models.Configuration;
using System.Net.Http;

namespace Contentful.Essential
{
    public interface IContentDeliveryClient
    {
        ContentfulClient Instance { get; }
    }

    public sealed class ContentDelivery : IContentDeliveryClient
    {
        private static ContentfulClient _instance = null;
        public ContentfulClient Instance
        {
            get
            {
                return _instance;
            }
        }

        public ContentDelivery(IContentfulOptions options)
        {
            if (_instance == null)
            {
                _instance = new ContentfulClient(new HttpClient(), options.GetOptionsObject());
            }
        }
    }
<<<<<<< HEAD
}
=======
}
>>>>>>> master
