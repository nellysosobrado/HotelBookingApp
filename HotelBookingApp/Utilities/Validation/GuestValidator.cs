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
            .NotEmpty().WithMessage("Phone number is required.");
            
    }
}
