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

        public UnpaidBookingService(BookingRepository bookingRepository)
        {
            _bookingRepository = bookingRepository;
        }

        public void Start()
        {
            bool exit = false;

            while (!exit)
            {
                Console.Clear();
                AnsiConsole.MarkupLine("[blue bold]Unpaid Booking Management[/]");

                var action = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("[green]Select an option:[/]")
                        .AddChoices("Handle Unpaid Bookings", "View Canceled Bookings History", "Return to Main Menu", "Exit"));

                switch (action)
                {
                    case "Handle Unpaid Bookings":
                        HandleUnpaidBookings();
                        break;

                    case "View Canceled Bookings History":
                        DisplayCanceledBookingHistory();
                        break;

                    case "Return to Main Menu":
                        exit = true;
                        break;

                    case "Exit":
                        Environment.Exit(0);
                        break;
                }
            }
        }

        public void HandleUnpaidBookings()
        {
            Console.Clear();
            AnsiConsole.MarkupLine("[bold yellow]Checking for unpaid bookings...[/]");

            var currentDate = DateTime.Now;

            var unpaidBookings = _bookingRepository.GetAllBookings()
                .Where(b => !b.IsCanceled
                && b.Invoices != null
                && b.Invoices.Any(i => !i.IsPaid && (DateTime.Now - i.CreatedDate).Days > 10)) // Basera på fakturans datum
                .ToList();


            if (unpaidBookings.Any())
            {
                DisplayUnpaidBookings(unpaidBookings);
                CancelUnpaidBookings(unpaidBookings);
            }
            else
            {
                AnsiConsole.MarkupLine("[green]No unpaid bookings older than 10 days found.[/]");
            }

            DisplayCanceledBookingHistory();
            Console.ReadKey();
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
                .AddColumn("[blue]Canceled Date[/]")
                .AddColumn("[blue]Reason[/]");

            foreach (var booking in bookings)
            {
                var unpaidInvoice = booking.Invoices.FirstOrDefault(i => !i.IsPaid);

                if (unpaidInvoice != null && (DateTime.Now - unpaidInvoice.CreatedDate).Days > 10)
                {
                    Console.WriteLine($"Processing Booking ID: {booking.BookingId}");
                    Console.WriteLine($"Invoice CreatedDate: {unpaidInvoice.CreatedDate}, Days Overdue: {(DateTime.Now - unpaidInvoice.CreatedDate).Days}");

                    booking.IsCanceled = true;
                    booking.CanceledDate = DateTime.Now;

                    // Uppdatera bokningen och fakturan i databasen
                    _bookingRepository.UpdateBooking(booking);

                    // Lägg till i historik
                    _bookingRepository.AddCanceledBooking(booking, "Canceled due to unpaid invoice over 10 days overdue.");

                    // Lägg till i tabellen
                    table.AddRow(
                        booking.BookingId.ToString(),
                        $"{booking.Guest.FirstName} {booking.Guest.LastName}",
                        booking.RoomId.ToString(),
                        booking.CanceledDate.Value.ToString("yyyy-MM-dd HH:mm:ss"),
                        "[red]Canceled due to unpaid invoice over 10 days overdue.[/]"
                    );
                }
                else
                {
                    // Logga varför bokningen inte annulleras
                    Console.WriteLine($"Skipping Booking ID: {booking.BookingId}");
                    if (unpaidInvoice == null)
                    {
                        Console.WriteLine("Reason: No unpaid invoice found.");
                    }
                    else
                    {
                        Console.WriteLine($"Reason: Invoice is not overdue. Days Overdue: {(DateTime.Now - unpaidInvoice.CreatedDate).Days}");
                    }
                }
            }

            if (table.Rows.Count > 0)
            {
                AnsiConsole.MarkupLine("[bold yellow]The following bookings have been canceled:[/]");
                AnsiConsole.Write(table);
            }
            else
            {
                AnsiConsole.MarkupLine("[green]No bookings were canceled.[/]");
            }
        }


        private void DisplayCanceledBookingHistory()
        {
            var canceledBookings = _bookingRepository.GetCanceledBookingsHistory();

            if (!canceledBookings.Any())
            {
                AnsiConsole.MarkupLine("[red]No canceled bookings found in the history.[/]");
                return;
            }

            var table = new Table()
                .Border(TableBorder.Rounded)
                .AddColumn("[blue]Booking ID[/]")
                .AddColumn("[blue]Guest[/]")
                .AddColumn("[blue]Room ID[/]")
                .AddColumn("[blue]Canceled Date[/]")
                .AddColumn("[blue]Reason[/]");

            foreach (var canceledBooking in canceledBookings)
            {
                table.AddRow(
                    canceledBooking.BookingId.ToString(),
                    canceledBooking.GuestName,
                    canceledBooking.RoomId.ToString(),
                    canceledBooking.CanceledDate.ToString("yyyy-MM-dd HH:mm:ss"),
                    canceledBooking.Reason
                );
            }

            AnsiConsole.MarkupLine("[bold yellow]NULLIFIED Canceled Booking History:[/]");
            AnsiConsole.Write(table);
        }
    }
}
