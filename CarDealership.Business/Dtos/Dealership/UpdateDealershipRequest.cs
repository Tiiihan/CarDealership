using System.Collections.Generic;

namespace CarDealership.Business.Dtos.Dealership
{
	public class UpdateDealershipRequest
	{
		public int Id { get; set; }
		public string License { get; set; }
		public string Name { get; set; }
		public string City { get; set; }
		public string Phone { get; set; }
		public IEnumerable<int> CompanyIds { get; set; }
	}
}
