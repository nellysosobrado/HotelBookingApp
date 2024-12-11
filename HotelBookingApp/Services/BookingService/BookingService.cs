using HotelBookingApp.Data;
using HotelBookingApp.Entities;
using HotelBookingApp.Interfaces;
using HotelBookingApp.Interfaces.InterfaceBooking;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace HotelBookingApp.Services.BookingService
{
    public class BookingService : IMenu, IMenuNavigation, ICheckIn,ICheckOut, ISearchBookingById, IDisplayAllGuestInfo, IDisplayPaidBooking, IDisplayNonPaidBooking
    {
        private readonly AppDbContext _context;


        public BookingService(AppDbContext context)
        {
            _context = context;
        }

        public void Menu()
        {
            string[] options = { 
                "Check in guest", "Check out guest", 
                "Search booking id", "View all guests", "View paid bookings", 
                "Edit or cancel booking", "Search for available room", 
                "Main menu", "Display non-paid bookings"};

            while (true)
            {
                int selectedOption = NavigateMenu(options);

                Console.Clear();

                switch (selectedOption)
                {
                    case 0:
                        CheckIn();
                        break;
                    case 1:
                        CheckOut();
                        break;
                    case 2:
                        SearchBookingById();
                        break;
                    case 3:
                        DisplayAllGuestInfo();
                        break;
                    case 4:
                        DisplayPaidBooking();
                        break;
                    case 5:
                        EditBooking();
                        break;
                    case 6:
                        SearchAvailableRooms();
                        break;
                    case 7:
                        return;
                    case 8:
                        DisplayNonPaidBooking();
                        break;


                    default:
                        Console.WriteLine("Invalid choice, try again.");
                        break;
                }

                //Console.WriteLine("\nPress any key to return to the menu...");
                //Console.ReadKey(true);
            }
        }

        public int NavigateMenu(string[] options)
        {
            int selectedOption = 0;

            while (true)
            {
                Console.Clear();
                Console.WriteLine("BookingService.cs");

                for (int i = 0; i < options.Length; i++)
                {
                    if (i == selectedOption)
                    {
                        Console.ForegroundColor = ConsoleColor.Green; 
                        Console.WriteLine($"> {options[i]}");
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.WriteLine($"  {options[i]}");
                    }
                }

                ConsoleKey key = Console.ReadKey(true).Key;
                switch (key)
                {
                    case ConsoleKey.UpArrow:
                        selectedOption = (selectedOption - 1 + options.Length) % options.Length; 
                        break;
                    case ConsoleKey.DownArrow:
                        selectedOption = (selectedOption + 1) % options.Length; 
                        break;
                    case ConsoleKey.Enter:
                        return selectedOption; 
                }
            }
        }
        
        public void SearchAvailableRooms()
        {
            Console.Clear();
            Console.WriteLine("Function: Find available room");

            Console.WriteLine("Enter start date (yyyy-MM-dd):");
            if (!DateTime.TryParse(Console.ReadLine(), out DateTime startDate))
            {
                Console.WriteLine("Invalid date format.");
                return;
            }

            if (startDate.Date < DateTime.Now.Date)
            {
                Console.WriteLine("Start date cannot be in the past.");
                return;
            }

            Console.WriteLine("Enter end date (yyyy-MM-dd):");
            if (!DateTime.TryParse(Console.ReadLine(), out DateTime endDate))
            {
                Console.WriteLine("Invalid date format.");
                return;
            }

            if (endDate.Date < startDate.Date)
            {
                Console.WriteLine("End date cannot be earlier than start date.");
                return;
            }

            Console.WriteLine("Enter the number of guests:");
            if (!int.TryParse(Console.ReadLine(), out int guestCount) || guestCount <= 0)
            {
                Console.WriteLine("Invalid number of guests.");
                return;
            }

            var availableRooms = _context.Rooms
                .Where(room => room.TotalPeople >= guestCount &&
                               !_context.Bookings.Any(b => b.RoomId == room.RoomId &&
                                                           b.CheckInDate <= endDate && b.CheckOutDate >= startDate))
                .ToList();

            if (!availableRooms.Any())
            {
                Console.WriteLine("No available rooms found for the given criteria.");
            }
            else
            {
                Console.WriteLine($"\nAvailable rooms from {startDate:yyyy-MM-dd} to {endDate:yyyy-MM-dd} for {guestCount} guest(s):");
                Console.WriteLine(new string('-', 50));
                Console.WriteLine($"{"Room ID",-10}{"Type",-15}{"TotalPeople",-10}{"Price/Night",-10}");
                Console.WriteLine(new string('-', 50));

                foreach (var room in availableRooms)
                {
                    Console.WriteLine($"{room.RoomId,-10}{room.Type,-15}{room.TotalPeople,-10}{room.PricePerNight,-10:C}");
                }
            }
            Console.WriteLine("\nPress any key to return to the menu...");
            Console.ReadKey();
        }



        public void EditBooking()
        {
            Console.Clear();
            Console.WriteLine("=== MODIFY OR CANCEL BOOKING ===");
            Console.WriteLine("Enter Booking ID to modify or cancel:");

            if (int.TryParse(Console.ReadLine(), out int bookingId))
            {
                // Hämta bokningen baserat på ID
                var booking = _context.Bookings.FirstOrDefault(b => b.BookingId == bookingId);
                if (booking == null)
                {
                    Console.WriteLine("Booking not found.");
                    return;
                }

                // Visa bokningsinformation
                Console.WriteLine("\n--- Current Booking Details ---");
                Console.WriteLine($"Booking ID: {booking.BookingId}");
                Console.WriteLine($"Room ID: {booking.RoomId}");
                Console.WriteLine($"Guest ID: {booking.GuestId}");
                Console.WriteLine($"Check-in Date: {booking.CheckInDate}");
                Console.WriteLine($"Check-out Date: {booking.CheckOutDate}");
                Console.WriteLine($"Checked In: {(booking.IsCheckedIn ? "Yes" : "No")}");
                Console.WriteLine($"Checked Out: {(booking.IsCheckedOut ? "Yes" : "No")}");
                Console.WriteLine("--------------------------------");

                Console.WriteLine("\nOptions:");
                Console.WriteLine("1. Cancel booking");
                Console.WriteLine("2. Modify booking details");
                Console.Write("Choose an option: ");

                var choice = Console.ReadLine()?.Trim();
                switch (choice)
                {
                    case "1":
                        CancelBooking(booking);
                        break;
                    case "2":
                        ModifyBookingDetails(booking);
                        break;
                    default:
                        Console.WriteLine("Invalid choice. Returning to menu...");
                        break;
                }
            }
            else
            {
                Console.WriteLine("Invalid Booking ID.");
            }
        }

        public void CancelBooking(Booking booking)
        {
            if (booking.IsCheckedIn || booking.IsCheckedOut)
            {
                Console.WriteLine("Cannot cancel the booking.");

                if (booking.IsCheckedIn && !booking.IsCheckedOut)
                {
                    Console.WriteLine("Reason: The guest has already checked in.");
                }
                else if (booking.IsCheckedOut)
                {
                    Console.WriteLine("Reason: The booking has already been completed with a check-out.");
                }

                Console.WriteLine("Cancellation is only allowed for bookings that have not started.");
                return;
            }

            var invoice = _context.Invoices.FirstOrDefault(i => i.BookingId == booking.BookingId);
            if (invoice != null)
            {
                var payments = _context.Payments.Where(p => p.InvoiceId == invoice.InvoiceId).ToList();
                _context.Payments.RemoveRange(payments); 
                _context.Invoices.Remove(invoice);       
            }

            _context.Bookings.Remove(booking);
            _context.SaveChanges();

            Console.WriteLine($"Booking with ID {booking.BookingId} has been successfully canceled.");
        }

        private void ModifyBookingDetails(Booking booking)
        {
            Console.Clear();
            Console.WriteLine("=== MODIFY BOOKING DETAILS ===");

            Console.WriteLine("Enter new Room ID (leave blank to keep current):");
            var newRoomIdInput = Console.ReadLine()?.Trim();
            if (!string.IsNullOrWhiteSpace(newRoomIdInput) && int.TryParse(newRoomIdInput, out int newRoomId))
            {
                var room = _context.Rooms.FirstOrDefault(r => r.RoomId == newRoomId);
                if (room == null)
                {
                    Console.WriteLine("Room not found. Keeping current room.");
                }
                else
                {
                    booking.RoomId = newRoomId;
                }
            }

            Console.WriteLine("Enter new Check-in Date (yyyy-MM-dd, leave blank to keep current):");
            var newCheckInDateInput = Console.ReadLine()?.Trim();
            if (!string.IsNullOrWhiteSpace(newCheckInDateInput) && DateTime.TryParse(newCheckInDateInput, out DateTime newCheckInDate))
            {
                booking.CheckInDate = newCheckInDate;
            }

            Console.WriteLine("Enter new Check-out Date (yyyy-MM-dd, leave blank to keep current):");
            var newCheckOutDateInput = Console.ReadLine()?.Trim();
            if (!string.IsNullOrWhiteSpace(newCheckOutDateInput) && DateTime.TryParse(newCheckOutDateInput, out DateTime newCheckOutDate))
            {
                booking.CheckOutDate = newCheckOutDate;
            }

            _context.SaveChanges();
            Console.WriteLine($"Booking with ID {booking.BookingId} has been successfully updated.");
        }






        public void CheckIn()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("CHECK IN GUEST");

                DisplayAllBookings();

                Console.WriteLine("Enter 'Exit' to go back to main menu");
                Console.Write("Enter Booking ID to check in: ");

                string input = Console.ReadLine()?.Trim();

                if (input?.ToLower() == "exit")
                {
                    break;
                }

                if (int.TryParse(input, out int checkInId))
                {
                    var booking = _context.Bookings.FirstOrDefault(b => b.BookingId == checkInId);
                    if (booking == null)
                    {
                        Console.WriteLine("Booking not found. Press any key to try again...");
                        Console.ReadKey();
                        continue;
                    }

                    if (booking.IsCheckedIn)
                    {
                        Console.WriteLine("Guest is already checked in. Press any key to try again...");
                        Console.ReadKey();
                        continue;
                    }

                    booking.IsCheckedIn = true;
                    booking.CheckInDate = DateTime.Now;
                    _context.SaveChanges();

                    var guest = _context.Guests.FirstOrDefault(g => g.GuestId == booking.GuestId);
                    var room = _context.Rooms.FirstOrDefault(r => r.RoomId == booking.RoomId);

                    Console.Clear();
                    Console.WriteLine($"\nGuest {guest?.FirstName + " " + guest?.LastName} has been successfully checked in.");
                    Console.WriteLine(new string('-', 60));
                    Console.WriteLine($"{"Booking ID",-15}{"Guest",-20}{"Room ID",-10}{"Check-In Date",-15}");
                    Console.WriteLine(new string('-', 60));
                    Console.WriteLine($"{booking.BookingId,-15}{guest?.FirstName + " " + guest?.LastName,-20}{room?.RoomId,-10}{booking.CheckInDate,-15:yyyy-MM-dd HH:mm}");
                    Console.WriteLine(new string('-', 60));

                    Console.WriteLine("\nPress any key to continue...");
                    Console.ReadKey();
                }
                else if (input == string.Empty)
                {
                    Console.WriteLine("Input cannot be empty. Press any key to try again..");
                    Console.ReadKey();
                }
                else
                {
                    Console.WriteLine("Invalid Booking ID. Press any key to try again...");
                    Console.ReadKey();
                }
            }
        }

        private void DisplayAllBookings()
        {
            var bookings = _context.Bookings
                .Join(_context.Guests, b => b.GuestId, g => g.GuestId, (b, g) => new
                {
                    BookingId = b.BookingId,
                    GuestName = g.FirstName + " " + g.LastName,
                    RoomId = b.RoomId,
                    IsCheckedIn = b.IsCheckedIn
                })
                .ToList();

            if (!bookings.Any())
            {
                Console.WriteLine("No bookings found.");
                return;
            }

            Console.WriteLine(new string('-', 60));
            Console.WriteLine($"{"Booking ID",-15}{"Guest",-20}{"Room ID",-10}{"Checked In",-10}");
            Console.WriteLine(new string('-', 60));

            foreach (var booking in bookings)
            {
                Console.WriteLine($"{booking.BookingId,-15}{booking.GuestName,-20}{booking.RoomId,-10}{(booking.IsCheckedIn ? "Yes" : "No"),-10}");
            }

            Console.WriteLine(new string('-', 60));
        }




        public void SearchBookingById()
        {
            Console.Clear();
            Console.WriteLine("Page: SearchBookingID()");
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

        public void DisplayAllGuestInfo()
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
                            b.RoomId,
                            b.IsCheckedIn,
                            b.IsCheckedOut,
                            b.BookingStatus
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
                Console.WriteLine(new string('-', 100));
                Console.WriteLine($"{"GuestID",-10}{"Name",-20}{"Email",-25}{"Phone",-15}{"RoomID",-8}{"In",-5}{"Out",-5}{"Status",-7}");
                Console.WriteLine(new string('-', 100));

                var guestsOnPage = guests
                    .Skip(currentPage * pageSize)
                    .Take(pageSize)
                    .ToList();

                foreach (var entry in guestsOnPage)
                {
                    var guest = entry.Guest;

                    var roomInfo = entry.Bookings.Any()
                        ? string.Join(", ", entry.Bookings.Select(b => b.RoomId.ToString()))
                        : "-";

                    var isCheckedIn = entry.Bookings.Any() && entry.Bookings.All(b => b.IsCheckedIn) ? "Yes" : "-";
                    var isCheckedOut = entry.Bookings.Any() && entry.Bookings.All(b => b.IsCheckedOut) ? "Yes" : "-";
                    var bookingStatus = entry.Bookings.Any() && entry.Bookings.All(b => b.BookingStatus) ? "Yes" : "-";

                    Console.WriteLine($"{guest.GuestId,-10}{guest.FirstName + " " + guest.LastName,-20}{guest.Email,-25}{guest.PhoneNumber,-15}{roomInfo,-8}{isCheckedIn,-5}{isCheckedOut,-5}{bookingStatus,-7}");
                }

                Console.WriteLine(new string('-', 100));
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
        public List<Booking> GetGuestsWithCheckInStatus()
        {
            return _context.Bookings
                .Include(b => b.Guest) 
                .Where(b => b.Guest != null) 
                .ToList();
        }



        public void DisplayNonPaidBooking()
        {
            Console.Clear();
            Console.WriteLine("page: DisplayNonPaidBooking");

            var nonPaidBookings = _context.Invoices
                .Where(i => !i.IsPaid) 
                .Join(_context.Bookings,
                    invoice => invoice.BookingId,
                    booking => booking.BookingId,
                    (invoice, booking) => new
                    {
                        Invoice = invoice,
                        Booking = booking
                    })
                .Join(_context.Guests,
                    bookingInvoice => bookingInvoice.Booking.GuestId,
                    guest => guest.GuestId,
                    (bookingInvoice, guest) => new
                    {
                        Guest = guest,
                        bookingInvoice.Booking,
                        bookingInvoice.Invoice
                    })
                .ToList();

            if (!nonPaidBookings.Any())
            {
                Console.WriteLine("No non-paid bookings found.");
                return;
            }

            const int pageSize = 5; 
            int currentPage = 0;
            int totalPages = (int)Math.Ceiling((double)nonPaidBookings.Count / pageSize);

            while (true)
            {
                Console.Clear();
                Console.WriteLine($"=== VIEW ALL NON-PAID BOOKINGS (Page {currentPage + 1}/{totalPages}) ===");
                Console.WriteLine(new string('-', 120));
                Console.WriteLine($"{"Booking ID",-12}{"Guest Name",-25}{"Room",-10}{"Amount Due",-15}{"Payment Deadline",-20}");
                Console.WriteLine(new string('-', 120));

                var bookingsOnPage = nonPaidBookings
                    .Skip(currentPage * pageSize)
                    .Take(pageSize)
                    .ToList();

                foreach (var entry in bookingsOnPage)
                {
                    var guest = entry.Guest;
                    var booking = entry.Booking;
                    var invoice = entry.Invoice;

                    Console.WriteLine($"{booking.BookingId,-12}{guest.FirstName + " " + guest.LastName,-25}ID {booking.RoomId,-10}{invoice.TotalAmount,-15:C}{invoice.PaymentDeadline,-20:yyyy-MM-dd}");
                }

                Console.WriteLine(new string('-', 120));
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
                        Console.WriteLine("Exiting non-paid bookings view...");
                        return;
                    default:
                        Console.WriteLine("Invalid choice. Please use [N], [P], or [Q]. Press any key to continue...");
                        Console.ReadKey(true);
                        break;
                }
            }
        }


        public void DisplayPaidBooking()
        {
            Console.Clear();
            Console.WriteLine("page: DisplayPaidBooking");

     
            var paidBookings = _context.Invoices
                .Where(i => i.IsPaid) 
                .Join(_context.Bookings,
                    invoice => invoice.BookingId,
                    booking => booking.BookingId,
                    (invoice, booking) => new
                    {
                        Invoice = invoice,
                        Booking = booking
                    })
                .Join(_context.Guests,
                    bookingInvoice => bookingInvoice.Booking.GuestId,
                    guest => guest.GuestId,
                    (bookingInvoice, guest) => new
                    {
                        Guest = guest,
                        bookingInvoice.Booking,
                        bookingInvoice.Invoice
                    })
                .ToList();

            if (!paidBookings.Any())
            {
                Console.WriteLine("No paid bookings found.");
                return;
            }

            const int pageSize = 5; 
            int currentPage = 0;
            int totalPages = (int)Math.Ceiling((double)paidBookings.Count / pageSize);

            while (true)
            {
                Console.Clear();
                Console.WriteLine($"=== VIEW ALL PAID BOOKINGS (Page {currentPage + 1}/{totalPages}) ===");
                Console.WriteLine(new string('-', 120));
                Console.WriteLine($"{"Booking ID",-12}{"Guest Name",-25}{"Room",-10}{"Amount Paid",-15}{"Paid On",-20}{"Deadline",-15}");
                Console.WriteLine(new string('-', 120));

                var bookingsOnPage = paidBookings
                    .Skip(currentPage * pageSize)
                    .Take(pageSize)
                    .ToList();

                foreach (var entry in bookingsOnPage)
                {
                    var guest = entry.Guest;
                    var booking = entry.Booking;
                    var invoice = entry.Invoice;

                    Console.WriteLine($"{booking.BookingId,-12}{guest.FirstName + " " + guest.LastName,-25}ID {booking.RoomId,-10}{invoice.TotalAmount,-15:C}{invoice.PaymentDeadline,-20:yyyy-MM-dd}{invoice.PaymentDeadline,-15:yyyy-MM-dd}");
                }

                Console.WriteLine(new string('-', 120));
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
                        Console.WriteLine("Exiting paid bookings view...");
                        return;
                    default:
                        Console.WriteLine("Invalid choice. Please use [N], [P], or [Q]. Press any key to continue...");
                        Console.ReadKey(true);
                        break;
                }
            }
        }
        public void CheckOut()
        {
            Console.Clear();
            Console.WriteLine("Page: Checkout()");
            Console.WriteLine("Enter Guest ID to check out:");
            if (int.TryParse(Console.ReadLine(), out int guestId))
            {
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

                var invoice = _context.Invoices.FirstOrDefault(i => i.BookingId == booking.BookingId);
                if (invoice == null)
                {
                    Console.WriteLine("\nGenerating invoice...");
                    invoice = new Invoice
                    {
                        BookingId = booking.BookingId,
                        TotalAmount = CalculateTotalAmount(booking),
                        IsPaid = false,
                        PaymentDeadline = DateTime.Now.AddDays(7)
                    };

                    _context.Invoices.Add(invoice);
                    _context.SaveChanges();

                    Console.WriteLine("Invoice generated successfully.");
                }

                Console.WriteLine($"\nInvoice Details:");
                Console.WriteLine($"Invoice ID: {invoice.InvoiceId}");
                Console.WriteLine($"Total Amount: {invoice.TotalAmount:C}");
                Console.WriteLine($"Payment Deadline: {invoice.PaymentDeadline:yyyy-MM-dd}");

                Console.WriteLine("\nEnter payment amount:");
                if (decimal.TryParse(Console.ReadLine(), out decimal paymentAmount))
                {
                    if (paymentAmount < invoice.TotalAmount)
                    {
                        Console.WriteLine("Insufficient payment. The guest must pay the full amount.");
                        return;
                    }

                    var payment = new Payment
                    {
                        InvoiceId = invoice.InvoiceId,
                        PaymentDate = DateTime.Now,
                        AmountPaid = paymentAmount
                    };

                    _context.Payments.Add(payment);

                    invoice.IsPaid = true;

                    booking.IsCheckedIn = false;
                    booking.IsCheckedOut = true;
                    booking.BookingStatus = true;
                    booking.CheckOutDate = DateTime.Now;

                    _context.SaveChanges();

                    Console.WriteLine("\nPayment processed successfully.");
                    Console.WriteLine($"Booking with Booking ID {booking.BookingId} has been successfully checked out and marked as completed.");
                }
                else
                {
                    Console.WriteLine("Invalid payment amount.");
                }
            }
            else
            {
                Console.WriteLine("Invalid Guest ID.");
            }
        }

        private decimal CalculateTotalAmount(Booking booking)
        {
            if (!booking.CheckInDate.HasValue)
            {
                throw new Exception("Check-In Date is not set.");
            }

            var room = _context.Rooms.FirstOrDefault(r => r.RoomId == booking.RoomId);
            if (room == null)
            {
                throw new Exception("Room not found.");
            }

            var checkOutDate = booking.CheckOutDate ?? DateTime.Now;
            var duration = checkOutDate.Date - booking.CheckInDate.Value.Date;

            if (duration.Days <= 0)
            {
                throw new Exception("Invalid duration for booking. Check-out date must be after check-in date.");
            }

            var dailyRate = room.Type == "Single" ? 100m : 150m;

            return duration.Days * dailyRate;
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
