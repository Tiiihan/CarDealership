using System.Collections.Generic;

namespace CarDealership.Business.Dtos
{
	public class DealershipDto
	{
		public int Dealership_id { get; set; }
		public string License { get; set; }
		public string Name { get; set; }
		public string City { get; set; }
		public string Phone { get; set; }
		public IEnumerable<CarDto> Cars { get; set; }
		public IEnumerable<CompanyDto> Companies { get; set; }
	}
}