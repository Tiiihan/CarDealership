using CarDealershipFramework.Validations;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace CarDealershipFramework.ViewModels
{
	public class CarViewModel
	{
		public int Car_id { get; set; }

		[Required]
		[MaxLength(150)]
		public string Car_make { get; set; }

		[Required]
		[MaxLength(150)]
		public string Car_model { get; set; }

		[Required]
		[DateValidationAttribute]
		public int Manufacture_year { get; set; }
		public int Dealership_id { get; set; }
		public string DealershipName { get; set; }
		public string DealershipCity { get; set; }
		public IEnumerable<SelectListItem> Dealerships { get; set; }
	}
}
