using CarDealership.Business.Dtos.Dealership;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CarDealership.Business.Services
{
	public interface IDealershipService
	{
		Task<DealershipDto> GetDealershipById(int id);
		Task<IEnumerable<DealershipDto>> GetAllDealerships();
		Task<DealershipDto> AddDealership(CreateDealershipRequest request);
		Task<bool> UpdateDealership(UpdateDealershipRequest request);
		Task<bool> DeleteDealership(int id);
	}
}
