using Contentful.Core.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contentful.Essential.Models.Configuration
{
    public interface IContentfulOptions
    {
        string DeliveryAPIKey { get; }
        string ManagementAPIKey { get; }
        string SpaceID { get; }
        bool UsePreviewAPI { get; }
        int MaxNumberOfRateLimitRetries { get; }
        ContentfulOptions GetOptionsObject();
    }
}
