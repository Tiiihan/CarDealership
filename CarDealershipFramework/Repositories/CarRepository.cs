using CarDealership.Business.Repositories;
using CarDealershipFramework.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using DomainCar = CarDealership.Business.Domain.Car;
using EntityCar = CarDealershipFramework.Models.Car;

namespace CarDealershipFramework.Repositories
{
	public class CarRepository : ICarRepository
	{
		private readonly CarDealershipDbEntities _dbContext;

		public CarRepository(CarDealershipDbEntities dbContext)
		{
			_dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
		}

		public async Task<DomainCar> Create(DomainCar car)
		{
			var newCar = new EntityCar
			{
				Car_make = car.Make,
				Car_model = car.Model,
				Manufacture_year = car.ManufactureYear,
				Dealership_id = car.DealershipId,
			};

			_dbContext.Cars.Add(newCar);
			await _dbContext.SaveChangesAsync();

			return MapToDomain(newCar);
		}

		public async Task Delete(DomainCar car)
		{
			var deleteCar = await _dbContext.Cars.FindAsync(car.Id);

			if (deleteCar is null)
			{
				return;
			}

			_dbContext.Cars.Remove(deleteCar);
			await _dbContext.SaveChangesAsync();
		}

		public async Task<IEnumerable<DomainCar>> GetAll()
		{
			var cars = await _dbContext.Cars.AsNoTracking().ToListAsync();

			return cars.Select(MapToDomain);
		}

		public async Task<IEnumerable<DomainCar>> GetByDealershipId(int dealershipId)
		{
			var cars = await _dbContext.Cars.Where(c => c.Dealership_id == dealershipId).AsNoTracking().ToListAsync();

			return cars.Select(MapToDomain);
		}

		public async Task<DomainCar> GetById(int id)
		{
			var car = await _dbContext.Cars
				.AsNoTracking()
				.FirstOrDefaultAsync(c => c.Car_id == id);

			if (car is null)
			{
				return null;
			}

			return MapToDomain(car);
		}

		public Task<bool> HasAnyForDealership(int dealershipId) => _dbContext.Cars.AnyAsync(c => c.Dealership_id == dealershipId);

		public async Task Update(DomainCar car)
		{
			var carForUpdate = await _dbContext.Cars.FindAsync(car.Id);

			if (carForUpdate is null)
			{
				return;
			}

			carForUpdate.Car_make = car.Make;
			carForUpdate.Car_model = car.Model;
			carForUpdate.Manufacture_year = car.ManufactureYear;
			carForUpdate.Dealership_id = car.DealershipId;

			await _dbContext.SaveChangesAsync();
		}

		private static DomainCar MapToDomain(EntityCar car)
		{
			if (car is null)
			{
				return null;
			}

			return new DomainCar
			{
				Id = car.Car_id,
				Make = car.Car_make,
				Model = car.Car_model,
				ManufactureYear = car.Manufacture_year,
				DealershipId = car.Dealership_id
			};
		}
	}
}
