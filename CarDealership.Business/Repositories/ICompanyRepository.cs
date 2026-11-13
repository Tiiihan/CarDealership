using CarDealership.Business.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CarDealership.Business.Repositories
{
	public interface ICompanyRepository
	{
		Task<Company> GetById(int id);
		Task<IEnumerable<Company>> GetByIds(IEnumerable<int> ids);
		Task<IEnumerable<Company>> GetByDealershipId(int dealershipId);
		Task<IEnumerable<Company>> GetAll();
		Task<Company> Create(Company company);
		Task Update(Company company);
		Task RemoveFromDealerships(int companyId);
		Task Delete(Company company);
	}
}
