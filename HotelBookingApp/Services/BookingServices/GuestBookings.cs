using HotelBookingApp.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation; 
using Spectre.Console; 
using FluentValidationResult = FluentValidation.Results.ValidationResult;
using SpectreValidationResult = Spectre.Console.ValidationResult;
using HotelBookingApp.Entities;


namespace HotelBookingApp.Services.BookingServices
{
    public class GuestBookings
    {
        private readonly GuestRepository _guestRepository;
        private readonly BookingRepository _bookingRepository;
        private readonly RoomRepository _roomRepository;
        public GuestBookings(GuestRepository guestRepository, BookingRepository bookingRepository, RoomRepository roomRepository)
        {
            _guestRepository = guestRepository;
            _bookingRepository = bookingRepository;
            _roomRepository = roomRepository;
        }
        public void RegisterBooking()
        {
            Console.Clear();

            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[italic yellow]Register a booking[/]")
                    .AddChoices("Register a booking for a new guest", "Register a booking for a existing guest", "Cancel")
            );

            switch (choice)
            {
                case "Register a booking for a new guest":
                    RegisterBookingForNewGuest();
                    break;

                case "Register a booking for a existing guest":
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
            bool keepRunning = true;

            while (keepRunning)
            {
                Console.Clear();
                AnsiConsole.MarkupLine("[italic yellow]Register a booking for a new guest[/]\n");

                Guest newGuest;
                FluentValidationResult validationResult;

                do
                {
                    newGuest = new Guest
                    {
                        FirstName = AnsiConsole.Prompt(new TextPrompt<string>("[white]Enter First Name:[/]")),
                        LastName = AnsiConsole.Prompt(new TextPrompt<string>("[white]Enter Last Name:[/]")),
                        Email = AnsiConsole.Prompt(new TextPrompt<string>("[white]Enter Email Address:[/]")),
                        PhoneNumber = AnsiConsole.Prompt(new TextPrompt<string>("[white]Enter Phone Number (must be Swedish, e.g., +46701234567 or 0701234567):[/]"))
                    };

                    if (_guestRepository.GetAllGuests().Any(g => g.Email == newGuest.Email))
                    {
                        AnsiConsole.MarkupLine("[bold red]Error: The email address already exists in the database.[/]");
                        continue;
                    }

                    if (_guestRepository.GetAllGuests().Any(g => g.PhoneNumber == newGuest.PhoneNumber))
                    {
                        AnsiConsole.MarkupLine("[bold red]Error: The phone number already exists in the database.[/]");
                        continue;
                    }

                    var validator = new GuestValidator();
                    validationResult = validator.Validate(newGuest);

                    if (!validationResult.IsValid)
                    {
                        AnsiConsole.MarkupLine("[bold red]Validation Error:[/]");
                        foreach (var failure in validationResult.Errors)
                        {
                            AnsiConsole.MarkupLine($"[red]- {failure.PropertyName}: {failure.ErrorMessage}[/]");
                        }

                        AnsiConsole.MarkupLine("[yellow]Please try again.[/]\n");
                    }
                    else
                    {
                        break; 
                    }
                }
                while (true);

                _guestRepository.AddGuest(newGuest);

                var booking = CreateNewBooking(newGuest);
                if (booking == null)
                {
                    AnsiConsole.MarkupLine("[red]Booking creation was canceled.[/]");
                }
                else
                {
                    AnsiConsole.MarkupLine("[bold green]\nBooking successfully added for new guest![/]");
                }

                var choice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .HighlightStyle("green")
                        .AddChoices("Register another guest", "Back")
                );

                if (choice == "Back")
                    keepRunning = false;
            }
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
            Console.Clear();

            var table = new Table()
                .Border(TableBorder.Rounded)
                .AddColumn(new TableColumn("[yellow]Guest Name[/]").Centered())
                .AddColumn(new TableColumn("[yellow]Room ID[/]").Centered())
                .AddColumn(new TableColumn("[yellow]Check-In Date[/]").Centered())
                .AddColumn(new TableColumn("[yellow]Check-Out Date[/]").Centered())
                .AddColumn(new TableColumn("[green]Total Amount[/]").Centered())
                .AddColumn(new TableColumn("[green]Payment Deadline[/]").Centered());

            table.AddRow(
                $"{guest.FirstName} {guest.LastName}",
                $"{booking.RoomId}",
                $"{booking.CheckInDate:yyyy-MM-dd}",
                $"{booking.CheckOutDate:yyyy-MM-dd}",
                $"{totalAmount:C}",
                $"{invoice.PaymentDeadline:yyyy-MM-dd}"
            );

            // Visa tabellen
            AnsiConsole.Write(table);

            return booking;
            
        }

        public void RegisterBookingForExistingGuest()
        {
            bool keepRunning = true;

            while (keepRunning)
            {
                Console.Clear();
                AnsiConsole.MarkupLine("[italic white]Add Booking for Existing Guest[/]\n");

                var guests = _guestRepository.GetAllGuests()
                    .Where(g => !g.IsDeleted)
                    .ToList();

                if (!guests.Any())
                {
                    AnsiConsole.MarkupLine("[red]No registered guests found in the system.[/]");
                    Console.ReadKey();
                    return;
                }

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

                Entities.Booking booking = null;

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
                            keepRunning = false; 
                            return;
                        }
                    }
                    else
                    {
                        var conflictingBooking = _bookingRepository.GetBookingsByRoomId(booking.RoomId)
                            .FirstOrDefault(b =>
                                (b.CheckInDate < booking.CheckOutDate && booking.CheckInDate < b.CheckOutDate) && !b.IsCheckedOut);

                        if (conflictingBooking != null)
                        {
                            AnsiConsole.MarkupLine($"[red]Room {booking.RoomId} is already booked during the selected period by {conflictingBooking.GuestId}.[/]");
                            AnsiConsole.MarkupLine($"[red]Booking: Guest ID {conflictingBooking.GuestId}, Check-In: {conflictingBooking.CheckInDate:yyyy-MM-dd}, Check-Out: {conflictingBooking.CheckOutDate:yyyy-MM-dd}[/]");
                            booking = null;

                            bool tryAgain = AnsiConsole.Confirm("[yellow]Would you like to select a different room?[/]");
                            if (!tryAgain)
                            {
                                AnsiConsole.MarkupLine("[red]Booking process canceled.[/]");
                                Console.ReadKey();
                                keepRunning = false; 
                                return;
                            }
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
                AnsiConsole.MarkupLine($"[white]Press any key to continue[/]");
                Console.ReadKey();

                keepRunning = AnsiConsole.Confirm("[yellow]Would you like to make another booking? [/]");
            }
        }
        //private List<Room> GetAvailableRooms(DateTime startDate, DateTime endDate, string roomType)
        //{
        //    var rooms = _roomRepository.GetRoomsWithBookings();

        //    // Hämta avbokade bokningar
        //    var canceledBookingIds = _bookingRepository.GetCanceledBookingsHistory()
        //        .Select(cb => cb.BookingId)
        //        .ToHashSet();

        //    return rooms
        //        .Where(room =>
        //            !room.IsDeleted && // Uteslut raderade rum
        //            room.Type.Equals(roomType, StringComparison.OrdinalIgnoreCase) && // Filtrera på rumstyp
        //            !room.Bookings.Any(booking =>
        //                !canceledBookingIds.Contains(booking.BookingId) && // Exkludera avbokade bokningar
        //                booking.CheckInDate.HasValue &&
        //                booking.CheckOutDate.HasValue &&
        //                !(booking.CheckOutDate.Value <= startDate || booking.CheckInDate.Value >= endDate))) // Kontrollera överlappande bokningar
        //        .ToList();
        //}


        private Entities.Booking CollectBookingDetailsWithCalendar(Guest guest)
        {
            string roomType = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[yellow]Please select room type:[/]")
                    .AddChoices("Single", "Double"));

            DateTime startDate = SelectDateWithCalendar("Select check-in date:[/]", roomType);
            if (startDate == DateTime.MinValue)
            {
                return null;
            }

            DateTime endDate = SelectDateWithCalendar("Select check-out date:[/]", roomType);
            if (endDate == DateTime.MinValue || endDate < startDate)
            {
                AnsiConsole.MarkupLine("[red]Check-out date must be on or after check-in date![/]");
                return null;
            }

            if (endDate == startDate)
            {
                endDate = startDate.AddDays(1); 
                AnsiConsole.MarkupLine("[blue]Single-day booking: Check-out adjusted to the next day.[/]");
            }

            var availableRooms = _roomRepository.GetAvailableRooms(startDate, endDate, roomType);

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

            var roomBookingConflict = _bookingRepository.GetBookingsByRoomId(roomId)
                .FirstOrDefault(b =>
                    (b.CheckInDate < endDate && startDate < b.CheckOutDate) &&
                    !b.IsCheckedOut);

            if (roomBookingConflict != null)
            {
                AnsiConsole.MarkupLine($"[red]Room {roomId} is already booked during the selected period by another guest.[/]");
                AnsiConsole.MarkupLine($"[red]Conflicting Booking: Guest ID {roomBookingConflict.GuestId}, Check-In: {roomBookingConflict.CheckInDate:yyyy-MM-dd}, Check-Out: {roomBookingConflict.CheckOutDate:yyyy-MM-dd}[/]");
                return null;
            }

            return new Entities.Booking
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



        public DateTime SelectDateWithCalendar(string prompt, string roomType)
        {
            DateTime currentDate = DateTime.Now.Date;
            DateTime selectedDate = currentDate;

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
                        return selectedDate;  
                    case ConsoleKey.Escape:
                        return DateTime.MinValue;  
                }
            }
        }
        public void RenderCalendar(DateTime selectedDate, string roomType)
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

    }
}
