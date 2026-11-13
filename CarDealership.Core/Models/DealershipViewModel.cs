using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace CarDealership.Core.Models;

public class DealershipViewModel
{
	public int Id { get; set; }

	[Required]
	[StringLength(50)]
	public string License { get; set; } = string.Empty;

	[Required]
	[StringLength(150)]
	public string Name { get; set; } = string.Empty;

	[Required]
	[StringLength(100)]
	public string City { get; set; } = string.Empty;

	[StringLength(25)]
	public string? Phone { get; set; }

	[Display(Name = "Partner companies")]
	public List<int> SelectedCompanyIds { get; set; } = [];

	public IEnumerable<SelectListItem> CompanyOptions { get; set; } = Enumerable.Empty<SelectListItem>();
	public IEnumerable<string> CompanyNames { get; set; } = Enumerable.Empty<string>();
	public IEnumerable<CarSummaryViewModel> Cars { get; set; } = Enumerable.Empty<CarSummaryViewModel>();
}
