using CarDealership.Business.Dtos.Car;
using CarDealership.Business.Dtos.Company;
using System.Collections.Generic;

namespace CarDealership.Business.Dtos.Dealership
{
	public class DealershipDto
	{
		public int Id { get; set; }
		public string License { get; set; }
		public string Name { get; set; }
		public string City { get; set; }
		public string Phone { get; set; }
		public IEnumerable<CarSummaryDto> Cars { get; set; }
		public IEnumerable<CompanySummaryDto> Companies { get; set; }
	}
}
