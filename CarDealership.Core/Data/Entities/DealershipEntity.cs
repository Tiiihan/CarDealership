namespace CarDealership.Core.Data.Entities;

public class DealershipEntity
{
	public int Id { get; set; }
	public string License { get; set; } = string.Empty;
	public string Name { get; set; } = string.Empty;
	public string City { get; set; } = string.Empty;
	public string? Phone { get; set; }

	public ICollection<CarEntity> Cars { get; set; } = [];
	public ICollection<CompanyEntity> Companies { get; set; } = [];
}
