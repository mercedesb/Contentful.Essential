namespace Contentful.Essential.Models
{
    public interface IPurgeCachedContentRepository
    {
        void PurgeCache(string id);
    }
}
