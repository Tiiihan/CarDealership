using CarDealership.Business.Repositories;
using CarDealershipFramework.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using DomainCompany = CarDealership.Business.Domain.Company;
using EntityCompany = CarDealershipFramework.Models.Company;

namespace CarDealershipFramework.Repositories
{
	public class CompanyRepository : ICompanyRepository
	{
		private readonly CarDealershipDbEntities _dbContext;

		public CompanyRepository(CarDealershipDbEntities dbContext)
		{
			_dbContext = dbContext;
		}

		public async Task<DomainCompany> Create(DomainCompany company)
		{
			var entityCompany = new EntityCompany
			{
				Company_name = company.Name,
				Establish_date = company.EstablishDate,
				Country = company.Country
			};

			_dbContext.Companies.Add(entityCompany);
			await _dbContext.SaveChangesAsync();

			return MapToDomain(entityCompany);
		}

		public async Task Delete(DomainCompany company)
		{
			var entityCompany = await _dbContext.Companies.FindAsync(company.Id);
			if (entityCompany is null)
			{
				return;
			}

			_dbContext.Companies.Remove(entityCompany);
			await _dbContext.SaveChangesAsync();
		}

		public async Task<IEnumerable<DomainCompany>> GetAll()
		{
			var companies = await _dbContext.Companies.AsNoTracking().ToListAsync();

			return companies.Select(MapToDomain).ToList();
		}

		public async Task<IEnumerable<DomainCompany>> GetByDealershipId(int dealershipId)
		{
			var companies = await _dbContext.Companies.AsNoTracking()
								.Where(c => c.Dealerships.Any(d => d.Dealership_id == dealershipId))
								.ToListAsync();

			return companies.Select(MapToDomain).ToList();
		}

		public async Task<DomainCompany> GetById(int id)
		{
			var company = await _dbContext.Companies.AsNoTracking().FirstOrDefaultAsync(c => c.Company_id == id);

			return MapToDomain(company);
		}

		public async Task<IEnumerable<DomainCompany>> GetByIds(IEnumerable<int> ids)
		{
			var companyIds = (ids ?? Enumerable.Empty<int>()).Distinct().ToList();
			if (!companyIds.Any())
			{
				return Enumerable.Empty<DomainCompany>();
			}

			var companies = await _dbContext.Companies.AsNoTracking().Where(c => companyIds.Contains(c.Company_id)).ToListAsync();

			return companies.Select(MapToDomain).ToList();
		}

		public async Task RemoveFromDealerships(int companyId)
		{
			var company = await _dbContext.Companies.Include(c => c.Dealerships).FirstOrDefaultAsync(c => c.Company_id == companyId);

			if (company is null)
			{
				return;
			}

			company.Dealerships.Clear();
			await _dbContext.SaveChangesAsync();
		}

		public async Task Update(DomainCompany company)
		{
			var entityCompany = await _dbContext.Companies.FindAsync(company.Id);
			if (entityCompany is null)
			{
				return;
			}

			entityCompany.Company_name = company.Name;
			entityCompany.Establish_date = company.EstablishDate;
			entityCompany.Country = company.Country;

			await _dbContext.SaveChangesAsync();
		}

		private static DomainCompany MapToDomain(EntityCompany company)
		{
			if (company is null)
			{
				return null;
			}

			return new DomainCompany
			{
				Id = company.Company_id,
				Name = company.Company_name,
				EstablishDate = company.Establish_date,
				Country = company.Country
			};
		}

	}
}
