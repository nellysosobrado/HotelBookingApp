﻿using HotelBookingApp.Repositories;
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
        public GuestBookings(GuestRepository guestRepository, BookingRepository bookingRepository)
        {
            _guestRepository = guestRepository;
            _bookingRepository = bookingRepository;
        }
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
            AnsiConsole.MarkupLine("[italic yellow]Register new bookign with new guest[/]\n");

            Guest newGuest;
            FluentValidationResult validationResult;

            do
            {
                newGuest = new Guest
                {
                    FirstName = AnsiConsole.Prompt(new TextPrompt<string>("[white]Enter First Name:[/]")),
                    LastName = AnsiConsole.Prompt(new TextPrompt<string>("[white]Enter Last Name:[/]")),
                    Email = AnsiConsole.Prompt(new TextPrompt<string>("[white]Enter Email Address:[/]")),
                    PhoneNumber = AnsiConsole.Prompt(new TextPrompt<string>("[white]Enter Phone Number :[/]"))
                };

                var validator = new GuestValidator();
                validationResult = validator.Validate(newGuest);

                if (!validationResult.IsValid)
                {
                    AnsiConsole.MarkupLine("[bold red]Error:[/]");
                    foreach (var failure in validationResult.Errors)
                    {
                        AnsiConsole.MarkupLine($"[red]- {failure.PropertyName}: {failure.ErrorMessage}[/]");
                    }

                    AnsiConsole.MarkupLine("[yellow]Please try again.[/]\n");
                }
            }
            while (!validationResult.IsValid); 

            _guestRepository.AddGuest(newGuest);

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
            Console.Clear();
            AnsiConsole.MarkupLine($"[green]Total Amount: {totalAmount:C}[/]");
            AnsiConsole.MarkupLine($"[green]Payment Deadline: {invoice.PaymentDeadline:yyyy-MM-dd}[/]");
 
            return booking;
            
        }


        public void RegisterBookingForExistingGuest()
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
                        AnsiConsole.MarkupLine($"[red]Room {booking.RoomId} is already booked during the selected period by another guest.[/]");
                        AnsiConsole.MarkupLine($"[red]Conflicting Booking: Guest ID {conflictingBooking.GuestId}, Check-In: {conflictingBooking.CheckInDate:yyyy-MM-dd}, Check-Out: {conflictingBooking.CheckOutDate:yyyy-MM-dd}[/]");
                        booking = null;

                        bool tryAgain = AnsiConsole.Confirm("[yellow]Would you like to select a different room?[/]");
                        if (!tryAgain)
                        {
                            AnsiConsole.MarkupLine("[red]Booking process canceled.[/]");
                            Console.ReadKey();
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
            Console.ReadKey();
        }





        private Entities.Booking CollectBookingDetailsWithCalendar(Guest guest)
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
    }
}