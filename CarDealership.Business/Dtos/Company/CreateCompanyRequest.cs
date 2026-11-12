using System;

namespace CarDealership.Business.Dtos.Company
{
	public class CreateCompanyRequest
	{
		public string Name { get; set; }
		public DateTime EstablishDate { get; set; }
		public string Country { get; set; }
	}
}
