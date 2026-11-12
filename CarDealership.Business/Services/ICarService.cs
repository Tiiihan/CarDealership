using CarDealership.Business.Dtos.Car;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CarDealership.Business.Services
{
	public interface ICarService
	{
		Task<IEnumerable<CarDto>> GetAllCars();
		Task<CarDto> GetCarById(int carId);
		Task<CarDto> AddCar(CreateCarRequest request);
		Task<bool> UpdateCar(UpdateCarRequest request);
		Task<bool> DeleteCar(int carId);
		Task<IEnumerable<CityCarStatisticsDto>> GetCityCarStatistics();
	}
}
