using System.ComponentModel.DataAnnotations;

namespace CarDealership.Core.Validations;

public sealed class NotFutureDateAttribute : ValidationAttribute
{
	public NotFutureDateAttribute() : base("The date cannot be in the future.")
	{
	}

	public override bool IsValid(object? value)
	{
		return value is DateTime date && date != default && date.Date <= DateTime.Today;
	}
}
