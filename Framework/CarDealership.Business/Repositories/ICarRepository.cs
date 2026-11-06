using CarDealership.Business.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CarDealership.Business.Repositories
{
	public interface ICarRepository
	{
		Task<Car> Create(Car car);
		Task Update(Car car);
		Task<IEnumerable<Car>> GetAll();
		Task<Car> GetById(int id);
		Task Delete(int id);
	}
}
