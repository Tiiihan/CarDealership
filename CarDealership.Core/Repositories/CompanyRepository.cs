using CarDealership.Business.Repositories;
using CarDealership.Core.Data;
using CarDealership.Core.Data.Entities;
using Microsoft.EntityFrameworkCore;
using DomainCompany = CarDealership.Business.Domain.Company;

namespace CarDealership.Core.Repositories;

public class CompanyRepository(CarDealershipDbContext dbContext) : ICompanyRepository
{
	public async Task<DomainCompany> Create(DomainCompany company)
	{
		var entityCompany = new CompanyEntity
		{
			Name = company.Name,
			EstablishDate = company.EstablishDate,
			Country = company.Country
		};

		dbContext.Companies.Add(entityCompany);
		await dbContext.SaveChangesAsync();

		return MapToDomain(entityCompany);
	}

	public async Task Delete(DomainCompany company)
	{
		var entityCompany = await dbContext.Companies.FindAsync(company.Id);
		if (entityCompany is null)
		{
			return;
		}

		dbContext.Companies.Remove(entityCompany);
		await dbContext.SaveChangesAsync();
	}

	public async Task<IEnumerable<DomainCompany>> GetAll()
	{
		var companies = await dbContext.Companies.AsNoTracking().ToListAsync();

		return [.. companies.Select(MapToDomain)];
	}

	public async Task<IEnumerable<DomainCompany>> GetByDealershipId(int dealershipId)
	{
		var companies = await dbContext.Companies.AsNoTracking()
						.Where(company => company.Dealerships.Any(dealership => dealership.Id == dealershipId))
						.ToListAsync();

		return [.. companies.Select(MapToDomain)];
	}

	public async Task<DomainCompany?> GetById(int id)
	{
		var company = await dbContext.Companies.AsNoTracking().FirstOrDefaultAsync(company => company.Id == id);

		return company is null ? null : MapToDomain(company);
	}

	public async Task<IEnumerable<DomainCompany>> GetByIds(IEnumerable<int> ids)
	{
		var companyIds = (ids ?? []).Distinct().ToList();
		if (companyIds.Count == 0)
		{
			return [];
		}

		var companies = await dbContext.Companies.AsNoTracking()
									.Where(company => companyIds.Contains(company.Id))
									.ToListAsync();

		return [.. companies.Select(MapToDomain)];
	}

	public async Task RemoveFromDealerships(int companyId)
	{
		var company = await dbContext.Companies.Include(company => company.Dealerships).FirstOrDefaultAsync(company => company.Id == companyId);

		if (company is null)
		{
			return;
		}

		company.Dealerships.Clear();
		await dbContext.SaveChangesAsync();
	}

	public async Task Update(DomainCompany company)
	{
		var entityCompany = await dbContext.Companies.FindAsync(company.Id);
		if (entityCompany is null)
		{
			return;
		}

		entityCompany.Name = company.Name;
		entityCompany.EstablishDate = company.EstablishDate;
		entityCompany.Country = company.Country;

		await dbContext.SaveChangesAsync();
	}

	private static DomainCompany MapToDomain(CompanyEntity company)
	{
		return new DomainCompany
		{
			Id = company.Id,
			Name = company.Name,
			EstablishDate = company.EstablishDate,
			Country = company.Country
		};
	}
}
