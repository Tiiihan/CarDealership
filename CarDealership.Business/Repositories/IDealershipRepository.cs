using CarDealership.Business.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CarDealership.Business.Repositories
{
	public interface IDealershipRepository
	{
		Task<Dealership> GetById(int id);
		Task<IEnumerable<Dealership>> GetAll();
		Task<Dealership> GetByLicense(string license);
		Task<Dealership> Create(Dealership dealership);
		Task Update(Dealership dealership);
		Task Delete(Dealership dealership);
		Task ReplaceCompanies(int dealershipId, IEnumerable<int> companyIds);
	}
}
