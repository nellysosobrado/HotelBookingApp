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

        private void DisplayUnpaidBookings(IEnumerable<Booking> bookings)
        {
            var table = new Table()
                .Border(TableBorder.Rounded)
                .AddColumn("[blue]Booking ID[/]")
                .AddColumn("[blue]Guest[/]")
                .AddColumn("[blue]Room ID[/]")
                .AddColumn("[blue]Invoice Amount[/]")
                .AddColumn("[blue]Payment Deadline[/]")
                .AddColumn("[blue]Days Overdue[/]");

            foreach (var booking in bookings)
            {
                var unpaidInvoice = booking.Invoices.FirstOrDefault(i => !i.IsPaid);

                if (unpaidInvoice != null)
                {
                    int daysOverdue = (DateTime.Now - unpaidInvoice.PaymentDeadline).Days;

                    table.AddRow(
                        booking.BookingId.ToString(),
                        $"{booking.Guest.FirstName} {booking.Guest.LastName}",
                        booking.RoomId.ToString(),
                        unpaidInvoice.TotalAmount.ToString("C"),
                        unpaidInvoice.PaymentDeadline.ToString("yyyy-MM-dd"),
                        daysOverdue > 0 ? $"{daysOverdue} days overdue" : "[green]Not overdue[/]"
                    );
                }
            }

            AnsiConsole.MarkupLine("[bold yellow]Unpaid Bookings:[/]");
            AnsiConsole.Write(table);
        }


        private void CancelUnpaidBookings(IEnumerable<Booking> bookings)
        {
            foreach (var booking in bookings)
            {
                var unpaidInvoice = booking.Invoices.FirstOrDefault(i => !i.IsPaid);

                if (unpaidInvoice != null && DateTime.Now > unpaidInvoice.PaymentDeadline)
                {
                    booking.IsCanceled = true;
                    booking.CanceledDate = DateTime.Now;

                    _bookingRepository.UpdateBooking(booking);

                    _bookingRepository.AddCanceledBooking(booking, "Canceled due to unpaid invoice after payment deadline.");
                }
            }
        }


        public void DisplayCanceledBookingHistory()
        {

            var unpaidBookings = _bookingRepository.GetAllBookings()
                .Where(b => !b.IsCanceled
                    && b.Invoices != null
                    && b.Invoices.Any(i => !i.IsPaid && DateTime.Now > i.PaymentDeadline))
                .ToList();

            if (unpaidBookings.Any())
            {
                DisplayUnpaidBookings(unpaidBookings);
                CancelUnpaidBookings(unpaidBookings);
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
                    canceledBooking.GuestName,
                    canceledBooking.RoomId.ToString(),
                    canceledBooking.CanceledDate.ToString("yyyy-MM-dd HH:mm:ss"),
                    canceledBooking.Reason
                );
            }

            AnsiConsole.MarkupLine("[bold yellow]Nullified Booking History (Automatic Removed booking from guest):[/]");
            AnsiConsole.Write(table);
        }

    }
}
