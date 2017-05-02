using System.Collections.Generic;
using System.Threading.Tasks;

namespace Contentful.Essential.Models
{
	public interface IRepository<T> where T : class
	{
		Task<IEnumerable<T>> GetAll();
		Task<T> Get(string id);
	}
}