using HotelBookingApp.Data;
using HotelBookingApp.Entities;
using HotelBookingApp.Repositories;
using HotelBookingApp.Services.BookingServices;
using HotelBookingApp.Services.GuestServices;
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
        private readonly DisplayRegisteredGuestsService _displayRegisteredGuestsService;
        private readonly GuestRemovalService _guestRemovalService;

        public GuestController(AppDbContext context, BookingRepository bookingRepository, GuestRepository guestRepository, DisplayRegisteredGuestsService displayRegisteredGuestsService, GuestRemovalService guestRemovalService)
        {
            _guestRepository = guestRepository;
            _bookingRepository = bookingRepository;
            _displayRegisteredGuestsService = displayRegisteredGuestsService;
            _guestRemovalService = guestRemovalService;

        }
        //CRUD , CREATE, READ, UPDATE, DELETE
        public void Run()
        {
            var action = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[bold green]What would you like to do?[/]")
                    .AddChoices(new[]
                    {
                    "Create new guest",
                    "Read all guestst",
                    "Update Guest Information",
                    "Remove Guest",
                    "Back"
                    })
                    .HighlightStyle(new Style(foreground: Color.Green))
            );

            switch (action)
            {
                case "Create new guest":
                    RegisterNewGuest();
                    break;
                case "Read all guestst":
                    _displayRegisteredGuestsService.DisplayAllRegisteredGuests();
                    break;
                case "Update Guest Information":
                    UpdateGuestInformation();
                    break;
                case "Remove Guest":
                    _guestRemovalService.Execute();
                    break;
                case "Back":
                    return;
                default:
                    AnsiConsole.Markup("[red]Invalid option. Try again.[/]\n");
                    break;
            }
        }

        //---

        public void RegisterBooking()
        {
            Console.Clear();
            AnsiConsole.MarkupLine("[italic white]Register Booking[/]\n");

            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[yellow]Choose an option to register a booking:[/]")
                    .AddChoices("New Guest", "Existing Guest", "Cancel")
            );

            switch (choice)
            {
                case "New Guest":
                    RegisterBookingForNewGuest();
                    break;

                case "Existing Guest":
                    RegisterBookingForExistingGuest();
                    break;

                case "Cancel":
                    AnsiConsole.MarkupLine("[red]Booking process canceled.[/]");
                    break;

                default:
                    AnsiConsole.MarkupLine("[red]Invalid choice. Returning to main menu.[/]");
                    break;
            }

            Console.ReadKey();
        }

        public void RegisterBookingForNewGuest()
        {
            Console.Clear();
            AnsiConsole.MarkupLine("[italic white]Add Booking for New Guest[/]\n");

            var newGuest = new Guest
            {
                FirstName = AnsiConsole.Prompt(new TextPrompt<string>("[yellow]Enter First Name:[/]")),
                LastName = AnsiConsole.Prompt(new TextPrompt<string>("[yellow]Enter Last Name:[/]")),
                Email = AnsiConsole.Prompt(new TextPrompt<string>("[yellow]Enter Email Address:[/]"))
            };

            _guestRepository.AddGuest(newGuest);

            AnsiConsole.MarkupLine("[bold green]New guest successfully added![/]");

            var booking = CreateNewBooking(newGuest);
            if (booking == null) return;

            AnsiConsole.MarkupLine("[bold green]Booking successfully added for new guest![/]");
            Console.ReadKey();
        }
        private Booking CreateNewBooking(Guest guest)
        {
            Booking booking = null;

            while (booking == null)
            {
                booking = CollectBookingDetailsWithCalendar(guest);
                if (booking == null)
                {
                    bool tryAgain = AnsiConsole.Confirm("[red]No rooms available. Would you like to try again?[/]");
                    if (!tryAgain)
                    {
                        AnsiConsole.MarkupLine("[red]Booking process canceled.[/]");
                        Console.ReadKey();
                        return null;
                    }
                }
            }

            _bookingRepository.AddBooking(booking);

            if (booking.BookingId <= 0)
            {
                AnsiConsole.MarkupLine("[red]Failed to save booking. Cannot create invoice.[/]");
                Console.ReadKey();
                return null;
            }

            decimal totalAmount = _guestRepository.CalculateTotalAmount(booking);

            var invoice = new Invoice
            {
                BookingId = booking.BookingId,
                TotalAmount = totalAmount,
                IsPaid = false,
                PaymentDeadline = DateTime.Now.AddDays(10)
            };

            _guestRepository.AddInvoice(invoice);

            AnsiConsole.MarkupLine($"[yellow]Invoice created:[/] Total Amount: {totalAmount:C}, Payment Deadline: {invoice.PaymentDeadline:yyyy-MM-dd}");
            return booking;
        }



        //public void RegisterBookingForExistingGuest()
        //{
        //    Console.Clear();
        //    AnsiConsole.MarkupLine("[italic white]Add Booking for Existing Guest[/]\n");

        //    var guests = _guestRepository.GetAllGuests();

        //    if (!guests.Any())
        //    {
        //        AnsiConsole.MarkupLine("[red]No guests found in the system.[/]");
        //        Console.ReadKey();
        //        return;
        //    }

        //    var table = new Table()
        //        .AddColumn("[yellow]Guest ID[/]")
        //        .AddColumn("[yellow]First Name[/]")
        //        .AddColumn("[yellow]Last Name[/]")
        //        .AddColumn("[yellow]Email[/]");

        //    foreach (var guest in guests)
        //    {
        //        table.AddRow(
        //            guest.GuestId.ToString(),
        //            guest.FirstName,
        //            guest.LastName,
        //            guest.Email
        //        );
        //    }

        //    AnsiConsole.Write(table);

        //    int guestId = AnsiConsole.Prompt(
        //        new TextPrompt<int>("[yellow]Enter the Guest ID to add a booking for:[/]")
        //            .ValidationErrorMessage("[red]Please enter a valid numeric Guest ID.[/]")
        //            .Validate(id => guests.Any(g => g.GuestId == id))
        //    );

        //    var selectedGuest = guests.First(g => g.GuestId == guestId);

        //    var existingBooking = _bookingRepository.GetBookingsByGuestId(guestId)
        //        .FirstOrDefault(b => !b.IsCheckedOut);

        //    if (existingBooking != null)
        //    {
        //        AnsiConsole.MarkupLine($"[red]Guest {selectedGuest.FirstName} {selectedGuest.LastName} already has an active booking for Room {existingBooking.RoomId} from {existingBooking.CheckInDate:yyyy-MM-dd} to {existingBooking.CheckOutDate:yyyy-MM-dd}.[/]");
        //        Console.ReadKey();
        //        return;
        //    }

        //    var booking = CreateNewBooking(selectedGuest);
        //    if (booking == null) return;

        //    AnsiConsole.MarkupLine("[bold green]Booking successfully added for existing guest![/]");
        //    Console.ReadKey();
        //}



        //--
        public void RegisterBookingForExistingGuest()
        {
            Console.Clear();
            AnsiConsole.MarkupLine("[italic white]Add Booking for Existing Guest[/]\n");

            // Hämta alla gäster som kan registrera en bokning (ej borttagna och utan aktiv bokning)
            var guests = _guestRepository.GetAllGuests()
                .Where(g => !g.IsDeleted && !_bookingRepository.GetBookingsByGuestId(g.GuestId).Any(b => !b.IsCheckedOut))
                .ToList();

            if (!guests.Any())
            {
                AnsiConsole.MarkupLine("[red]No guests available for booking.[/]");
                Console.ReadKey();
                return;
            }

            // Visa endast gäster som kan registrera en bokning
            var table = new Table()
                .AddColumn("[yellow]Guest ID[/]")
                .AddColumn("[yellow]First Name[/]")
                .AddColumn("[yellow]Last Name[/]")
                .AddColumn("[yellow]Email[/]");

            foreach (var guest in guests)
            {
                table.AddRow(
                    guest.GuestId.ToString(),
                    guest.FirstName,
                    guest.LastName,
                    guest.Email
                );
            }

            AnsiConsole.Write(table);

            int guestId = AnsiConsole.Prompt(
                new TextPrompt<int>("[yellow]Enter the Guest ID to add a booking for:[/]")
                    .ValidationErrorMessage("[red]Please enter a valid numeric Guest ID.[/]")
                    .Validate(id => guests.Any(g => g.GuestId == id))
            );

            var selectedGuest = guests.First(g => g.GuestId == guestId);

            Booking booking = null;
            while (booking == null)
            {
                booking = CollectBookingDetailsWithCalendar(selectedGuest);
                if (booking == null)
                {
                    bool tryAgain = AnsiConsole.Confirm("[red]No rooms available. Would you like to try again?[/]");
                    if (!tryAgain)
                    {
                        AnsiConsole.MarkupLine("[red]Booking process canceled.[/]");
                        Console.ReadKey();
                        return;
                    }
                }
            }

            _bookingRepository.AddBooking(booking);

            if (booking.BookingId <= 0)
            {
                AnsiConsole.MarkupLine("[red]Failed to save booking. Cannot create invoice.[/]");
                Console.ReadKey();
                return;
            }

            decimal totalAmount = _guestRepository.CalculateTotalAmount(booking);

            var invoice = new Invoice
            {
                BookingId = booking.BookingId,
                TotalAmount = totalAmount,
                IsPaid = false,
                PaymentDeadline = DateTime.Now.AddDays(10)
            };

            _guestRepository.AddInvoice(invoice);

            AnsiConsole.MarkupLine("[bold green]Booking successfully added for existing guest![/]");
            AnsiConsole.MarkupLine($"[yellow]Invoice created:[/] Total Amount: {totalAmount:C}, Payment Deadline: {invoice.PaymentDeadline:yyyy-MM-dd}");
            Console.ReadKey();
        }



        public void RegisterNewGuest()
        {
            Console.Clear();

            bool proceed = AnsiConsole.Confirm("[italic white]Do you want to register a new guest?[/]");
            if (!proceed)
            {
                AnsiConsole.MarkupLine("[red]Guest registration canceled. Returning to the main menu.[/]");
                return;
            }

            AnsiConsole.MarkupLine("[italic white]Register New Guest[/]\n");

            var guest = CollectGuestInformation();
            if (guest == null)
            {
                AnsiConsole.MarkupLine("[red]Registration has been canceled.[/]");
                return;
            }

            _guestRepository.AddGuest(guest);
            AnsiConsole.MarkupLine("[bold green]Guest has been successfully registered![/]");
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
                .AddColumn("[blue]Price per Night[/]")
                .AddColumn("[blue]Size (sqm)[/]")
                .AddColumn("[blue]Max People[/]");

            foreach (var room in availableRooms)
            {
                table.AddRow(
                    room.RoomId.ToString(),
                    room.Type,
                    room.PricePerNight.ToString("C"),
                    room.SizeInSquareMeters.ToString(),
                    room.TotalPeople.ToString()
                );
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
                BookingCompleted = false
            };
        }
        private DateTime SelectDateWithCalendar(string prompt, string roomType)
        {
            DateTime currentDate = DateTime.Now.Date;
            DateTime selectedDate = currentDate;

            var bookedDates = _bookingRepository.GetAllBookings()
                .Where(b => b.Room.Type.Equals(roomType, StringComparison.OrdinalIgnoreCase))
                .Where(b => b.CheckInDate.HasValue && b.CheckOutDate.HasValue)
                .Where(b => b.CheckOutDate.Value >= b.CheckInDate.Value)
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


        public void UpdateGuestInformation()
        {
            Console.Clear();
            AnsiConsole.MarkupLine("[bold cyan]UPDATE GUEST DETAILS[/]");

            var guests = _guestRepository.GetAllGuests();

            if (!guests.Any())
            {
                AnsiConsole.MarkupLine("[red]No guests found.[/]");
                Console.ReadKey(true);
                return;
            }

            var table = new Table();
            table.AddColumn("[bold yellow]ID[/]");
            table.AddColumn("[bold yellow]First Name[/]");
            table.AddColumn("[bold yellow]Last Name[/]");
            table.AddColumn("[bold yellow]Email[/]");
            table.AddColumn("[bold yellow]Phone Number[/]");

            void RefreshTable()
            {
                table.Rows.Clear();
                foreach (var guest in guests)
                {
                    table.AddRow(
                        guest.GuestId.ToString(),
                        guest.FirstName,
                        guest.LastName,
                        guest.Email,
                        guest.PhoneNumber
                    );
                }
                Console.Clear();
                AnsiConsole.Write(table);
            }

            RefreshTable();

            int guestId;
            while (true)
            {
                Console.Clear();
                AnsiConsole.Write(table);
                AnsiConsole.MarkupLine("[bold grey] To go back enter 'back'[/]");
                var input = AnsiConsole.Ask<string>("[bold green]\nEnter the Guest ID you want to update:[/]");
                if (input.Trim().ToLower() == "back")
                {
                    AnsiConsole.MarkupLine("[yellow]Returning to menu...[/]");
                    return;
                }

                if (!int.TryParse(input, out guestId))
                {
                    AnsiConsole.MarkupLine("[red]Enter a valid ID. Press any key to try again[/]");
                    continue;
                }

                var selectedGuest = _guestRepository.GetGuestById(guestId);

                if (selectedGuest == null)
                {
                    AnsiConsole.MarkupLine("[red]Guest not found. Press any key to try again.[/]");
                    Console.ReadKey();
                    continue;
                }

                while (true)
                {
                    var columnToUpdate = AnsiConsole.Prompt(
                        new SelectionPrompt<string>()
                            .HighlightStyle(new Style(foreground: Color.Green))
                            .Title("[bold green]Choose a option:[/]")
                            .AddChoices("First Name", "Last Name", "Email", "Phone Number", "Back")
                    );

                    if (columnToUpdate.ToLower() == "back")
                    {
                        AnsiConsole.MarkupLine("[yellow]Returning to menu...[/]");
                        return;
                    }

                    switch (columnToUpdate)
                    {
                        case "First Name":
                            selectedGuest.FirstName = AnsiConsole.Ask<string>(
                                $"[bold green]Current First Name:[/] {selectedGuest.FirstName}\nEnter new First Name ");
                            break;
                        case "Last Name":
                            selectedGuest.LastName = AnsiConsole.Ask<string>(
                                $"[bold green]Current Last Name:[/] {selectedGuest.LastName}\nEnter new Last Name ");
                            break;
                        case "Email":
                            while (true)
                            {
                                var email = AnsiConsole.Ask<string>(
                                    $"[bold green]Current Email:[/] {selectedGuest.Email}\nEnter new Email ");
                                if (string.IsNullOrWhiteSpace(email) || email.Contains("@"))
                                {
                                    selectedGuest.Email = email;
                                    break;
                                }
                                AnsiConsole.MarkupLine("[red]Invalid email format. Please enter a valid email address.[/]");
                            }
                            break;
                        case "Phone Number":
                            while (true)
                            {
                                var phone = AnsiConsole.Ask<string>(
                                    $"[bold green]Current Phone Number:[/] {selectedGuest.PhoneNumber}\nEnter new Phone Number ");
                                if (string.IsNullOrWhiteSpace(phone) || phone.All(char.IsDigit))
                                {
                                    selectedGuest.PhoneNumber = phone;
                                    break;
                                }
                                AnsiConsole.MarkupLine("[red]Invalid phone number. Please enter only numeric values.[/]");
                            }
                            break;
                    }

                    _guestRepository.UpdateGuest(selectedGuest);
                    AnsiConsole.MarkupLine("[bold green]\nGuest information has been updated! Press any key to continue[/]");
                    Console.ReadKey();
                    RefreshTable();
                }
            }
        }

    }
}
