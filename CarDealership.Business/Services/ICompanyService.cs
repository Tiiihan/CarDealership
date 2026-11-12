using CarDealership.Business.Dtos.Company;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CarDealership.Business.Services
{
	public interface ICompanyService
	{
		Task<CompanyDto> GetCompanyById(int id);
		Task<IEnumerable<CompanyDto>> GetAllCompanies();
		Task<CompanyDto> CreateCompany(CreateCompanyRequest request);
		Task<bool> UpdateCompany(UpdateCompanyRequest request);
		Task<bool> DeleteCompany(int id);
	}
}
