using FluentValidation;
using HotelBookingApp.Entities;

public class BookingValidator : AbstractValidator<Booking>
{
    public BookingValidator()
    {
        RuleFor(b => b.Guest)
            .NotNull().WithMessage("Guest information is required.");

        RuleFor(b => b.RoomId)
            .GreaterThan(0).WithMessage("Room ID must be greater than 0.");

        RuleFor(b => b.CheckInDate)
            .NotNull().WithMessage("Check-In Date is required.")
            .GreaterThanOrEqualTo(DateTime.Now.Date).WithMessage("Check-In Date cannot be in the past.");

        RuleFor(b => b.CheckOutDate)
            .NotNull().WithMessage("Check-Out Date is required.")
            .GreaterThan(b => b.CheckInDate).WithMessage("Check-Out Date must be after Check-In Date.");

        RuleFor(b => b.BookingId)
            .GreaterThan(0).WithMessage("Booking ID must be valid.");
    }
}
