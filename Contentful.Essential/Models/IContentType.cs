using Contentful.Core.Models;

namespace Contentful.Essential.Models
{
    public interface IContentType
    {
        SystemProperties Sys { get; set; }
    }

    public interface IManagementContentType<T> where T : IContentType
    { }
}