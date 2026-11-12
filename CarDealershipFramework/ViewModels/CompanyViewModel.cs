using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CarDealershipFramework.ViewModels
{
	public class CompanyViewModel
	{
		public int Company_id { get; set; }

		[Required]
		[StringLength(300)]
		[Display(Name = "Company name")]
		public string Company_name { get; set; }

		[Required]
		[DataType(DataType.Date)]
		[Display(Name = "Established")]
		public DateTime Establish_date { get; set; }

		[StringLength(60)]
		public string Country { get; set; }

		public IEnumerable<string> DealershipNames { get; set; }
	}
}
