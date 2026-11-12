using System.Collections.Generic;
using System.Threading.Tasks;

namespace CarDealershipFramework.Repositories
{
	public interface IGenericRepository<T> where T : class
	{
		Task<List<T>> GetAllAsync();
		Task<T> GetByIdAsync(int id);
		void Add(T entity);
		void Remove(T entity);
		Task SaveChangesAsync();
	}
}
