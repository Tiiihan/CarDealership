using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace CarDealershipFramework.ViewModels
{
	public class DealershipViewModel
	{
		public int Id { get; set; }

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
		public List<int> SelectedCompanyIds { get; set; } = new List<int>();

		public IEnumerable<SelectListItem> CompanyOptions { get; set; } = Enumerable.Empty<SelectListItem>();
		public IEnumerable<string> CompanyNames { get; set; } = Enumerable.Empty<string>();
		public IEnumerable<CarSummaryViewModel> Cars { get; set; } = Enumerable.Empty<CarSummaryViewModel>();
	}

	public class CarSummaryViewModel
	{
		public int Id { get; set; }
		public string Make { get; set; }
		public string Model { get; set; }
		public int ManufactureYear { get; set; }
	}
}
