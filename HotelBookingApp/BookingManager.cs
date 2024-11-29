using System;
using System.Collections.Generic;
using System.Linq;

namespace HotelBookingApp
{
    public class BookingManager
    {
        private readonly AppDbContext _context;
        private readonly RegisterNewBooking _registerNewBooking;
        private readonly Dictionary<string, Action> _menuActions;

        public BookingManager(AppDbContext context, RegisterNewBooking registerNewBooking)
        {
            _context = context;
            _registerNewBooking = registerNewBooking;

            _menuActions = new Dictionary<string, Action>
            {
                { "1", CheckInGuest },
                { "2", CheckOutGuest },
                { "3", ViewBookingDetails },
                { "4", _registerNewBooking.Execute },
                { "5", ViewAllGuests },
                { "6", ReturnToMainMenu }
            };
        }

        public void Run()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("BOOKING MANAGER");
                Console.WriteLine("Choose an option:");
                Console.WriteLine("1. Check In Guest");
                Console.WriteLine("2. Check Out Guest");
                Console.WriteLine("3. View Booking Details");
                Console.WriteLine("4. Register New Booking");
                Console.WriteLine("5. View All Guests");
                Console.WriteLine("6. Back to Main Menu");

                var choice = Console.ReadLine();

                if (_menuActions.ContainsKey(choice))
                {
                    _menuActions[choice]();
                }
                else
                {
                    Console.WriteLine("Invalid choice. Please try again.");
                }

                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
            }
        }

        private void ReturnToMainMenu()
        {
            Console.WriteLine("Returning to main menu...");
        }

        public void CheckInGuest()
        {
            Console.WriteLine("Enter Booking ID to check in:");
            if (int.TryParse(Console.ReadLine(), out int checkInId))
            {
                var booking = _context.Bookings.FirstOrDefault(b => b.BookingId == checkInId);
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
                booking.CheckInDate = DateTime.Now; // 
                _context.SaveChanges();
                Console.WriteLine($"Guest with Booking ID {checkInId} has been successfully checked in.");
            }
            else
            {
                Console.WriteLine("Invalid Booking ID.");
            }
        }


        public void CheckOutGuest()
        {
            Console.WriteLine("Enter Booking ID to check out:");
            if (int.TryParse(Console.ReadLine(), out int checkOutId))
            {
                var booking = _context.Bookings.FirstOrDefault(b => b.BookingId == checkOutId);
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
                Console.WriteLine($"Guest with Booking ID {checkOutId} has been successfully checked out.");
            }
            else
            {
                Console.WriteLine("Invalid Booking ID.");
            }
        }

        public void ViewBookingDetails()
        {
            Console.WriteLine("Enter Booking ID to view details:");
            if (int.TryParse(Console.ReadLine(), out int viewId))
            {
                var booking = _context.Bookings
                    .Join(_context.Guests, b => b.GuestId, g => g.GuestId, (b, g) => new
                    {
                        Booking = b,
                        Guest = g
                    })
                    .FirstOrDefault(bg => bg.Booking.BookingId == viewId);

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
            else
            {
                Console.WriteLine("Invalid Booking ID.");
            }
        }

        public void ViewAllGuests()
        {
            var guests = _context.Guests
                .GroupJoin(
                    _context.Bookings,
                    g => g.GuestId,
                    b => b.GuestId,
                    (guest, bookings) => new
                    {
                        Guest = guest,
                        Bookings = bookings.ToList()
                    })
                .ToList();

            if (!guests.Any())
            {
                Console.WriteLine("No guests found.");
                return;
            }

            Console.WriteLine("---- Guests and Bookings ----");
            foreach (var entry in guests)
            {
                var guest = entry.Guest;
                Console.WriteLine($"Guest: {guest.FirstName} {guest.LastName} (ID: {guest.GuestId})");
                Console.WriteLine($"Email: {guest.Email}");
                Console.WriteLine($"Phone: {guest.PhoneNumber}");

                if (!entry.Bookings.Any())
                {
                    Console.WriteLine("No bookings for this guest.");
                }
                else
                {
                    foreach (var booking in entry.Bookings)
                    {
                        Console.WriteLine($"- Booking ID: {booking.BookingId}");
                        Console.WriteLine($"  Room ID: {booking.RoomId}");
                        Console.WriteLine($"  Check-In Date: {booking.CheckInDate}");
                        Console.WriteLine($"  Check-Out Date: {booking.CheckOutDate}");
                        Console.WriteLine($"  Checked In: {(booking.IsCheckedIn ? "Yes" : "No")}");
                        Console.WriteLine($"  Checked Out: {(booking.IsCheckedOut ? "Yes" : "No")}");
                    }
                }

                Console.WriteLine("----------------------------");
            }
        }
    }
}

