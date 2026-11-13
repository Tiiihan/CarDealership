using CarDealership.Business.Repositories;
using CarDealership.Core.Data;
using CarDealership.Core.Data.Entities;
using Microsoft.EntityFrameworkCore;
using DomainDealership = CarDealership.Business.Domain.Dealership;

namespace CarDealership.Core.Repositories;

public class DealershipRepository(CarDealershipDbContext dbContext) : IDealershipRepository
{
	public async Task<DomainDealership> Create(DomainDealership dealership)
	{
		var entityDealership = new DealershipEntity
		{
			License = dealership.License,
			Name = dealership.Name,
			City = dealership.City,
			Phone = dealership.Phone
		};

		dbContext.Dealerships.Add(entityDealership);
		await dbContext.SaveChangesAsync();

		return MapToDomain(entityDealership);
	}

	public async Task Delete(DomainDealership dealership)
	{
		var entityDealership = await dbContext.Dealerships.FindAsync(dealership.Id);
		if (entityDealership is null)
		{
			return;
		}

		dbContext.Dealerships.Remove(entityDealership);
		await dbContext.SaveChangesAsync();
	}

	public async Task<IEnumerable<DomainDealership>> GetAll()
	{
		var dealerships = await dbContext.Dealerships.AsNoTracking().ToListAsync();

		return [.. dealerships.Select(MapToDomain)];
	}

	public async Task<IEnumerable<DomainDealership>> GetByCompanyId(int companyId)
	{
		var dealerships = await dbContext.Dealerships.AsNoTracking()
							.Where(dealership => dealership.Companies.Any(company => company.Id == companyId))
							.ToListAsync();

		return [.. dealerships.Select(MapToDomain)];
	}

	public async Task<DomainDealership?> GetById(int id)
	{
		var dealership = await dbContext.Dealerships.AsNoTracking().FirstOrDefaultAsync(dealership => dealership.Id == id);

		return dealership is null ? null : MapToDomain(dealership);
	}

	public async Task<DomainDealership?> GetByLicense(string license)
	{
		var dealership = await dbContext.Dealerships.AsNoTracking().FirstOrDefaultAsync(dealership => dealership.License == license);

		return dealership is null ? null : MapToDomain(dealership);
	}

	public async Task ReplaceCompanies(int dealershipId, IEnumerable<int> companyIds)
	{
		var dealership = await dbContext.Dealerships.Include(dealership => dealership.Companies)
							.FirstOrDefaultAsync(dealership => dealership.Id == dealershipId);

		if (dealership is null)
		{
			return;
		}

		var ids = (companyIds ?? []).Distinct().ToList();
		var companies = ids.Count == 0 ? [] : await dbContext.Companies.Where(company => ids.Contains(company.Id)).ToListAsync();

		dealership.Companies.Clear();
		foreach (var company in companies)
		{
			dealership.Companies.Add(company);
		}

		await dbContext.SaveChangesAsync();
	}

	public async Task Update(DomainDealership dealership)
	{
		var entityDealership = await dbContext.Dealerships.FindAsync(dealership.Id);
		if (entityDealership is null)
		{
			return;
		}

		entityDealership.License = dealership.License;
		entityDealership.Name = dealership.Name;
		entityDealership.City = dealership.City;
		entityDealership.Phone = dealership.Phone;

		await dbContext.SaveChangesAsync();
	}

	private static DomainDealership MapToDomain(DealershipEntity dealership)
	{
		return new DomainDealership
		{
			Id = dealership.Id,
			License = dealership.License,
			Name = dealership.Name,
			City = dealership.City,
			Phone = dealership.Phone
		};
	}
}
