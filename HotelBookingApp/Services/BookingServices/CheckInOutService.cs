using HotelBookingApp.Entities;
using HotelBookingApp.Repositories;
using HotelBookingApp.Services.DisplayServices;
using Spectre.Console;
using System;
using System.Linq;

namespace HotelBookingApp.Services.BookingServices
{
    public class CheckInOutService
    {
        private readonly BookingRepository _bookingRepository;
        private readonly TableDisplayService _tableDisplayService;

        public CheckInOutService(BookingRepository bookingRepository, TableDisplayService tableDisplayService)
        {
            _bookingRepository = bookingRepository;
            _tableDisplayService = tableDisplayService;
        }

        public void Execute()
        {
            while (true)
            {
                Console.Clear();

                _tableDisplayService.DisplayBookingStatuses();

                var activeBookings = _bookingRepository.GetActiveBookings()
                  .Where(b => !b.IsCanceled) 
                  .ToList();

                if (!activeBookings.Any())
                {
                    AnsiConsole.MarkupLine("[red]No active bookings available.[/]");
                    Console.ReadKey();
                    break;
                }

                var bookingChoices = activeBookings
                    .Select(b => $"{b.BookingId}: {b.Guest.FirstName} {b.Guest.LastName}")
                    .ToList();

                bookingChoices.Add("Back");

                var selectedBooking = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("[bold green]Select a booking[/]")
                        .AddChoices(bookingChoices)
                        .HighlightStyle(new Style(foreground: Color.Green))
                );

                if (selectedBooking == "Back")
                    break;

                if (!int.TryParse(selectedBooking.Split(':')[0], out var bookingId))
                {
                    AnsiConsole.MarkupLine("[red]Invalid booking selection. Try again.[/]");
                    Console.ReadKey();
                    continue;
                }

                var booking = _bookingRepository.GetBookingById(bookingId);
                if (booking == null || booking.IsCanceled) 
                {
                    AnsiConsole.MarkupLine("[red]Selected booking could not be found or has been removed. Try again.[/]");
                    Console.ReadKey();
                    continue;
                }

                var action = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("[bold green]What action would you like to perform?[/]")
                        .AddChoices("Check In", "Check Out", "Go Back")
                        .HighlightStyle(new Style(foreground: Color.Green))
                );

                switch (action)
                {
                    case "Check In":
                        HandleCheckIn(booking);
                        break;

                    case "Check Out":
                        HandleCheckOut(booking);
                        activeBookings = _bookingRepository.GetActiveBookings()
                            .Where(b => !b.IsCanceled) 
                            .ToList();
                        break;

                    case "Go Back":
                        continue;
                }

                AnsiConsole.MarkupLine("[green]Press any key to continue[/]");
                Console.ReadKey();

            }
        }
        private void HandleCheckIn(Entities.Booking booking)
        {
            if (booking.IsCheckedIn)
            {
                AnsiConsole.MarkupLine("[yellow]Booking is already checked in.[/]");
                return;
            }

            var invoice = _bookingRepository.GetInvoiceByBookingId(booking.BookingId);
            if (invoice != null && !invoice.IsPaid)
            {
                AnsiConsole.MarkupLine($"[yellow]Invoice Amount: {invoice.TotalAmount:C} is unpaid.[/]");
                var confirmPayment = AnsiConsole.Confirm("[bold green]Does the guest want to pay now?[/]");
                if (confirmPayment)
                {
                    invoice.IsPaid = true;
                    _bookingRepository.UpdateInvoice(invoice);
                    AnsiConsole.MarkupLine("[green]Payment completed successfully.[/]");
                }
                else
                {
                    AnsiConsole.MarkupLine("[yellow]Check-In proceeding without payment.[/]");
                }
            }

            booking.IsCheckedIn = true;
            booking.CheckInDate = DateTime.Now;
            _bookingRepository.UpdateBooking(booking);

            AnsiConsole.MarkupLine("[green]Check-In completed successfully.[/]");
            //UpdateAndDisplayBookings();
        }

        private void HandleCheckOut(Entities.Booking booking)
        {
            
            if (!booking.IsCheckedIn)
            {
                AnsiConsole.MarkupLine("[Bold red]Cannot check out. Booking needs to check in first![/]");
                return;
            }

            var invoice = _bookingRepository.GetInvoiceByBookingId(booking.BookingId);
            if (invoice != null && !invoice.IsPaid)
            {
                AnsiConsole.MarkupLine($"[bold]Invoice Amount: {invoice.TotalAmount:C} must be paid before checking out.[/]");
                var confirmPayment = AnsiConsole.Confirm("[bold green]Does the guest want to pay now?[/]");
                if (!confirmPayment)
                {
                    AnsiConsole.MarkupLine("[red]Check-Out canceled due to unpaid invoice.[/]");
                    return;
                }

                invoice.IsPaid = true;
                _bookingRepository.UpdateInvoice(invoice);
                AnsiConsole.MarkupLine("[green]Payment completed successfully.[/]");
            }

            booking.IsCheckedOut = true;
            booking.CheckOutDate = DateTime.Now;
            _bookingRepository.UpdateBooking(booking);

            AnsiConsole.MarkupLine("[green]Check-Out completed successfully.[/]");
           // UpdateAndDisplayBookings();
        }

        private void UpdateAndDisplayBookings()
        {
            Console.Clear();

            var activeBookings = _bookingRepository.GetActiveBookings().ToList();

            var completedBookings = _bookingRepository.GetCompletedBookings().ToList();

            if (activeBookings.Any())
            {
                Console.WriteLine(new string('-', 100));
                _tableDisplayService.DisplayBookingTable(activeBookings, "Active Bookings", includePaymentAndStatus: true);
            }
            else
            {
                AnsiConsole.MarkupLine("[bold gray]No active bookings found.[/]");
            }

            if (completedBookings.Any())
            {
                _tableDisplayService.DisplayBookingTable(completedBookings, "Completed Bookings", includePaymentAndStatus: false);
                Console.WriteLine(new string('-', 100));
            }
            else
            {
                Console.WriteLine(new string('-', 100));
                AnsiConsole.MarkupLine("[bold gray]No completed bookings found.[/]");
                Console.WriteLine(new string('-', 100));
            }
        }

    }
}
