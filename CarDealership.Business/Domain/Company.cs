using System;

namespace CarDealership.Business.Domain
{
	public class Company
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public DateTime EstablishDate { get; set; }
		public string Country { get; set; }
	}
}
