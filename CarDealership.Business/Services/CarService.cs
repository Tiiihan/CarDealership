using CarDealership.Business.Domain;
using CarDealership.Business.Dtos.Car;
using CarDealership.Business.Dtos.Dealership;
using CarDealership.Business.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CarDealership.Business.Services
{
	public class CarService : ICarService
	{
		private const int MaxMakeLength = 100;
		private const int MaxModelLength = 100;

		private readonly ICarRepository _carRepository;
		private readonly IDealershipRepository _dealershipRepository;

		public CarService(ICarRepository carRepository, IDealershipRepository dealershipRepository)
		{
			_carRepository = carRepository;
			_dealershipRepository = dealershipRepository;
		}

		public async Task<CarDto> AddCar(CreateCarRequest request)
		{
			if (request is null || !IsRequestValid(request.Make, request.Model, request.ManufactureYear))
			{
				return null;
			}

			var dealership = await _dealershipRepository.GetById(request.DealershipId);

			if (dealership is null)
			{
				return null;
			}

			var newCar = new Car
			{
				Make = request.Make,
				Model = request.Model,
				ManufactureYear = request.ManufactureYear,
				DealershipId = request.DealershipId
			};

			var result = await _carRepository.Create(newCar);

			return MapCar(result, dealership);
		}

		public async Task<bool> DeleteCar(int carId)
		{
			var car = await _carRepository.GetById(carId);

			if (car is null)
			{
				return false;
			}

			await _carRepository.Delete(car);
			return true;
		}

		public async Task<IEnumerable<CarDto>> GetAllCars()
		{
			var cars = await _carRepository.GetAll();

			if (cars is null)
			{
				return Enumerable.Empty<CarDto>();
			}

			return cars.Select(car => MapCar(car));
		}

		public async Task<CarDto> GetCarById(int carId)
		{
			var car = await _carRepository.GetById(carId);

			if (car is null)
			{
				return null;
			}

			return MapCar(car);
		}

		public async Task<bool> UpdateCar(UpdateCarRequest request)
		{
			if (request is null || !IsRequestValid(request.Make, request.Model, request.ManufactureYear))
			{
				return false;
			}

			var car = await _carRepository.GetById(request.Id);

			if (car is null)
			{
				return false;
			}

			var dealership = await _dealershipRepository.GetById(request.DealershipId);

			if (dealership is null)
			{
				return false;
			}

			car.Make = request.Make;
			car.Model = request.Model;
			car.ManufactureYear = request.ManufactureYear;
			car.DealershipId = request.DealershipId;

			await _carRepository.Update(car);

			return true;
		}

		public async Task<IEnumerable<CityCarStatisticsDto>> GetCityCarStatistics()
		{
			var cars = await _carRepository.GetAll();
			var dealerships = await _dealershipRepository.GetAll();

			if (cars is null || dealerships is null)
			{
				return Enumerable.Empty<CityCarStatisticsDto>();
			}

			return dealerships.Join(cars, d => d.Id, c => c.DealershipId,
				(d, c) => new 
				{
					d.City,
					c.Make,
					c.ManufactureYear, 
					DealershipId = d.Id 
				})
				.GroupBy(d => d.City)
				.Select(
					stats => new CityCarStatisticsDto
					{
						City = stats.Key,
						DealershipCount = stats.Select(d => d.DealershipId).Distinct().Count(),
						CarCount = stats.Count(),
						AverageManufactureYear = ((int)stats.Select(c => c.ManufactureYear).Average()),
						NewestManufactureYear = stats.Select(c => c.ManufactureYear).Max(),
						OldestManufactureYear = stats.Select(c => c.ManufactureYear).Min(),
						MostPopularMake = stats.GroupBy(d => d.Make).OrderByDescending(g => g.Count()).ThenBy(g => g.Key).First().Key,
					}
				).OrderBy(c => c.City);
		}

		private static bool IsRequestValid(string carMake, string carModel, int manufactureYear)
		{
			return !string.IsNullOrWhiteSpace(carMake)
				&& carMake.Length <= MaxMakeLength
				&& !string.IsNullOrWhiteSpace(carModel)
				&& carModel.Length <= MaxModelLength
				&& manufactureYear >= 1885 && manufactureYear <= DateTime.Now.Year;
		}

		private static CarDto MapCar(Car car, Dealership dealership = null)
		{
			return new CarDto
			{
				Id = car.Id,
				Make = car.Make,
				Model = car.Model,
				ManufactureYear = car.ManufactureYear,
				DealershipId = car.DealershipId,
				Dealership = dealership is null
					? null
					: new DealershipSummaryDto
					{
						Id = dealership.Id,
						Name = dealership.Name,
						License = dealership.License
					}
			};
		}
	}
}
