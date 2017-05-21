using Contentful.CodeFirst;
using Contentful.Core.Models;

namespace Contentful.Essential.Models
{
    public abstract class BaseEntry : IContentType
    {
        [IgnoreContentField]
        public SystemProperties Sys { get; set; }
    }
}