using System;
using System.Collections.Generic;
using System.Linq;
using HotelBookingApp.Entities;
using HotelBookingApp.Repositories;
using Spectre.Console;

namespace HotelBookingApp.Services.BookingServices
{
    public class UnpaidBookingService
    {
        private readonly BookingRepository _bookingRepository;
        private readonly List<Booking> _canceledBookingHistory = new(); // Håll historik för annullerade bokningar

        public UnpaidBookingService(BookingRepository bookingRepository)
        {
            _bookingRepository = bookingRepository;
        }

        public void HandleUnpaidBookings()
        {
            while (true)
            {
                Console.Clear();
                AnsiConsole.MarkupLine("[bold yellow]Checking for unpaid bookings...[/]");

                var currentDate = DateTime.Now;

                // Hämta obetalda bokningar
                var unpaidBookings = _bookingRepository.GetAllBookings()
                    .Where(b => !b.IsCanceled && b.Invoices != null && b.Invoices.Any(i => !i.IsPaid && (currentDate - i.CreatedDate).Days > 10))
                    .ToList();

                if (unpaidBookings.Any())
                {
                    // Visa en tabell med detaljer om obetalda bokningar
                    DisplayUnpaidBookings(unpaidBookings);

                    // Automatisk annullering av obetalda bokningar
                    CancelUnpaidBookings(unpaidBookings);
                }
                else
                {
                    AnsiConsole.MarkupLine("[green]No unpaid bookings older than 10 days found.[/]");
                }

                // Visa historik över alla annullerade bokningar
                DisplayCanceledBookingHistory();

                // Fråga användaren vad de vill göra härnäst
                var action = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("[green]What would you like to do next?[/]")
                        .AddChoices("Check Again", "View Canceled Booking History", "Return to Main Menu", "Exit")
                        .HighlightStyle(new Style(foreground: Color.Green))
                );

                switch (action)
                {
                    case "Check Again":
                        continue;
                    case "View Canceled Booking History":
                        DisplayCanceledBookingHistory();
                        break;
                    case "Return to Main Menu":
                        return;
                    case "Exit":
                        Environment.Exit(0);
                        break;
                }
            }
        }

        private void DisplayUnpaidBookings(IEnumerable<Booking> bookings)
        {
            var table = new Table()
                .Border(TableBorder.Rounded)
                .AddColumn("[blue]Booking ID[/]")
                .AddColumn("[blue]Guest[/]")
                .AddColumn("[blue]Room ID[/]")
                .AddColumn("[blue]Invoice Amount[/]")
                .AddColumn("[blue]Days Overdue[/]");

            foreach (var booking in bookings)
            {
                var unpaidInvoice = booking.Invoices.FirstOrDefault(i => !i.IsPaid);

                table.AddRow(
                    booking.BookingId.ToString(),
                    $"{booking.Guest.FirstName} {booking.Guest.LastName}",
                    booking.RoomId.ToString(),
                    unpaidInvoice?.TotalAmount.ToString("C") ?? "[red]N/A[/]",
                    $"{(DateTime.Now - unpaidInvoice.CreatedDate).Days} days"
                );
            }

            AnsiConsole.MarkupLine("[bold yellow]Unpaid Bookings:[/]");
            AnsiConsole.Write(table);
        }

        private void CancelUnpaidBookings(IEnumerable<Booking> bookings)
        {
            var table = new Table()
                .Border(TableBorder.Rounded)
                .AddColumn("[blue]Booking ID[/]")
                .AddColumn("[blue]Guest[/]")
                .AddColumn("[blue]Room ID[/]")
                .AddColumn("[blue]Reason[/]");

            foreach (var booking in bookings)
            {
                booking.IsCanceled = true;

                // Annullera alla kopplade fakturor
                foreach (var invoice in booking.Invoices)
                {
                    invoice.IsPaid = false;
                }

                _bookingRepository.UpdateBooking(booking);

                // Lägg till bokningen i historiken
                _canceledBookingHistory.Add(booking);

                // Lägg till information om den annullerade bokningen i tabellen
                table.AddRow(
                    booking.BookingId.ToString(),
                    $"{booking.Guest.FirstName} {booking.Guest.LastName}",
                    booking.RoomId.ToString(),
                    "[red]Canceled due to unpaid invoice over 10 days overdue.[/]"
                );
            }

            Console.Clear();
            AnsiConsole.MarkupLine("[bold yellow]The following bookings have been canceled:[/]");
            AnsiConsole.Write(table);
        }

        private void DisplayCanceledBookingHistory()
        {
            Console.Clear();
            if (!_canceledBookingHistory.Any())
            {
                AnsiConsole.MarkupLine("[red]No canceled bookings found in the history.[/]");
                return;
            }

            var table = new Table()
                .Border(TableBorder.Rounded)
                .AddColumn("[blue]Booking ID[/]")
                .AddColumn("[blue]Guest[/]")
                .AddColumn("[blue]Room ID[/]")
                .AddColumn("[blue]Reason[/]");

            foreach (var booking in _canceledBookingHistory)
            {
                table.AddRow(
                    booking.BookingId.ToString(),
                    $"{booking.Guest.FirstName} {booking.Guest.LastName}",
                    booking.RoomId.ToString(),
                    "[red]Booking was canceled manually or due to unpaid invoices.[/]"
                );
            }

            AnsiConsole.MarkupLine("[bold yellow]Canceled Booking History:[/]");
            AnsiConsole.Write(table);
            Console.ReadKey();
        }
    }
}
