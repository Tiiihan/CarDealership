namespace CarDealership.Business.Dtos
{
	public class CarDto
	{
		public int Id { get; set; }
		public string Make { get; set; }
		public string Model { get; set; }
		public int ManufactureYear { get; set; }
		public DealershipDto Dealership { get; set; }
	}
}
