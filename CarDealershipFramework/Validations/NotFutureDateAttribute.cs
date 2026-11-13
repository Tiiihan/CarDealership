using System;
using System.ComponentModel.DataAnnotations;

namespace CarDealershipFramework.Validations
{
	public class NotFutureDateAttribute : ValidationAttribute
	{
		public NotFutureDateAttribute()
			: base("The date cannot be in the future.")
		{
		}

		public override bool IsValid(object value)
		{
			return value is DateTime date
				&& date != default(DateTime)
				&& date.Date <= DateTime.Today;
		}
	}
}
