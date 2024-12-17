//using HotelBookingApp.Data;
//using HotelBookingApp.Interfaces;
//using System;
//using System.Linq;

//namespace HotelBookingApp
//{
//    public class GuestServices : IMenu, IMenuNavigation
//    {
//        private readonly AppDbContext _context;

//        public GuestServices(AppDbContext context)
//        {
//            _context = context;
//        }
//        public void Menu()
//        {
//            string[] options = { "View All Guests","Update guest information","Main Menu", "Register new guest" };

//            while (true)
//            {

//                int selectedOption = NavigateMenu(options);

//                Console.Clear();

//                switch (selectedOption)
//                {
//                    case 0:
//                        ViewAllGuests();
//                        break;
//                    case 1:
//                        UpdateGuestInformation();
//                        break;
//                    case 2:
//                        Console.WriteLine("Exiting menu...");
//                        return;
//                    case 3:
//                        RegisterNewGuest();
//                        break;
//                }

//                Console.WriteLine("\nPress any key to return to the menu...");
//                Console.ReadKey(true);
//            }
//        }
//        public int NavigateMenu(string[] options)
//        {
//            int selectedOption = 0;

//            while (true)
//            {
//                Console.Clear();
//                Console.WriteLine("GuestServices.cs");

//                for (int i = 0; i < options.Length; i++)
//                {
//                    if (i == selectedOption)
//                    {
//                        Console.ForegroundColor = ConsoleColor.Green; 
//                        Console.WriteLine($"> {options[i]}");
//                        Console.ResetColor();
//                    }
//                    else
//                    {
//                        Console.WriteLine($"  {options[i]}");
//                    }
//                }


//                ConsoleKey key = Console.ReadKey(true).Key;
//                switch (key)
//                {
//                    case ConsoleKey.UpArrow:
//                        selectedOption = (selectedOption - 1 + options.Length) % options.Length; 
//                        break;
//                    case ConsoleKey.DownArrow:
//                        selectedOption = (selectedOption + 1) % options.Length; 
//                        break;
//                    case ConsoleKey.Enter:
//                        return selectedOption; 
//                }
//            }
//        }
//        public void RegisterNewGuest()
//        {
//            Console.Clear();
//            Console.WriteLine("Register new guest");

//            Console.Write("Enter First Name: ");
//            string firstName = Console.ReadLine();

//            Console.Write("Enter Last Name: ");
//            string lastName = Console.ReadLine();

//            Console.Write("Enter Email: ");
//            string email = Console.ReadLine();

//            Console.Write("Enter Phone Number: ");
//            string phone = Console.ReadLine();

//            if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(phone))
//            {
//                Console.WriteLine("All fields are required. Registration failed.");
//                Console.WriteLine("Press any key to return to the menu...");
//                Console.ReadKey(true);
//                return;
//            }

//            var newGuest = new Guest
//            {
//                FirstName = firstName,
//                LastName = lastName,
//                Email = email,
//                PhoneNumber = phone
//            };

//            _context.Guests.Add(newGuest);
//            _context.SaveChanges();

//            Console.WriteLine("Guest registered successfully!");
//            Console.WriteLine("Press any key to return to the menu...");
//            Console.ReadKey(true);
//        }

//        public void UpdateGuestInformation()
//        {
//            Console.Clear();
//            Console.WriteLine("UPDATE GUEST INFO");

//            var guests = _context.Guests.ToList();

//            if (!guests.Any())
//            {
//                Console.WriteLine("No guests found.");
//                return;
//            }

//            Console.WriteLine("Select a guest to update by ID:");
//            foreach (var guest in guests)
//            {
//                Console.WriteLine($"ID: {guest.GuestId} | Name: {guest.FirstName} {guest.LastName}");
//            }

//            Console.Write("Enter Guest ID: ");
//            if (!int.TryParse(Console.ReadLine(), out int guestId))
//            {
//                Console.WriteLine("Invalid ID. Returning to menu...");
//                return;
//            }

//            var selectedGuest = _context.Guests.FirstOrDefault(g => g.GuestId == guestId);

//            if (selectedGuest == null)
//            {
//                Console.WriteLine("Guest not found. Returning to menu...");
//                return;
//            }

//            Console.WriteLine("Leave fields blank to keep current value.");

//            Console.WriteLine($"Current First Name: {selectedGuest.FirstName}");
//            Console.Write("Enter new First Name: ");
//            string firstName = Console.ReadLine();
//            if (!string.IsNullOrEmpty(firstName))
//            {
//                selectedGuest.FirstName = firstName;
//            }

//            Console.WriteLine($"Current Last Name: {selectedGuest.LastName}");
//            Console.Write("Enter new Last Name: ");
//            string lastName = Console.ReadLine();
//            if (!string.IsNullOrEmpty(lastName))
//            {
//                selectedGuest.LastName = lastName;
//            }

//            Console.WriteLine($"Current Email: {selectedGuest.Email}");
//            Console.Write("Enter new Email: ");
//            string email = Console.ReadLine();
//            if (!string.IsNullOrEmpty(email))
//            {
//                selectedGuest.Email = email;
//            }

//            Console.WriteLine($"Current Phone Number: {selectedGuest.PhoneNumber}");
//            Console.Write("Enter new Phone Number: ");
//            string phone = Console.ReadLine();
//            if (!string.IsNullOrEmpty(phone))
//            {
//                selectedGuest.PhoneNumber = phone;
//            }

//            _context.SaveChanges();

//            Console.WriteLine("Guest information updated successfully.");
//            Console.WriteLine("Press any key to return to the menu...");
//            Console.ReadKey(true);
//        }
//        public void ViewAllGuests()
//        {
//            Console.Clear();
//            Console.WriteLine("=== VIEW ALL GUESTS ===");

//            var guests = _context.Guests
//                .GroupJoin(
//                    _context.Bookings,
//                    g => g.GuestId,
//                    b => b.GuestId,
//                    (guest, bookings) => new
//                    {
//                        Guest = guest,
//                        Bookings = bookings.Select(b => new
//                        {
//                            RoomId = b.RoomId,
//                            IsCheckedIn = b.IsCheckedIn,
//                            IsCheckedOut = b.IsCheckedOut,
//                            BookingStatus = b.BookingStatus
//                        }).ToList()
//                    })
//                .ToList();

//            if (!guests.Any())
//            {
//                Console.WriteLine("No guests found.");
//                return;
//            }

//            const int pageSize = 5;
//            int currentPage = 0;
//            int totalPages = (int)Math.Ceiling((double)guests.Count / pageSize);

//            while (true)
//            {
//                Console.Clear();
//                Console.WriteLine($"(Page {currentPage + 1}/{totalPages}) ");
//                Console.WriteLine(new string('=', 90));
//                Console.WriteLine($"{"Guest ID",-10}{"Name",-20}{"Email",-30}{"Phone",-15}{"Rooms",-15}");
//                Console.WriteLine(new string('-', 90));

//                var guestsOnPage = guests
//                    .Skip(currentPage * pageSize)
//                    .Take(pageSize)
//                    .ToList();

//                foreach (var entry in guestsOnPage)
//                {
//                    var guest = entry.Guest;
//                    var roomInfo = entry.Bookings.Any()
//                        ? string.Join(", ", entry.Bookings.Select(b => $"Room {b.RoomId}"))
//                        : "No bookings";

//                    Console.WriteLine($"{guest.GuestId,-10}{guest.FirstName + " " + guest.LastName,-20}{guest.Email,-30}{guest.PhoneNumber,-15}{roomInfo,-15}");
//                }

//                Console.WriteLine(new string('=', 90));
//                Console.WriteLine($"[N] Next Page | [P] Previous Page | [Q] Quit");
//                Console.WriteLine($"You are on page {currentPage + 1} of {totalPages}");

//                var input = Console.ReadKey(true).Key;

//                switch (input)
//                {
//                    case ConsoleKey.N:
//                        if (currentPage < totalPages - 1)
//                        {
//                            currentPage++;
//                        }
//                        else
//                        {
//                            Console.WriteLine("You are already on the last page. Press any key to continue...");
//                            Console.ReadKey(true);
//                        }
//                        break;

//                    case ConsoleKey.P:
//                        if (currentPage > 0)
//                        {
//                            currentPage--;
//                        }
//                        else
//                        {
//                            Console.WriteLine("You are already on the first page. Press any key to continue...");
//                            Console.ReadKey(true);
//                        }
//                        break;

//                    case ConsoleKey.Q:
//                        Console.WriteLine("Exiting guest view...");
//                        return;

//                    default:
//                        Console.WriteLine("Invalid input. Please use [N], [P], or [Q]. Press any key to continue...");
//                        Console.ReadKey(true);
//                        break;
//                }
//            }
//        }






//    }
//}