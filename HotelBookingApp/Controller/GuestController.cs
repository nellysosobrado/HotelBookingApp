﻿using HotelBookingApp.Data;
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

        public GuestController(AppDbContext context)
        {
            _guestRepository = new GuestRepository(context);
        }
        public void RegisterNewGuest()
        {
            Console.Clear();
            AnsiConsole.MarkupLine("[bold green]Register new guest[/]\n");

            string firstName = AnsiConsole.Prompt(
                new TextPrompt<string>("[yellow]Enter firstname[/]")
                    .ValidationErrorMessage("[red]Firstname cannot be empty[/]")
                    .Validate(input => !string.IsNullOrWhiteSpace(input)));

            string lastName = AnsiConsole.Prompt(
                new TextPrompt<string>("[yellow]Enter lastname[/]")
                    .ValidationErrorMessage("[red]Last name cannot be empty[/]")
                    .Validate(input => !string.IsNullOrWhiteSpace(input)));

            string email = AnsiConsole.Prompt(
                new TextPrompt<string>("[yellow]Enter gmail[/]")
                    .ValidationErrorMessage("[red]Invalid gmail [/]")
                    .Validate(input => input.Contains("@")));

            string phone = AnsiConsole.Prompt(
                new TextPrompt<string>("[yellow]Enter phone number[/]")
                    .ValidationErrorMessage("[red]Invalid phone number![/]")
                    .Validate(input => long.TryParse(input, out _)));

            AnsiConsole.MarkupLine("\n[bold green]Summary[/]");
            var table = new Table()
                .Border(TableBorder.Rounded)
                .AddColumn("[blue]Fält[/]")
                .AddColumn("[blue]Värde[/]")
                .AddRow("Firstname", firstName)
                .AddRow("Lastname", lastName)
                .AddRow("Gmail", email)
                .AddRow("Phone number", phone);

            AnsiConsole.Write(table);

            bool confirm = AnsiConsole.Confirm("[bold]Would you want to continue?[/]");

            if (!confirm)
            {
                AnsiConsole.MarkupLine("[red]Register has been cancelled.[/]");
                Console.ReadKey();
                return;
            }

            var newGuest = new Guest
            {
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                PhoneNumber = phone
            };

            bool createBooking = AnsiConsole.Confirm("Would you want to create aw booking ?");
            Booking newBooking = null;
            Invoice newInvoice = null;

            if (createBooking)
            {
                int guestCount = AnsiConsole.Prompt(
                    new TextPrompt<int>("[yellow]Enter total guests:[/]")
                        .ValidationErrorMessage("[red]Total guest must be a number![/]")
                        .Validate(input => input > 0));

                DateTime startDate = SelectDate("[yellow]Enter start date:[/]");
                DateTime endDate = SelectDate("[yellow]Entere a end date:[/]");

                if (endDate <= startDate)
                {
                    AnsiConsole.MarkupLine("[red]End date must be a valid date, and not in the past![/]");
                    Console.ReadKey();
                    return;
                }

                var availableRooms = _guestRepository.GetAvailableRooms(startDate, endDate, guestCount);

                if (!availableRooms.Any())
                {
                    AnsiConsole.MarkupLine("[red] No avaiable rooms found [/]");
                    Console.ReadKey();
                    return;
                }

                AnsiConsole.MarkupLine("\n[bold green]Available rooms:[/]");
                var roomTable = new Table()
                    .Border(TableBorder.Rounded)
                    .AddColumn("[blue]Room ID[/]")
                    .AddColumn("[blue]Room Type[/]")
                    .AddColumn("[blue]Price/night[/]");

                foreach (var room in availableRooms)
                {
                    roomTable.AddRow(room.RoomId.ToString(), room.Type, room.PricePerNight.ToString("C"));
                }

                AnsiConsole.Write(roomTable);

                int roomId = AnsiConsole.Prompt(
    new TextPrompt<int>("[yellow]Enter room ID to book:[/]")
        .ValidationErrorMessage("[red]Invalid room Id![/]")
        .Validate(input => availableRooms.Any(r => r.RoomId == input)));
                var selectedRoom = availableRooms.First(r => r.RoomId == roomId);

                int extraBeds = 0;
                if (selectedRoom.Type == "Double")
                {
                    extraBeds = AnsiConsole.Prompt(
                        new TextPrompt<int>("[yellow]How many extra beds would you like? (0-2):[/]")
                            .ValidationErrorMessage("[red]Invalid input! Enter a number between 0 and 2.[/]")
                            .Validate(input => input >= 0 && input <= 2));

                    selectedRoom.TotalPeople += extraBeds;
                }

                var booking = new Booking
                {
                    RoomId = roomId,
                    CheckInDate = startDate,
                    CheckOutDate = endDate,
                    IsCheckedIn = false,
                    IsCheckedOut = false,
                    BookingStatus = false
                };

                decimal totalAmount = _guestRepository.CalculateTotalAmount(booking);
                totalAmount += extraBeds * selectedRoom.ExtraBedPrice; 

                var invoice = new Invoice
                {
                    BookingId = booking.BookingId,
                    TotalAmount = totalAmount,
                    IsPaid = false,
                    PaymentDeadline = endDate.AddDays(7) 
                };

                _guestRepository.RegisterNewGuestWithBooking(newGuest, booking, invoice);


                AnsiConsole.MarkupLine("[bold green]\nGuest has been registered![/]");
                Console.ReadKey();
            }
        }


        private DateTime SelectDate(string prompt)
        {
            DateTime currentDate = DateTime.Now.Date;
            DateTime selectedDate = new DateTime(currentDate.Year, currentDate.Month, 1);

            while (true)
            {
                Console.Clear();
                Console.WriteLine(prompt);
                RenderCalendar(selectedDate);

                var key = Console.ReadKey(true).Key;
                switch (key)
                {
                    case ConsoleKey.RightArrow:
                        selectedDate = selectedDate.AddDays(1);
                        break;
                    case ConsoleKey.LeftArrow:
                        if (selectedDate.AddDays(-1) >= currentDate)
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

        private void RenderCalendar(DateTime selectedDate)
        {
            var calendarContent = new StringWriter();
            calendarContent.WriteLine($"[red]{selectedDate:MMMM}[/]".ToUpper());
            calendarContent.WriteLine("Mon  Tue  Wed  Thu  Fri  Sat  Sun");
            calendarContent.WriteLine("─────────────────────────────────");

            DateTime firstDayOfMonth = new DateTime(selectedDate.Year, selectedDate.Month, 1);
            int daysInMonth = DateTime.DaysInMonth(selectedDate.Year, selectedDate.Month);
            int startDay = (int)firstDayOfMonth.DayOfWeek;
            startDay = (startDay == 0) ? 6 : startDay - 1;

            for (int i = 0; i < startDay; i++)
            {
                calendarContent.Write("     ");
            }

            for (int day = 1; day <= daysInMonth; day++)
            {
                if (day == selectedDate.Day)
                {
                    calendarContent.Write($"[green]{day,2}[/]   ");
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
                Header = new PanelHeader($"[red]{selectedDate:yyyy}[/]", Justify.Center)
            };

            AnsiConsole.Write(panel);
            Console.WriteLine();
            AnsiConsole.MarkupLine("\nUse arrow keys [blue]\u25C4 \u25B2 \u25BA \u25BC[/] to navigate and [green]Enter[/] to select.");
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
