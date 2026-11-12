namespace CarDealership.Business.Domain
{
	public class Car
	{
		public int Id { get; set; }
		public string Make { get; set; }
		public string Model { get; set; }
		public int ManufactureYear { get; set; }
		public int DealershipId { get; set; }
	}
}
