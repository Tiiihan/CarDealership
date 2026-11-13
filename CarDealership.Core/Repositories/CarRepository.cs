using CarDealership.Business.Repositories;
using CarDealership.Core.Data;
using CarDealership.Core.Data.Entities;
using Microsoft.EntityFrameworkCore;
using DomainCar = CarDealership.Business.Domain.Car;

namespace CarDealership.Core.Repositories;

public class CarRepository(CarDealershipDbContext dbContext) : ICarRepository
{
	public async Task<DomainCar> Create(DomainCar car)
	{
		var entityCar = new CarEntity
		{
			Make = car.Make,
			Model = car.Model,
			ManufactureYear = car.ManufactureYear,
			DealershipId = car.DealershipId
		};

		dbContext.Cars.Add(entityCar);
		await dbContext.SaveChangesAsync();

		return MapToDomain(entityCar);
	}

	public async Task Delete(DomainCar car)
	{
		var entityCar = await dbContext.Cars.FindAsync(car.Id);
		if (entityCar is null)
		{
			return;
		}

		dbContext.Cars.Remove(entityCar);
		await dbContext.SaveChangesAsync();
	}

	public async Task<IEnumerable<DomainCar>> GetAll()
	{
		var cars = await dbContext.Cars.AsNoTracking().ToListAsync();

		return [.. cars.Select(MapToDomain)];
	}

	public async Task<IEnumerable<DomainCar>> GetByDealershipId(int dealershipId)
	{
		var cars = await dbContext.Cars.AsNoTracking().Where(car => car.DealershipId == dealershipId).ToListAsync();

		return [.. cars.Select(MapToDomain)];
	}

	public async Task<DomainCar?> GetById(int id)
	{
		var car = await dbContext.Cars.AsNoTracking().FirstOrDefaultAsync(car => car.Id == id);

		return car is null ? null : MapToDomain(car);
	}

	public Task<bool> HasAnyForDealership(int dealershipId) => dbContext.Cars.AnyAsync(car => car.DealershipId == dealershipId);

	public async Task Update(DomainCar car)
	{
		var entityCar = await dbContext.Cars.FindAsync(car.Id);
		if (entityCar is null)
		{
			return;
		}

		entityCar.Make = car.Make;
		entityCar.Model = car.Model;
		entityCar.ManufactureYear = car.ManufactureYear;
		entityCar.DealershipId = car.DealershipId;

		await dbContext.SaveChangesAsync();
	}

	private static DomainCar MapToDomain(CarEntity car)
	{
		return new DomainCar
		{
			Id = car.Id,
			Make = car.Make,
			Model = car.Model,
			ManufactureYear = car.ManufactureYear,
			DealershipId = car.DealershipId
		};
	}
}
