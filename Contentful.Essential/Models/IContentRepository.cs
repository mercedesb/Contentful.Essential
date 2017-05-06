using Contentful.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Contentful.Essential.Models
{
    public interface IContentRepository<T> where T : class
	{
        Task<IEnumerable<Entry<T>>> GetAll();
        Task<Entry<T>> Get(string id);
	}
}