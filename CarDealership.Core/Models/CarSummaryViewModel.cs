namespace CarDealership.Core.Models;

public class CarSummaryViewModel
{
	public int Id { get; set; }
	public string Make { get; set; } = string.Empty;
	public string Model { get; set; } = string.Empty;
	public int ManufactureYear { get; set; }
}
