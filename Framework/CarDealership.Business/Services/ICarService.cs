using CarDealership.Business.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CarDealership.Business.Services
{
	public interface ICarService
	{
		Task<CarDto> AddCar(CreateCarRequest request);
		Task UpdateCar(UpdateCarRequest request);
		Task DeleteCar(int carId);
		Task<IEnumerable<CarDto>> GetAllCars();
		Task<CarDto> GetCarById(int carId);
	}
}
