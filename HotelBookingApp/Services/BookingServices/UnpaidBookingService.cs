using System;
using System.Collections.Generic;
using System.Linq;
using HotelBookingApp.Data;
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

        public void DisplayUnpaidBookings(IEnumerable<Booking> bookings)
        {
            var table = new Table()
                .Border(TableBorder.Rounded)
                .AddColumn("[white]Booking ID[/]")
                .AddColumn("[white]Guest[/]")
                .AddColumn("[white]Invoice Amount[/]")
                .AddColumn("[red]Payment Deadline[/]");

            foreach (var booking in bookings)
            {
                var unpaidInvoice = booking.Invoices.First(i => !i.IsPaid);
                table.AddRow(
                    booking.BookingId.ToString(),
                    $"{booking.Guest.FirstName} {booking.Guest.LastName}",
                    unpaidInvoice.TotalAmount.ToString("C"),
                    unpaidInvoice.PaymentDeadline.ToString("yyyy-MM-dd")
                );
            }

            AnsiConsole.MarkupLine("[bold red]Unpaid Bookings:[/]");
            AnsiConsole.Write(table);
        }


        public void DisplayCanceledBookingHistory()
        {
            var unpaidBookings = _bookingRepository.GetAllBookings()
                .Where(b => !_bookingRepository.GetCanceledBookingsHistory().Any(cb => cb.BookingId == b.BookingId)
                    && b.Invoices != null
                    && b.Invoices.Any(i => !i.IsPaid && DateTime.Now > i.PaymentDeadline))
                .ToList();

            if (unpaidBookings.Any())
            {
                DisplayUnpaidBookings(unpaidBookings);
                _bookingRepository.CancelUnpaidBookings(unpaidBookings);
            }

            var canceledBookings = _bookingRepository.GetCanceledBookingsHistory();

            if (!canceledBookings.Any())
            {
                AnsiConsole.Markup($"[bold gray]There's no existing 'Canceled Bookings' currently.[/]\n");
                return;
            }

            var table = new Table()
                .Border(TableBorder.Rounded)
                .AddColumn("[white]Booking ID[/]")
                .AddColumn("[white]Guest[/]")
                .AddColumn("[white]Room ID[/]")
                .AddColumn("[white]Canceled Date[/]")
                .AddColumn("[red]Reason[/]");

            foreach (var canceledBooking in canceledBookings)
            {
                table.AddRow(
                    canceledBooking.BookingId.ToString(),
                    canceledBooking.GuestName ?? "[grey]Unknown Guest[/]",
                    canceledBooking.RoomId.ToString(),
                    canceledBooking.CanceledDate.ToString("yyyy-MM-dd HH:mm:ss"),
                    canceledBooking.Reason ?? "[grey]No reason provided[/]"
                );
            }

            AnsiConsole.MarkupLine("[bold yellow]HISTORY Of Cancel/Unbook Bookings:[/]");
            AnsiConsole.Write(table);
        }


    }
}