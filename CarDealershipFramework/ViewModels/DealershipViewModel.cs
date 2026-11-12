using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace CarDealershipFramework.ViewModels
{
	public class DealershipViewModel
	{
		public int Dealership_id { get; set; }

		[Required]
		[StringLength(50)]
		public string License { get; set; }

		[Required]
		[StringLength(150)]
		public string Name { get; set; }

		[Required]
		[StringLength(100)]
		public string City { get; set; }

		[StringLength(25)]
		public string Phone { get; set; }

		[Display(Name = "Partner companies")]
		public List<int> SelectedCompanyIds { get; set; }

		public IEnumerable<SelectListItem> CompanyOptions { get; set; }
		public IEnumerable<string> CompanyNames { get; set; }
		public IEnumerable<CarSummaryViewModel> Cars { get; set; }
	}

	public class CarSummaryViewModel
	{
		public int Car_id { get; set; }
		public string Car_make { get; set; }
		public string Car_model { get; set; }
		public int Manufacture_year { get; set; }
	}
}
