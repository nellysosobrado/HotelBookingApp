using HotelBookingApp.Data;
using HotelBookingApp.Entities;
using HotelBookingApp.Repositories;
using Microsoft.EntityFrameworkCore;
using Spectre.Console;
using System;
using System.Linq;

namespace HotelBookingApp.Controllers
{
    public class GuestController
    {
        private readonly GuestRepository _guestRepository;
        private readonly BookingRepository _bookingRepository;

        public GuestController(AppDbContext context, BookingRepository bookingRepository, GuestRepository guestRepository)
        {
            _guestRepository = guestRepository;
            _bookingRepository = bookingRepository;
        }
        public void RegisterNewGuest()
        {
            Console.Clear();
            AnsiConsole.MarkupLine("[bold green]Register New Guest[/]\n");

            var guest = CollectGuestInformation();
            if (guest == null)
            {
                AnsiConsole.MarkupLine("[red]Registration has been canceled.[/]");
                return;
            }

            bool createBooking = AnsiConsole.Confirm("Would you like to create a booking for this guest?");
            if (createBooking)
            {
                Booking booking = null;

                while (booking == null)
                {
                    booking = CollectBookingDetailsWithCalendar(guest);
                    if (booking == null)
                    {
                        bool tryAgain = AnsiConsole.Confirm("No rooms available for the selected dates and room type. Would you like to try again?");
                        if (!tryAgain)
                        {
                            AnsiConsole.MarkupLine("[red]Booking has been canceled.[/]");
                            return;
                        }
                    }
                }

                decimal totalAmount = _guestRepository.CalculateTotalAmount(booking);

                var invoice = new Invoice
                {
                    BookingId = booking.BookingId,
                    TotalAmount = totalAmount,
                    IsPaid = false,
                    PaymentDeadline = booking.CheckOutDate?.AddDays(7) ?? DateTime.Now.AddDays(7)
                };

                _guestRepository.RegisterNewGuestWithBookingAndInvoice(guest, booking, invoice);

                AnsiConsole.MarkupLine("[bold green]Guest has been successfully registered and booked![/]");
                AnsiConsole.MarkupLine($"[yellow]Invoice created:[/] Total Amount: {totalAmount:C}, Payment Deadline: {invoice.PaymentDeadline:yyyy-MM-dd}");
            }
            else
            {
                _guestRepository.AddGuest(guest);
                AnsiConsole.MarkupLine("[bold green]Guest has been successfully registered![/]");
            }

            Console.ReadKey();
        }



        private Guest CollectGuestInformation()
        {
            string firstName = AnsiConsole.Prompt(
                new TextPrompt<string>("[yellow]Please enter your first name:[/]")
                    .ValidationErrorMessage("[red]First name cannot be empty[/]")
                    .Validate(input => !string.IsNullOrWhiteSpace(input)));

            string lastName = AnsiConsole.Prompt(
                new TextPrompt<string>("[yellow]Please enter your last name:[/]")
                    .ValidationErrorMessage("[red]Last name cannot be empty[/]")
                    .Validate(input => !string.IsNullOrWhiteSpace(input)));

            string email = AnsiConsole.Prompt(
                new TextPrompt<string>("[yellow]Please enter your email address (must include @):[/]")
                    .ValidationErrorMessage("[red]Invalid email[/]")
                    .Validate(input => input.Contains("@")));

            string phone = AnsiConsole.Prompt(
                new TextPrompt<string>("[yellow]Please enter your phone number:[/]")
                    .ValidationErrorMessage("[red]Invalid phone number![/]")
                    .Validate(input => long.TryParse(input, out _)));

            return new Guest
            {
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                PhoneNumber = phone
            };
        }

        private Booking CollectBookingDetailsWithCalendar(Guest guest)
        {
            string roomType = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[yellow]Please select room type:[/]")
                    .AddChoices("Single", "Double"));

            DateTime startDate = SelectDateWithCalendar("[yellow]Select check-in date:[/]", roomType);
            if (startDate == DateTime.MinValue)
            {
                return null;
            }

            DateTime endDate = SelectDateWithCalendar("[yellow]Select check-out date:[/]", roomType);
            if (endDate == DateTime.MinValue || endDate <= startDate)
            {
                AnsiConsole.MarkupLine("[red]Check-out date must be after check-in date![/]");
                return null;
            }

            var availableRooms = _guestRepository.GetAvailableRooms(startDate, endDate, roomType);
            if (!availableRooms.Any())
            {
                AnsiConsole.MarkupLine("[red]No available rooms found for the selected dates and room type.[/]");
                return null;
            }

            AnsiConsole.MarkupLine("\n[bold green]Available Rooms:[/]");
            var table = new Table()
                .Border(TableBorder.Rounded)
                .AddColumn("[blue]Room ID[/]")
                .AddColumn("[blue]Room Type[/]")
                .AddColumn("[blue]Price per Night[/]");

            foreach (var room in availableRooms)
            {
                table.AddRow(room.RoomId.ToString(), room.Type, room.PricePerNight.ToString("C"));
            }

            AnsiConsole.Write(table);
            int roomId = AnsiConsole.Prompt(
                new TextPrompt<int>("[yellow]Enter Room ID to book:[/]")
                    .ValidationErrorMessage("[red]Invalid Room ID![/]")
                    .Validate(input => availableRooms.Any(r => r.RoomId == input)));

            return new Booking
            {
                RoomId = roomId,
                CheckInDate = startDate,
                CheckOutDate = endDate,
                GuestId = guest.GuestId,
                IsCheckedIn = false,
                IsCheckedOut = false,
                BookingStatus = false
            };
        }



        private DateTime SelectDateWithCalendar(string prompt, string roomType)
        {
            DateTime currentDate = DateTime.Now.Date;
            DateTime selectedDate = currentDate;

            var bookedDates = _bookingRepository.GetAllBookings()
                .Where(b => b.Room.Type.Equals(roomType, StringComparison.OrdinalIgnoreCase))
                .Where(b => b.CheckInDate.HasValue && b.CheckOutDate.HasValue)
                .SelectMany(b => Enumerable.Range(0, 1 + (b.CheckOutDate.Value - b.CheckInDate.Value).Days)
                                            .Select(offset => b.CheckInDate.Value.AddDays(offset)))
                .ToHashSet();

            while (true)
            {
                Console.Clear();
                Console.WriteLine(prompt);
                RenderCalendar(selectedDate, roomType);

                var key = Console.ReadKey(true).Key;
                switch (key)
                {
                    case ConsoleKey.RightArrow:
                        selectedDate = selectedDate.AddDays(1);
                        break;
                    case ConsoleKey.LeftArrow:
                        if (selectedDate > currentDate)
                            selectedDate = selectedDate.AddDays(-1);
                        break;
                    case ConsoleKey.UpArrow:
                        if (selectedDate.AddDays(-7) >= currentDate)
                            selectedDate = selectedDate.AddDays(-7);
                        break;
                    case ConsoleKey.DownArrow:
                        selectedDate = selectedDate.AddDays(7);
                        break;
                    case ConsoleKey.Enter:
                        if (bookedDates.Contains(selectedDate))
                        {
                            AnsiConsole.MarkupLine("[red]The selected date is already booked. Please choose another date.[/]");
                            Console.ReadKey();
                        }
                        else
                        {
                            return selectedDate; 
                        }
                        break;
                    case ConsoleKey.Escape:
                        return DateTime.MinValue; 
                }
            }
        }


        private void RenderCalendar(DateTime selectedDate, string roomType)
        {
            var calendarContent = new StringWriter();
            calendarContent.WriteLine($"[bold yellow]{selectedDate:MMMM yyyy}[/]".ToUpper());
            calendarContent.WriteLine("Mon  Tue  Wed  Thu  Fri  Sat  Sun");
            calendarContent.WriteLine("─────────────────────────────────");

            DateTime firstDayOfMonth = new DateTime(selectedDate.Year, selectedDate.Month, 1);
            int daysInMonth = DateTime.DaysInMonth(selectedDate.Year, selectedDate.Month);
            int startDay = (int)firstDayOfMonth.DayOfWeek;
            startDay = (startDay == 0) ? 6 : startDay - 1;

            // Hämta bokade datum specifikt för valt rumstyp
            var bookedDates = _bookingRepository.GetAllBookings()
                .Where(b => b.Room.Type.Equals(roomType, StringComparison.OrdinalIgnoreCase)) 
                .SelectMany(b => Enumerable.Range(0, 1 + (b.CheckOutDate.Value - b.CheckInDate.Value).Days)
                                            .Select(offset => b.CheckInDate.Value.AddDays(offset)))
                .ToHashSet();

            for (int i = 0; i < startDay; i++)
            {
                calendarContent.Write("     ");
            }

            for (int day = 1; day <= daysInMonth; day++)
            {
                DateTime currentDate = new DateTime(selectedDate.Year, selectedDate.Month, day);

                if (currentDate == selectedDate)
                {
                    calendarContent.Write($"[blue]{day,2}[/]   ");
                }
                else if (bookedDates.Contains(currentDate))
                {
                    calendarContent.Write($"[red]{day,2}[/]   ");
                }
                else if (currentDate < DateTime.Now.Date)
                {
                    calendarContent.Write($"[grey]{day,2}[/]   ");
                }
                else
                {
                    calendarContent.Write($"{day,2}   ");
                }

                if ((startDay + day) % 7 == 0)
                {
                    calendarContent.WriteLine();
                }
            }

            var panel = new Panel(calendarContent.ToString())
            {
                Border = BoxBorder.Double,
                Header = new PanelHeader($"[yellow]{selectedDate:yyyy}[/]", Justify.Center)
            };

            AnsiConsole.Write(panel);
            Console.WriteLine();
            AnsiConsole.MarkupLine("[blue]Use arrow keys to navigate and Enter to select a date. Press Escape to cancel.[/]");
        }

        private DateTime SelectDate(string prompt, string selectedRoomType)
        {
            DateTime currentDate = DateTime.Now.Date;
            DateTime selectedDate = currentDate;  

            while (true)
            {
                Console.Clear();
                Console.WriteLine(prompt);
                RenderCalendar(selectedDate, selectedRoomType);  

                var key = Console.ReadKey(true).Key;
                switch (key)
                {
                    case ConsoleKey.RightArrow:
                        selectedDate = selectedDate.AddDays(1); 
                        break;
                    case ConsoleKey.LeftArrow:
                        selectedDate = selectedDate.AddDays(-1);  
                        break;
                    case ConsoleKey.UpArrow:
                        selectedDate = selectedDate.AddDays(-7); 
                        break;
                    case ConsoleKey.DownArrow:
                        selectedDate = selectedDate.AddDays(7);  
                        break;
                    case ConsoleKey.Enter:
                        var bookings = _bookingRepository.GetAllBookings()
                            .Where(b => b.CheckInDate.HasValue && b.CheckOutDate.HasValue)
                            .Where(b => b.CheckInDate.Value.Date <= selectedDate.Date && b.CheckOutDate.Value.Date >= selectedDate.Date)
                            .Where(b => b.Room.Type == selectedRoomType)  
                            .ToList();

                        if (bookings.Any())
                        {
                            AnsiConsole.MarkupLine("[red]The selected date is already booked for this room type.[/]");  
                            Console.ReadKey(true);
                            continue;  
                        }

                        if (selectedDate >= currentDate)
                            return selectedDate;  

                        AnsiConsole.MarkupLine("[red]The date cannot be in the past.[/]");  
                        Console.ReadKey(true);
                        break;
                    case ConsoleKey.Escape:
                        return DateTime.MinValue; 
                }
            }
        }


        private DateTime PromptForDate(string message)
        {
            while (true)
            {
                Console.Write(message);
                if (DateTime.TryParse(Console.ReadLine(), out DateTime date))
                {
                    if (date.Date < DateTime.Now.Date)
                    {
                        Console.WriteLine("The date cannot be in the past. Please enter a valid future date.");
                        continue;
                    }
                    return date;
                }
                Console.WriteLine("Invalid date format. Please use yyyy-MM-dd.");
            }
        }

        public void UpdateGuestInformation()
        {
            Console.Clear();
            Console.WriteLine("UPDATE GUEST DETAILS");

            var guests = _guestRepository.GetAllGuests();

            if (!guests.Any())
            {
                Console.WriteLine("No guests found.");
                Console.ReadKey(true);
                return;
            }

            Console.WriteLine("\nSelect a guest to update by ID:");
            foreach (var guest in guests)
            {
                Console.WriteLine($"ID: {guest.GuestId} | Name: {guest.FirstName} {guest.LastName}");
            }

            var guestId = PromptForInt("Enter Guest ID: ");
            var selectedGuest = _guestRepository.GetGuestById(guestId);

            if (selectedGuest == null)
            {
                Console.WriteLine("Guest not found. Returning to menu...");
                Console.ReadKey(true);
                return;
            }

            Console.WriteLine("\nLeave fields blank to keep current value.");

            selectedGuest.FirstName = PromptInput($"Current First Name: {selectedGuest.FirstName}\nEnter new First Name: ", selectedGuest.FirstName);
            selectedGuest.LastName = PromptInput($"Current Last Name: {selectedGuest.LastName}\nEnter new Last Name: ", selectedGuest.LastName);
            selectedGuest.Email = PromptInput($"Current Email: {selectedGuest.Email}\nEnter new Email: ", selectedGuest.Email);
            selectedGuest.PhoneNumber = PromptInput($"Current Phone Number: {selectedGuest.PhoneNumber}\nEnter new Phone Number: ", selectedGuest.PhoneNumber);

            _guestRepository.UpdateGuest(selectedGuest);

            Console.WriteLine("\nGuest information updated successfully!");
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey(true);
        }

        public void ViewAllGuests()
        {
            Console.Clear();
            Console.WriteLine("ALL GUESTS");

            var guests = _guestRepository.GetGuestsWithBookings();

            if (!guests.Any())
            {
                Console.WriteLine("No guests found.");
                Console.ReadKey(true);
                return;
            }

            foreach (var entry in guests)
            {
                var guest = ((dynamic)entry).Guest;
                var bookings = ((dynamic)entry).Bookings as IEnumerable<dynamic>;

                foreach (var booking in bookings)
                {
                    var invoice = booking.Invoice != null
                        ? $"Amount: {booking.Invoice.TotalAmount:C}, Status: {(booking.Invoice.IsPaid ? "Paid" : "Not Paid")}"
                        : "No Invoice"; 

                    Console.WriteLine($"Guest: {guest.FirstName} {guest.LastName} | Room: {booking.RoomId} | " +
                                      $"Check-In: {booking.CheckInDate:yyyy-MM-dd} | Check-Out: {booking.CheckOutDate:yyyy-MM-dd} | {invoice}");
                }
            }

            Console.WriteLine("\nPress any key to return...");
            Console.ReadKey(true);
        }



        private string PromptInput(string message, string defaultValue = null)
        {
            Console.Write(message);
            var input = Console.ReadLine();
            return string.IsNullOrWhiteSpace(input) ? defaultValue : input;
        }

        private int PromptForInt(string message)
        {
            int result;
            while (true)
            {
                Console.Write(message);
                if (int.TryParse(Console.ReadLine(), out result))
                {
                    return result;
                }
                Console.WriteLine("Invalid input. Please enter a valid number.");
            }
        }

    }
}
