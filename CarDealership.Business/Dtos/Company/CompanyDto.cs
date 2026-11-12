using CarDealership.Business.Dtos.Dealership;
using System;
using System.Collections.Generic;

namespace CarDealership.Business.Dtos.Company
{
	public class CompanyDto
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public DateTime EstablishDate { get; set; }
		public string Country { get; set; }
		public IEnumerable<DealershipSummaryDto> Dealerships { get; set; }
	}
}
