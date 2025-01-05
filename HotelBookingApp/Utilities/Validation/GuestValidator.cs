using FluentValidation;
using HotelBookingApp;
using HotelBookingApp.Entities;

public class GuestValidator : AbstractValidator<Guest>
{
    public GuestValidator()
    {
        RuleFor(guest => guest.FirstName)
            .NotEmpty().WithMessage("First name is required.")
            .MaximumLength(50).WithMessage("First name must be less than 50 characters.");

        RuleFor(guest => guest.LastName)
            .NotEmpty().WithMessage("Last name is required.")
            .MaximumLength(50).WithMessage("Last name must be less than 50 characters.");

        RuleFor(guest => guest.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email format.");

        RuleFor(guest => guest.PhoneNumber)
    .NotEmpty().WithMessage("Phone number is required.")
    .Matches(@"^(?:\+46|0)\d{9}$").WithMessage("Phone number must be a valid Swedish number with exactly 10 digits, starting with +46 or 0.")
    .Must(BeAValidSwedishNumber).WithMessage("Invalid Swedish phone number.");


    }
    private bool BeAValidSwedishNumber(string phoneNumber)
    {
        phoneNumber = phoneNumber.Replace(" ", "").Replace("-", "");

        if (phoneNumber.StartsWith("0"))
        {
            return phoneNumber.Length == 10;
        }
        else if (phoneNumber.StartsWith("+46"))
        {
            return phoneNumber.Length == 12;
        }

        return false; 
    }

}
