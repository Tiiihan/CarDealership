namespace CarDealership.Core.Data.Entities;

public class CompanyEntity
{
	public int Id { get; set; }
	public string Name { get; set; } = string.Empty;
	public DateTime EstablishDate { get; set; }
	public string? Country { get; set; }

	public ICollection<DealershipEntity> Dealerships { get; set; } = [];
}
