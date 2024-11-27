using System;
using System.Linq;

namespace HotelBookingApp
{
    public class BookingManager
    {
        private readonly AppDbContext _context;

        public BookingManager(AppDbContext context)
        {
            _context = context;
        }

        // Metod för att checka in en gäst
        public void CheckInGuest(int bookingId)
        {
            var booking = _context.Bookings.FirstOrDefault(b => b.BookingId == bookingId);
            if (booking == null)
            {
                Console.WriteLine("Booking not found.");
                return;
            }

            if (booking.IsCheckedIn)
            {
                Console.WriteLine("Guest is already checked in.");
                return;
            }

            booking.IsCheckedIn = true;
            _context.SaveChanges();
            Console.WriteLine($"Guest with Booking ID {bookingId} has been successfully checked in.");
        }

        // Metod för att checka ut en gäst
        public void CheckOutGuest(int bookingId)
        {
            var booking = _context.Bookings.FirstOrDefault(b => b.BookingId == bookingId);
            if (booking == null)
            {
                Console.WriteLine("Booking not found.");
                return;
            }

            if (!booking.IsCheckedIn)
            {
                Console.WriteLine("Guest has not checked in yet.");
                return;
            }

            booking.IsCheckedIn = false;
            booking.IsCheckedOut = true;
            _context.SaveChanges();
            Console.WriteLine($"Guest with Booking ID {bookingId} has been successfully checked out.");
        }

        // Metod för att visa detaljer om en bokning
        public void ViewBookingDetails(int bookingId)
        {
            var booking = _context.Bookings
                .Join(_context.Guests, b => b.GuestId, g => g.GuestId, (b, g) => new
                {
                    Booking = b,
                    Guest = g
                })
                .FirstOrDefault(bg => bg.Booking.BookingId == bookingId);

            if (booking == null)
            {
                Console.WriteLine("Booking not found.");
                return;
            }

            Console.WriteLine("---- Booking Details ----");
            Console.WriteLine($"Booking ID: {booking.Booking.BookingId}");
            Console.WriteLine($"Room ID: {booking.Booking.RoomId}");
            Console.WriteLine($"Guest: {booking.Guest.FirstName} {booking.Guest.LastName}");
            Console.WriteLine($"Check-in Date: {booking.Booking.CheckInDate}");
            Console.WriteLine($"Check-out Date: {booking.Booking.CheckOutDate}");
            Console.WriteLine($"Checked In: {(booking.Booking.IsCheckedIn ? "Yes" : "No")}");
            Console.WriteLine($"Checked Out: {(booking.Booking.IsCheckedOut ? "Yes" : "No")}");
            Console.WriteLine("--------------------------");
        }
    }
}
