using System.Collections.Generic;

namespace CarDealership.Business.Dtos.Dealership
{
	public class CreateDealershipRequest
	{
		public string License { get; set; }
		public string Name { get; set; }
		public string City { get; set; }
		public string Phone { get; set; }
		public IEnumerable<int> CompanyIds { get; set; }
	}
}
