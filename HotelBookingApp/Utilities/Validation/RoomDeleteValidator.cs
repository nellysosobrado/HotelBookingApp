using FluentValidation;
using HotelBookingApp.Entities;

public class RoomDeleteValidator : AbstractValidator<Room>
{
    public RoomDeleteValidator()
    {
        RuleFor(r => r)
            .NotNull().WithMessage("Room does not exist.");

        RuleFor(r => r.IsDeleted)
            .Equal(false).WithMessage("Room is already marked as deleted.");

        RuleFor(r => r.Bookings)
            .Must(bookings => bookings == null || !bookings.Any(b => !b.IsCheckedOut))
            .WithMessage("Cannot delete a room that has active bookings.");
    }
}
