using HotelBookingApp.Entities;
using HotelBookingApp.Repositories;
using Spectre.Console;
using System.Collections.Generic;
using System.Linq;

namespace HotelBookingApp.Services.DisplayServices
{
    public class TableDisplayService
    {
        private readonly BookingRepository _bookingRepository;
        private readonly GuestRepository _guestRepository;

        public TableDisplayService(BookingRepository bookingRepository, GuestRepository guestRepository)
        {
            _bookingRepository = bookingRepository;
            _guestRepository = guestRepository;
        }
        public void DisplayGuestTable(IEnumerable<Guest> guests)
        {
            var guestTable = new Table()
                .Border(TableBorder.Rounded)
                .AddColumn("[blue]Guest ID[/]")
                .AddColumn("[blue]Name[/]")
                .AddColumn("[blue]Lastname[/]")
                .AddColumn("[blue]Email[/]")
                .AddColumn("[blue]Phone[/]")
                .AddColumn("[blue]Status[/]");

            foreach (var guest in guests)
            {
                bool hasActiveBooking = guest.Bookings?.Any(b => !b.IsCanceled && !b.IsCheckedOut) ?? false;
                guestTable.AddRow(
                    guest.GuestId.ToString(),
                    guest.FirstName,
                    guest.LastName,
                    guest.Email,
                    guest.PhoneNumber,
                    hasActiveBooking
                        ? "[green]Has booking attached, cannot be removed[/]"
                        : "[red]Has no booking attached[/]"
                );
            }

            AnsiConsole.Write(guestTable);
        }


        public void DisplayActiveBookings()
        {
            var bookings = _bookingRepository.GetAllBookings()
                .Where(b => !b.IsCanceled && !b.IsCheckedOut)
                .ToList();
            DisplayTable(bookings, "Active Bookings:", includePaymentAndStatus: true);
        }

        public void DisplayCompletedBookings()
        {
            var bookings = _bookingRepository.GetAllBookings()
                .Where(b => b.IsCheckedOut)
                .ToList();
            DisplayTable(bookings, "Completed Bookings:", includePaymentAndStatus: true);
        }


        private void DisplayTable(IEnumerable<Booking> bookings, string title, bool includePaymentAndStatus)
        {
            if (bookings == null || !bookings.Any())
            {
                AnsiConsole.Markup($"[red]No {title} found.[/]\n");
                return;
            }

            var table = new Table().Border(TableBorder.Rounded);

            table.AddColumn("[bold]Booking ID[/]");
            table.AddColumn("[bold]Guest[/]");
            table.AddColumn("[bold]Room ID[/]");
            table.AddColumn("[bold]Check-In Date[/]");
            table.AddColumn("[bold]Check-Out Date[/]");

            if (includePaymentAndStatus)
            {
                table.AddColumn("[bold]Amount[/]");
                table.AddColumn("[bold]Payment Status[/]");
                table.AddColumn("[bold]Booking Status[/]");
            }

            foreach (var booking in bookings)
            {
                var invoice = booking.Invoices?.OrderByDescending(i => i.PaymentDeadline).FirstOrDefault();
                string paymentStatus = includePaymentAndStatus && invoice != null && invoice.IsPaid
                    ? "[green]Paid[/]"
                    : "[red]Not Paid[/]";
                string bookingStatus = includePaymentAndStatus
                    ? (booking.IsCheckedIn ? "[green]Checked In[/]" : "[yellow]Not Checked In[/]")
                    : string.Empty;
                string amount = includePaymentAndStatus && invoice != null
                    ? $"{invoice.TotalAmount:C}"
                    : "[grey]No Invoice[/]";
                string checkInDate = booking.CheckInDate?.ToString("yyyy-MM-dd") ?? "[grey]N/A[/]";
                string checkOutDate = booking.CheckOutDate?.ToString("yyyy-MM-dd") ?? "[grey]N/A[/]";

                if (includePaymentAndStatus)
                {
                    table.AddRow(
                        booking.BookingId.ToString(),
                        $"{booking.Guest.FirstName} {booking.Guest.LastName}",
                        booking.RoomId.ToString(),
                        checkInDate,
                        checkOutDate,
                        amount,
                        paymentStatus,
                        bookingStatus
                    );
                }
                else
                {
                    table.AddRow(
                        booking.BookingId.ToString(),
                        $"{booking.Guest.FirstName} {booking.Guest.LastName}",
                        booking.RoomId.ToString(),
                        checkInDate,
                        checkOutDate
                    );
                }
            }

            AnsiConsole.Markup($"[bold green]{title}[/]\n");
            AnsiConsole.Write(table);
        }

        public void DisplayCanceledBookings()
        {
            Console.Clear();
            Console.WriteLine("CANCELED BOOKINGS HISTORY");
            Console.WriteLine(new string('-', 80));

            var canceledBookings = _bookingRepository.GetAllBookings()
                .Where(b => b.IsCanceled)
                .ToList();

            if (!canceledBookings.Any())
            {
                Console.WriteLine("No canceled bookings found.");
            }
            else
            {
                var table = new Table();
                table.Border(TableBorder.Rounded);

                table.AddColumn("[bold yellow]Booking ID[/]");
                table.AddColumn("[bold yellow]Guest ID[/]");
                table.AddColumn("[bold yellow]Room ID[/]");
                table.AddColumn("[bold yellow]Check-In Date[/]");
                table.AddColumn("[bold yellow]Check-Out Date[/]");

                foreach (var booking in canceledBookings)
                {
                    table.AddRow(
                        booking.BookingId.ToString(),
                        booking.GuestId.ToString(),
                        booking.RoomId.ToString(),
                        booking.CheckInDate.HasValue ? booking.CheckInDate.Value.ToString("yyyy-MM-dd") : "[grey]N/A[/]",
                        booking.CheckOutDate.HasValue ? booking.CheckOutDate.Value.ToString("yyyy-MM-dd") : "[grey]N/A[/]"
                    );
                }

                AnsiConsole.Write(table);
            }

            Console.WriteLine("Press any key to return...");
            Console.ReadKey();
        }

        public void DisplayBookingTable(IEnumerable<Booking> bookings, string title, bool includePaymentAndStatus = true)
        {
            if (bookings == null || !bookings.Any())
            {
                AnsiConsole.Markup($"[bold gray]There's no existing '{title}' currently.[/]\n");
                return;
            }

            var table = new Table().Border(TableBorder.Rounded);

            table.AddColumn("[bold]Booking ID[/]");
            table.AddColumn("[bold]Guest[/]");
            table.AddColumn("[bold]Room ID[/]");
            table.AddColumn("[bold]Check-In Date[/]");
            table.AddColumn("[bold]Check-Out Date[/]");

            if (includePaymentAndStatus)
            {
                table.AddColumn("[bold]Amount[/]");
                table.AddColumn("[bold]Payment Status[/]");
                table.AddColumn("[bold]Booking Status[/]");
            }

            foreach (var booking in bookings)
            {
                var invoice = booking.Invoices?.OrderByDescending(i => i.PaymentDeadline).FirstOrDefault();
                string paymentStatus = includePaymentAndStatus && invoice != null && invoice.IsPaid
                    ? "[green]Paid[/]"
                    : "[gray]not Paid[/]";
                string bookingStatus = includePaymentAndStatus
                    ? (booking.IsCheckedIn ? "[green]Checked In[/]" : "[gray]Not Checked In[/]")
                    : string.Empty;
                string amount = includePaymentAndStatus && invoice != null
                    ? $"{invoice.TotalAmount:C}"
                    : "[red]No Invoice[/]";
                string checkInDate = booking.CheckInDate?.ToString("yyyy-MM-dd") ?? "[grey]N/A[/]";
                string checkOutDate = booking.CheckOutDate?.ToString("yyyy-MM-dd") ?? "[grey]N/A[/]";

                if (includePaymentAndStatus)
                {
                    table.AddRow(
                        booking.BookingId.ToString(),
                        $"{booking.Guest.FirstName} {booking.Guest.LastName}",
                        booking.RoomId.ToString(),
                        checkInDate,
                        checkOutDate,
                        amount,
                        paymentStatus,
                        bookingStatus
                    );
                }
                else
                {
                    table.AddRow(
                        booking.BookingId.ToString(),
                        $"{booking.Guest.FirstName} {booking.Guest.LastName}",
                        booking.RoomId.ToString(),
                        checkInDate,
                        checkOutDate
                    );
                }
            }

            AnsiConsole.Markup($"[bold yellow]{title}[/]\n");
            AnsiConsole.Write(table);
        }


        public void DisplayRooms(IEnumerable<Room> rooms, string title, bool includeDeleted)
        {
            if (rooms == null || !rooms.Any())
            {
                AnsiConsole.Markup($"[red]No rooms found for {title}[/].\n");
                return;
            }

            var table = new Table()
                .Border(TableBorder.Rounded)
                .AddColumn("[bold]Room ID[/]")
                .AddColumn("[bold]Type[/]")
                .AddColumn("[bold]Price[/]")
                .AddColumn("[bold]Size (sqm)[/]");

            if (!includeDeleted)
            {
                table.AddColumn("[bold]Max People[/]");
            }

            foreach (var room in rooms)
            {
                if (includeDeleted)
                {
                    table.AddRow(
                        room.RoomId.ToString(),
                        room.Type,
                        room.PricePerNight.ToString("C"),
                        room.SizeInSquareMeters.ToString()
                    );
                }
                else
                {
                    table.AddRow(
                        room.RoomId.ToString(),
                        room.Type,
                        room.PricePerNight.ToString("C"),
                        room.SizeInSquareMeters.ToString(),
                        room.TotalPeople.ToString("F1")
                    );
                }
            }

            AnsiConsole.Markup($"[bold green]{title}[/]\n");
            AnsiConsole.Write(table);
        }

        public void DisplayBookedRooms(IEnumerable<Room> bookedRooms, string title)
        {
            if (bookedRooms == null || !bookedRooms.Any())
            {
                AnsiConsole.Markup($"[red]No {title} found.[/]\n");
                return;
            }

            var table = new Table()
                .Border(TableBorder.Rounded)
                .AddColumn("[bold]Room ID[/]")
                .AddColumn("[bold]Booked By[/]")
                .AddColumn("[bold]Start Date[/]")
                .AddColumn("[bold]End Date[/]");

            foreach (var room in bookedRooms)
            {
                foreach (var booking in room.Bookings.Where(b => !b.IsCanceled))
                {
                    table.AddRow(
                        room.RoomId.ToString(),
                        $"{booking.Guest?.FirstName ?? "Unknown"} {booking.Guest?.LastName ?? "Unknown"}",
                        booking.CheckInDate?.ToString("yyyy-MM-dd") ?? "N/A",
                        booking.CheckOutDate?.ToString("yyyy-MM-dd") ?? "N/A"
                    );
                }
            }

            AnsiConsole.Markup($"[bold green]{title}[/]\n");
            AnsiConsole.Write(table);
        }
    }
}
