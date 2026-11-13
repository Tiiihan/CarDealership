using CarDealership.Core.Validations;
using System.ComponentModel.DataAnnotations;

namespace CarDealership.Core.Models;

public class CompanyViewModel
{
	public int Id { get; set; }

	[Required]
	[StringLength(300)]
	[Display(Name = "Company name")]
	public string Name { get; set; } = string.Empty;

	[NotFutureDate]
	[DataType(DataType.Date)]
	[Display(Name = "Established")]
	public DateTime EstablishDate { get; set; }

	[StringLength(60)]
	public string? Country { get; set; }

	public IEnumerable<string> DealershipNames { get; set; } = Enumerable.Empty<string>();
}
