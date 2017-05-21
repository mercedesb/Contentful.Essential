using Contentful.Core.Search;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Contentful.Essential.Models
{
    public interface IContentRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAll();
        Task<T> Get(string id);
        Task<IEnumerable<T>> Search(QueryBuilder<T> builder);
    }
}