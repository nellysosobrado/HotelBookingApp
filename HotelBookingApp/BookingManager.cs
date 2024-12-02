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
                    .Join(_context.Guests,
                          b => b.GuestId,
                          g => g.GuestId,
                          (b, g) => new
                          {
                              Booking = b,
                              Guest = g
                          })
                    .FirstOrDefault(bg => bg.Guest.GuestId == guestId && bg.Booking.IsCheckedIn && !bg.Booking.IsCheckedOut);

                if (booking == null)
                {
                    Console.WriteLine("No active booking found for this guest.");
                    return;
                }

                Console.WriteLine("\nGuest Details:");
                Console.WriteLine($"Name: {booking.Guest.FirstName} {booking.Guest.LastName}");
                Console.WriteLine($"Booking ID: {booking.Booking.BookingId}");
                Console.WriteLine($"Room ID: {booking.Booking.RoomId}");
                Console.WriteLine($"Check-In Status: {(booking.Booking.IsCheckedIn ? "Checked In" : "Not Checked In")}");
                Console.WriteLine($"Check-Out Status: {(booking.Booking.IsCheckedOut ? "Checked Out" : "Not Checked Out")}");

                // Kontrollera om gästen är incheckad
                if (!booking.Booking.IsCheckedIn)
                {
                    Console.WriteLine("\nThis guest has not checked in yet. Cannot check out.");
                    return;
                }

                // Kontrollera om gästen redan är utcheckad
                if (booking.Booking.IsCheckedOut)
                {
                    Console.WriteLine("\nThis guest has already checked out.");
                    return;
                }

                // Utcheckning
                booking.Booking.IsCheckedIn = false;
                booking.Booking.IsCheckedOut = true;
                booking.Booking.CheckOutDate = DateTime.Now; // Lägg till utcheckningsdatum
                _context.SaveChanges();

                Console.WriteLine($"\nGuest '{booking.Guest.FirstName} {booking.Guest.LastName}' with Booking ID {booking.Booking.BookingId} has been successfully checked out.");
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
                        Bookings = bookings.Join(
                            _context.Rooms,
                            booking => booking.RoomId,
                            room => room.RoomId,
                            (booking, room) => new
                            {
                                RoomId = room.RoomId,
                                IsCheckedIn = booking.IsCheckedIn,
                                IsCheckedOut = booking.IsCheckedOut
                            }).ToList()
                    })
                .ToList();

            if (!guests.Any())
            {
                Console.WriteLine("No guests found.");
                return;
            }

            const int pageSize = 5; // Antal gäster per sida
            int currentPage = 0;
            int totalPages = (int)Math.Ceiling((double)guests.Count / pageSize);

            while (true)
            {
                Console.Clear();
                Console.WriteLine($"=== VIEW ALL GUESTS (Page {currentPage + 1}/{totalPages}) ===");
                Console.WriteLine(new string('-', 100));
                Console.WriteLine($"{"Guest ID",-10}{"Name",-25}{"Email",-30}{"Phone",-15}{"Booked Room",-15}{"Checked In",-12}{"Checked Out",-12}");
                Console.WriteLine(new string('-', 100));

                var guestsOnPage = guests
                    .Skip(currentPage * pageSize)
                    .Take(pageSize)
                    .ToList();

                foreach (var entry in guestsOnPage)
                {
                    var guest = entry.Guest;

                    // Endast rums-ID
                    var roomInfo = entry.Bookings.Any()
                        ? string.Join(", ", entry.Bookings.Select(b => $"ID {b.RoomId}"))
                        : "No Room Booked";

                    var isCheckedIn = entry.Bookings.Any()
                        ? (entry.Bookings.All(b => b.IsCheckedIn) ? "Yes" : "No")
                        : "N/A";

                    var isCheckedOut = entry.Bookings.Any()
                        ? (entry.Bookings.All(b => b.IsCheckedOut) ? "Yes" : "No")
                        : "N/A";

                    Console.WriteLine($"{guest.GuestId,-10}{guest.FirstName + " " + guest.LastName,-25}{guest.Email,-30}{guest.PhoneNumber,-15}{roomInfo,-15}{isCheckedIn,-12}{isCheckedOut,-12}");
                }

                Console.WriteLine(new string('-', 100));
                Console.WriteLine("\nOptions: [N] Next Page | [P] Previous Page | [Q] Quit");
                ConsoleKey input = Console.ReadKey(true).Key; // Läs tangenttryckning utan att visa på skärmen

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
                        return; // Gå tillbaka till huvudmenyn
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
