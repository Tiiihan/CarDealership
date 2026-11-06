using CarDealership.Business.Domain;
using CarDealership.Business.Dtos;
using CarDealership.Business.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CarDealership.Business.Services
{
	internal class CarService : ICarService
	{
		private ICarRepository _carRepository;
		private IDealershipRepository _dealershipRepository;

		public CarService(ICarRepository carRepository, IDealershipRepository dealershipRepository) 
		{
			_carRepository = carRepository;
			_dealershipRepository = dealershipRepository;
		}

		public async Task<CarDto> AddCar(CreateCarRequest request)
		{
			var dealership = await _dealershipRepository.GetById(request.DealershipId);

			if (dealership is null)
			{
				return null;
			}

			if (request.ManufactureYear <= 1885 || request.ManufactureYear >= DateTime.UtcNow.Year)
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

			return new CarDto
			{
				Id = result.Id,
				Make = result.Make,
				Model = result.Model,
				ManufactureYear = result.ManufactureYear,
				Dealership = new DealershipDto 
				{ 
					Dealership_id = dealership.Id, 
					Name = dealership.Name, 
					License = dealership.License, 
					City = dealership.City,
					Phone = dealership.Phone,
				}
			};
		}

		public Task DeleteCar(int carId)
		{
			throw new System.NotImplementedException();
		}

		public Task<IEnumerable<CarDto>> GetAllCars()
		{
			throw new System.NotImplementedException();
		}

		public Task<CarDto> GetCarById(int carId)
		{
			throw new System.NotImplementedException();
		}

		public Task UpdateCar(UpdateCarRequest request)
		{
			throw new System.NotImplementedException();
		}
	}
}
