using HotelBookingApp.Interfaces;
using System;
using System.Linq;

namespace HotelBookingApp
{
    public class GuestServices : IMenu, IMenuNavigation
    {
        private readonly AppDbContext _context;

        public GuestServices(AppDbContext context)
        {
            _context = context;
        }
        public void Menu()
        {
            string[] options = { "View All Guests", "Main Menu" };

            while (true)
            {

                int selectedOption = NavigateMenu(options);

                Console.Clear();

                switch (selectedOption)
                {
                    case 0:
                        ViewAllGuests();
                        break;
                    case 3:
                        Console.WriteLine("Exiting menu...");
                        return;
                }

                // Ge användaren tid att se resultatet innan menyn visas igen
                Console.WriteLine("\nPress any key to return to the menu...");
                Console.ReadKey(true);
            }
        }
        public int NavigateMenu(string[] options)
        {
            int selectedOption = 0;

            while (true)
            {
                Console.Clear();
                Console.WriteLine("GuestServices.cs");

                // Visa alternativen och markera det valda alternativet
                for (int i = 0; i < options.Length; i++)
                {
                    if (i == selectedOption)
                    {
                        Console.ForegroundColor = ConsoleColor.Green; // Markera det valda alternativet
                        Console.WriteLine($"> {options[i]}");
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.WriteLine($"  {options[i]}");
                    }
                }

                // Hantera knapptryckningar
                ConsoleKey key = Console.ReadKey(true).Key;
                switch (key)
                {
                    case ConsoleKey.UpArrow:
                        selectedOption = (selectedOption - 1 + options.Length) % options.Length; // Flytta upp
                        break;
                    case ConsoleKey.DownArrow:
                        selectedOption = (selectedOption + 1) % options.Length; // Flytta ner
                        break;
                    case ConsoleKey.Enter:
                        return selectedOption; // Returnera det valda alternativet
                }
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

                    var roomInfo = entry.Bookings.Any()
                        ? string.Join(", ", entry.Bookings.Select(b => $"ID {b.RoomId}"))
                        : "-";

                    var isCheckedIn = entry.Bookings.Any()
                        ? entry.Bookings.All(b => b.IsCheckedIn) ? "Yes" : "-"
                        : "-";

                    var isCheckedOut = entry.Bookings.Any()
                        ? entry.Bookings.All(b => b.IsCheckedOut) ? "Yes" : "-"
                        : "-";

                    var bookingStatus = entry.Bookings.Any()
                        ? entry.Bookings.All(b => b.BookingStatus) ? "Yes" : "-"
                        : "-";

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

    }
}