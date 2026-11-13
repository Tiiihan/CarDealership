using CarDealershipFramework.Validations;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace CarDealershipFramework.ViewModels
{
	public class CarViewModel
	{
		public int Id { get; set; }

		[Required]
		[MaxLength(100)]
		[Display(Name = "Make")]
		public string Make { get; set; }

		[Required]
		[MaxLength(100)]
		[Display(Name = "Model")]
		public string Model { get; set; }

		[Required]
		[DateValidationAttribute]
		[Display(Name = "Manufacture year")]
		public int ManufactureYear { get; set; }

		[Range(1, int.MaxValue, ErrorMessage = "Select a dealership.")]
		[Display(Name = "Dealership")]
		public int DealershipId { get; set; }
		public string DealershipName { get; set; }
		public string DealershipCity { get; set; }
		public IEnumerable<SelectListItem> DealershipOptions { get; set; } = Enumerable.Empty<SelectListItem>();
	}
}
