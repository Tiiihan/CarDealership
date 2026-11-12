using System;
using System.ComponentModel.DataAnnotations;

namespace CarDealershipFramework.Validations
{
	public class DateValidationAttribute : ValidationAttribute
	{
		private const int FIRST_CAR = 1885;

		public override bool IsValid(object value)
		{
			if (!int.TryParse(value.ToString(), out int result))
			{
				return false;
			}

			return result >= FIRST_CAR && result <= DateTime.Now.Year;
		}
	}
}