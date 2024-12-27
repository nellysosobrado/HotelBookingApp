using FluentValidation;
using HotelBookingApp.Entities;

public class RoomValidator : AbstractValidator<Room>
{
    public RoomValidator()
    {

        RuleFor(room => room.Type)
            .NotEmpty().WithMessage("Room type is required.");

        RuleFor(room => room.PricePerNight)
            .GreaterThan(0).WithMessage("Price per night must be greater than 0.");

        RuleFor(room => room.Type)
            .NotEmpty().WithMessage("Room type is required.")
            .Must(type => type.ToLower() == "single" || type.ToLower() == "double")
            .WithMessage("Room type must be 'Single' or 'Double'.");

        RuleFor(room => room.PricePerNight)
            .GreaterThan(0).WithMessage("Price per night must be greater than 0.");

        RuleFor(room => room.SizeInSquareMeters)
            .GreaterThan(0).WithMessage("Size must be greater than 0.");

        RuleFor(room => room.ExtraBeds)
            .GreaterThanOrEqualTo(0).WithMessage("Extra beds cannot be negative.")
            .LessThanOrEqualTo(2).When(room => room.Type.ToLower() == "double")
            .WithMessage("Double rooms can have up to 2 extra beds.");
    }
}
