using System;

namespace CarDealership.Business.Dtos.Company
{
	public class UpdateCompanyRequest
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public DateTime EstablishDate { get; set; }
		public string Country { get; set; }
	}
}
