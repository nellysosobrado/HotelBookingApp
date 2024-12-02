using System;
using System.Linq;

namespace HotelBookingApp
{
    public class BookingManager
    {
        private readonly AppDbContext _context;
        private readonly RegisterNewBooking _registerNewBooking;

        public BookingManager(AppDbContext context, RegisterNewBooking registerNewBooking)
        {
            _context = context;
            _registerNewBooking = registerNewBooking;
        }

        public void Run()
        {
            while (true)
            {
                Console.Clear();
                DisplayMenu();

                var choice = Console.ReadLine()?.Trim();

                switch (choice)
                {
                    case "1":
                        CheckInGuest();
                        break;
                    case "2":
                        CheckOutGuest();
                        break;
                    case "3":
                        ViewBookingDetails();
                        break;
                    case "4":
                        _registerNewBooking.Execute();
                        break;
                    case "5":
                        ViewAllGuests();
                        break;
                    case "6":
                        ReturnToMainMenu();
                        return; // Avsluta och återgå till huvudmenyn
                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }

                PromptToContinue();
            }
        }

        private void DisplayMenu()
        {
            Console.WriteLine("=== BOOKING MANAGER ===");
            Console.WriteLine("Choose an option:");
            Console.WriteLine("1. Check In Guest");
            Console.WriteLine("2. Check Out Guest");
            Console.WriteLine("3. View Booking Details");
            Console.WriteLine("4. Register New Booking");
            Console.WriteLine("5. View All Guests");
            Console.WriteLine("6. Back to Main Menu");
            Console.WriteLine("========================");
        }

        public void CheckInGuest()
        {
            Console.Clear();
            Console.WriteLine("=== CHECK IN GUEST ===");
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
                booking.CheckInDate = DateTime.Now;
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
            Console.Clear();
            Console.WriteLine("=== CHECK OUT GUEST ===");
            Console.WriteLine("Enter Guest ID to check out:");
            if (int.TryParse(Console.ReadLine(), out int guestId))
            {
                // Hämta aktuell bokning för gästen
                var booking = _context.Bookings
                    .FirstOrDefault(b => b.GuestId == guestId && b.IsCheckedIn && !b.IsCheckedOut);

                if (booking == null)
                {
                    Console.WriteLine("No active booking found for this guest.");
                    return;
                }

                Console.WriteLine("\nGuest Details:");
                Console.WriteLine($"Booking ID: {booking.BookingId}");
                Console.WriteLine($"Room ID: {booking.RoomId}");
                Console.WriteLine($"Check-In Status: {(booking.IsCheckedIn ? "Checked In" : "Not Checked In")}");
                Console.WriteLine($"Check-Out Status: {(booking.IsCheckedOut ? "Checked Out" : "Not Checked Out")}");

                // Utcheckning
                booking.IsCheckedIn = false;
                booking.IsCheckedOut = true;
                booking.BookingStatus = true; // Markera bokningen som slutförd
                booking.CheckOutDate = DateTime.Now;
                _context.SaveChanges();

                Console.WriteLine($"\nBooking with Booking ID {booking.BookingId} has been successfully checked out and marked as completed.");
            }
            else
            {
                Console.WriteLine("Invalid Guest ID.");
            }
        }



        public void ViewBookingDetails()
        {
            Console.Clear();
            Console.WriteLine("=== VIEW BOOKING DETAILS ===");
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
            Console.Clear();
            Console.WriteLine("=== VIEW ALL GUESTS ===");

            var guests = _context.Guests
                .GroupJoin(
                    _context.Bookings,
                    g => g.GuestId,
                    b => b.GuestId,
                    (guest, bookings) => new
                    {
                        Guest = guest,
                        Bookings = bookings.Select(b => new
                        {
                            RoomId = b.RoomId,
                            IsCheckedIn = b.IsCheckedIn,
                            IsCheckedOut = b.IsCheckedOut,
                            BookingStatus = b.BookingStatus
                        }).ToList()
                    })
                .ToList();

            if (!guests.Any())
            {
                Console.WriteLine("No guests found.");
                return;
            }

            const int pageSize = 5;
            int currentPage = 0;
            int totalPages = (int)Math.Ceiling((double)guests.Count / pageSize);

            while (true)
            {
                Console.Clear();
                Console.WriteLine($"=== VIEW ALL GUESTS (Page {currentPage + 1}/{totalPages}) ===");
                Console.WriteLine(new string('-', 150));
                Console.WriteLine($"{"Guest ID",-10}{"Name",-25}{"Email",-30}{"Phone",-15}{"Room",-15}{"Checked In",-12}{"Checked Out",-12}{"Status",-10}");
                Console.WriteLine(new string('-', 150));

                var guestsOnPage = guests
                    .Skip(currentPage * pageSize)
                    .Take(pageSize)
                    .ToList();

                foreach (var entry in guestsOnPage)
                {
                    var guest = entry.Guest;

                    // Endast rums-ID eller "-"
                    var roomInfo = entry.Bookings.Any()
                        ? string.Join(", ", entry.Bookings.Select(b => $"ID {b.RoomId}"))
                        : "-";

                    // Markera CheckedIn och CheckedOut som "-" om ingen incheckning eller utcheckning har skett
                    var isCheckedIn = entry.Bookings.Any()
                        ? entry.Bookings.All(b => b.IsCheckedIn) ? "Yes" : "-"
                        : "-";

                    var isCheckedOut = entry.Bookings.Any()
                        ? entry.Bookings.All(b => b.IsCheckedOut) ? "Yes" : "-"
                        : "-";

                    var bookingStatus = entry.Bookings.Any()
                        ? entry.Bookings.All(b => b.BookingStatus) ? "Yes" : "-"
                        : "-";

                    // Justera bredden för att visa dynamiskt innehåll
                    Console.WriteLine($"{guest.GuestId,-10}{guest.FirstName + " " + guest.LastName,-25}{guest.Email,-30}{guest.PhoneNumber,-15}{roomInfo,-15}{isCheckedIn,-12}{isCheckedOut,-12}{bookingStatus,-10}");
                }

                Console.WriteLine(new string('-', 150));
                Console.WriteLine("\nOptions: [N] Next Page | [P] Previous Page | [Q] Quit");
                ConsoleKey input = Console.ReadKey(true).Key;

                switch (input)
                {
                    case ConsoleKey.N:
                        if (currentPage < totalPages - 1)
                        {
                            currentPage++;
                        }
                        else
                        {
                            Console.WriteLine("You are on the last page. Press any key to continue...");
                            Console.ReadKey(true);
                        }
                        break;
                    case ConsoleKey.P:
                        if (currentPage > 0)
                        {
                            currentPage--;
                        }
                        else
                        {
                            Console.WriteLine("You are on the first page. Press any key to continue...");
                            Console.ReadKey(true);
                        }
                        break;
                    case ConsoleKey.Q:
                        Console.WriteLine("Exiting guest view...");
                        return;
                    default:
                        Console.WriteLine("Invalid choice. Please use [N], [P], or [Q]. Press any key to continue...");
                        Console.ReadKey(true);
                        break;
                }
            }
        }







        private void ReturnToMainMenu()
        {
            Console.WriteLine("Returning to main menu...");
        }

        private void PromptToContinue()
        {
            Console.WriteLine("\nPress any key to return to the menu...");
            Console.ReadKey();
        }
    }
}
