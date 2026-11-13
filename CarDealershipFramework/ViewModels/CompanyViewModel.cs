using CarDealershipFramework.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace CarDealershipFramework.ViewModels
{
	public class CompanyViewModel
	{
		public int Id { get; set; }

		[Required]
		[StringLength(300)]
		[Display(Name = "Company name")]
		public string Name { get; set; }

		[Required]
		[DataType(DataType.Date)]
		[Display(Name = "Established")]
		[NotFutureDate]
		public DateTime EstablishDate { get; set; }

		[StringLength(60)]
		public string Country { get; set; }

		public IEnumerable<string> DealershipNames { get; set; } = Enumerable.Empty<string>();
	}
}
