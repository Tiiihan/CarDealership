namespace CarDealership.Business.Dtos.Car
{
	public class CreateCarRequest
	{
		public string Make { get; set; }
		public string Model { get; set; }
		public int ManufactureYear { get; set; }
		public int DealershipId { get; set; }
	}
}
