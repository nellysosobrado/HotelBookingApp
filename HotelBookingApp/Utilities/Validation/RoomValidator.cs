using FluentValidation;
using HotelBookingApp.Entities;

public class RoomValidator : AbstractValidator<Room>
{
    public RoomValidator()
    {
        RuleFor(r => r.Type)
            .NotEmpty().WithMessage("Room type is required.")
            .Must(type => type == "Single" || type == "Double")
            .WithMessage("Room type must be either 'Single' or 'Double'.")
            .MaximumLength(50).WithMessage("Room type cannot exceed 50 characters.");

        RuleFor(r => r.PricePerNight)
            .NotNull().WithMessage("Price per night is required.")
            .GreaterThan(0).WithMessage("Price per night must be greater than 0.");

        RuleFor(r => r.SizeInSquareMeters)
            .NotNull().WithMessage("Size in square meters is required.")
            .GreaterThan(0).WithMessage("Size in square meters must be greater than 0.");

        RuleFor(r => r.ExtraBeds)
            .GreaterThanOrEqualTo(0).WithMessage("Extra beds cannot be negative.")
            .LessThanOrEqualTo(2).WithMessage("Extra beds cannot exceed 2.")
            .When(r => r.Type == "Double", ApplyConditionTo.CurrentValidator)
            .WithMessage("Extra beds are only applicable for 'Double' rooms.");

        //// Validera totala antalet personer
        //RuleFor(r => r.TotalPeople)
        //    .NotNull().WithMessage("Total people is required.")
        //    .GreaterThan(0).WithMessage("Total people must be greater than 0.")
        //    .LessThanOrEqualTo(4).WithMessage("Total people cannot exceed 4.");
    }
}
