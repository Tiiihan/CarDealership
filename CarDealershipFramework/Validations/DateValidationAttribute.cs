using System;
using System.ComponentModel.DataAnnotations;

namespace CarDealershipFramework.Validations
{
	public class DateValidationAttribute : ValidationAttribute
	{
		private const int FIRST_CAR = 1885;

		public DateValidationAttribute()
			: base("Enter a valid manufacture year.")
		{
		}

		public override bool IsValid(object value)
		{
			return value is int year
				&& year >= FIRST_CAR
				&& year <= DateTime.Today.Year;
		}
	}
}
