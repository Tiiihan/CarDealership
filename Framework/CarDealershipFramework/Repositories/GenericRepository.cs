using CarDealershipFramework.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Threading.Tasks;

namespace CarDealershipFramework.Repositories
{
	public class GenericRepository<T> : IGenericRepository<T> where T : class
	{
		private readonly CarDealershipDbEntities _dbContext;
		private readonly DbSet<T> _dbSet;

		public GenericRepository(CarDealershipDbEntities dbContext)
		{
			if (dbContext == null)
			{
				throw new ArgumentNullException(nameof(dbContext));
			}

			_dbContext = dbContext;
			_dbSet = _dbContext.Set<T>();
		}

		public async Task<List<T>> GetAllAsync()
		{
			return await _dbSet.ToListAsync();
		}

		public async Task<T> GetByIdAsync(int id)
		{
			return await _dbSet.FindAsync(id);
		}

		public void Add(T entity)
		{
			_dbSet.Add(entity);
		}

		public void Remove(T entity)
		{
			_dbSet.Remove(entity);
		}

		public async Task SaveChangesAsync()
		{
			await _dbContext.SaveChangesAsync();
		}
	}
}
