using CarDealership.Business.Domain;
using System.Threading.Tasks;

namespace CarDealership.Business.Repositories
{
	public interface IDealershipRepository
	{
		Task<Dealership> GetById(int id);
	}
}
