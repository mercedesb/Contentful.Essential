namespace Contentful.Essential.Models
{
    public interface ICachedContentRepository
    {
        void PurgeCache();
        void PurgeCache(string id);
    }
}
