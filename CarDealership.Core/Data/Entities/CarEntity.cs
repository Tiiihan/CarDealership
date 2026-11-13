namespace CarDealership.Core.Data.Entities;

public class CarEntity
{
	public int Id { get; set; }
	public string Make { get; set; } = string.Empty;
	public string Model { get; set; } = string.Empty;
	public int ManufactureYear { get; set; }
	public int DealershipId { get; set; }

	public DealershipEntity Dealership { get; set; } = null!;
}
