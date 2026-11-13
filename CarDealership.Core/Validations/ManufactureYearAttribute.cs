using System.ComponentModel.DataAnnotations;

namespace CarDealership.Core.Validations;

public sealed class ManufactureYearAttribute : ValidationAttribute
{
	public ManufactureYearAttribute() : base("Enter a valid manufacture year.")
	{
	}

	public override bool IsValid(object? value)
	{
		return value is int year && year >= 1885 && year <= DateTime.Today.Year;
	}
}
