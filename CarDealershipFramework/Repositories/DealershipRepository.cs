using CarDealership.Business.Repositories;
using CarDealershipFramework.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using DomainDealership = CarDealership.Business.Domain.Dealership;
using EntityDealership = CarDealershipFramework.Models.Dealership;

namespace CarDealershipFramework.Repositories
{
	public class DealershipRepository : IDealershipRepository
	{
		private readonly CarDealershipDbEntities _dbContext;

		public DealershipRepository(CarDealershipDbEntities dbContext)
		{
			_dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
		}

		public async Task<DomainDealership> Create(DomainDealership dealership)
		{
			var entityDealership = new EntityDealership
			{
				License = dealership.License,
				Name = dealership.Name,
				City = dealership.City,
				Phone = dealership.Phone
			};

			_dbContext.Dealerships.Add(entityDealership);
			await _dbContext.SaveChangesAsync();

			return MapToDomain(entityDealership);
		}

		public async Task Delete(DomainDealership dealership)
		{
			var entityDealership = await _dbContext.Dealerships.FindAsync(dealership.Id);

			if (entityDealership is null)
			{
				return;
			}

			_dbContext.Dealerships.Remove(entityDealership);
			await _dbContext.SaveChangesAsync();
		}

		public async Task<IEnumerable<DomainDealership>> GetAll()
		{
			var dealerships = await _dbContext.Dealerships.AsNoTracking().ToListAsync();

			return dealerships.Select(MapToDomain).ToList();
		}

		public async Task<IEnumerable<DomainDealership>> GetByCompanyId(int companyId)
		{
			var dealerships = await _dbContext.Dealerships
				.AsNoTracking()
				.Where(d => d.Companies.Any(c => c.Company_id == companyId))
				.ToListAsync();

			return dealerships.Select(MapToDomain).ToList();
		}

		public async Task<DomainDealership> GetById(int id)
		{
			var dealership = await _dbContext.Dealerships.AsNoTracking().FirstOrDefaultAsync(d => d.Dealership_id == id);

			return MapToDomain(dealership);
		}

		public async Task<DomainDealership> GetByLicense(string license)
		{
			var dealership = await _dbContext.Dealerships.AsNoTracking().FirstOrDefaultAsync(d => d.License == license);

			return MapToDomain(dealership);
		}

		public async Task ReplaceCompanies(int dealershipId, IEnumerable<int> companyIds)
		{
			var dealership = await _dbContext.Dealerships.Include(d => d.Companies)
									.FirstOrDefaultAsync(d => d.Dealership_id == dealershipId);

			if (dealership is null)
			{
				return;
			}

			var ids = (companyIds ?? Enumerable.Empty<int>()).Distinct().ToList();
			var companies = await _dbContext.Companies.Where(c => ids.Contains(c.Company_id)).ToListAsync();

			dealership.Companies.Clear();
			foreach (var company in companies)
			{
				dealership.Companies.Add(company);
			}

			await _dbContext.SaveChangesAsync();
		}

		public async Task Update(DomainDealership dealership)
		{
			var entityDealership = await _dbContext.Dealerships.FindAsync(dealership.Id);
			if (entityDealership is null)
			{
				return;
			}

			entityDealership.License = dealership.License;
			entityDealership.Name = dealership.Name;
			entityDealership.City = dealership.City;
			entityDealership.Phone = dealership.Phone;

			await _dbContext.SaveChangesAsync();
		}

		private static DomainDealership MapToDomain(EntityDealership dealership)
		{
			if (dealership is null)
			{
				return null;
			}

			return new DomainDealership
			{
				Id = dealership.Dealership_id,
				License = dealership.License,
				Name = dealership.Name,
				City = dealership.City,
				Phone = dealership.Phone
			};
		}
	}
}
