using CarDealership.Core.Validations;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace CarDealership.Core.Models;

public class CarViewModel
{
	public int Id { get; set; }

	[Required]
	[StringLength(100)]
	public string Make { get; set; } = string.Empty;

	[Required]
	[StringLength(100)]
	public string Model { get; set; } = string.Empty;

	[ManufactureYear]
	[Display(Name = "Manufacture year")]
	public int ManufactureYear { get; set; }

	[Range(1, int.MaxValue, ErrorMessage = "Select a dealership.")]
	[Display(Name = "Dealership")]
	public int DealershipId { get; set; }

	public string? DealershipName { get; set; }
	public string? DealershipCity { get; set; }
	public IEnumerable<SelectListItem> DealershipOptions { get; set; } = Enumerable.Empty<SelectListItem>();
}
