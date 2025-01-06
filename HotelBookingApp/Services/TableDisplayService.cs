using HotelBookingApp.Data;
using HotelBookingApp.Entities;
using HotelBookingApp.Repositories;
using Microsoft.EntityFrameworkCore;
using Spectre.Console;
using System.Collections.Generic;
using System.Linq;

namespace HotelBookingApp.Services.DisplayServices
{
    public class TableDisplayService
    {
        private readonly BookingRepository _bookingRepository;
        private readonly GuestRepository _guestRepository;
        private readonly RoomRepository _roomRepository;

        public TableDisplayService(BookingRepository bookingRepository, GuestRepository guestRepository, RoomRepository roomRepository)
        {
            _bookingRepository = bookingRepository;
            _guestRepository = guestRepository;
            _roomRepository = roomRepository;
        }
        public void DisplayBookings()
        {
            while (true)
            {
                var action = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("[italic yellow]Display Bookings[/]")
                        .AddChoices(new[]
                        {
                    "Display Canceled Bookings",
                    "Display Booking Statuses",
                    "Back"
                        })
                        .HighlightStyle(new Style(foreground: Color.Green))
                );

                switch (action)
                {
                    case "Display Canceled Bookings":
                        DisplayCanceledBookings();
                        break;

                    case "Display Booking Statuses":
                        DisplayBookingStatuses();
                        break;

                    case "Back":
                        return;

                    default:
                        AnsiConsole.Markup("[red]Invalid option. Try again.[/]\n");
                        break;
                }
            }
        }

        public void DisplayBookingStatuses()
        {
            while (true)
            {
                Console.Clear();

                var allBookings = _bookingRepository.GetAllBookings();
                if (allBookings == null || !allBookings.Any())
                {
                    AnsiConsole.Markup("[red]No bookings available.[/]\n");
                    Console.WriteLine("Press any key to return...");
                    Console.ReadKey();
                    return;
                }

                foreach (var booking in allBookings)
                {
                    var invoice = booking.Invoices?.FirstOrDefault();
                    if (invoice != null)
                    {
                        if (!invoice.IsPaid && invoice.PaymentDeadline < DateTime.Now)
                        {
                            booking.IsCanceled = true;
                            _bookingRepository.UpdateBooking(booking);
                        }
                    }
                }

                var table = new Table()
                    .Border(TableBorder.Rounded)
                    .AddColumn("[bold]Booking ID[/]")
                    .AddColumn("[bold]Guest Name[/]")
                    .AddColumn("[bold]Room ID[/]")
                    .AddColumn("[bold]Check-In Date[/]")
                    .AddColumn("[bold]Check-Out Date[/]")
                    .AddColumn("[bold]Registration Date[/]")
                    .AddColumn("[bold]Status[/]")
                    .AddColumn("[bold]Payment Status[/]")
                    .AddColumn("[bold]Days Remaining[/]")
                    .AddColumn("[bold]Payment Deadline Day[/]");

                foreach (var booking in allBookings)
                {
                    string status = "[grey]Unknown[/]";
                    string paymentStatus = "[grey]No Invoice[/]";
                    string daysRemaining = "[grey]N/A[/]";
                    string paymentDeadline = "[grey]N/A[/]";
                    string roomId = booking.RoomId.ToString();
                    string checkInDate = booking.CheckInDate?.ToString("yyyy-MM-dd") ?? "[grey]N/A[/]";
                    string checkOutDate = booking.CheckOutDate?.ToString("yyyy-MM-dd") ?? "[grey]N/A[/]";
                    string registrationDate = booking.RegistrationDate.ToString("yyyy-MM-dd HH:mm:ss");

                    if (booking.IsCanceled)
                    {
                        var invoice = booking.Invoices?.OrderByDescending(i => i.PaymentDeadline).FirstOrDefault();

                        if (invoice != null && !invoice.IsPaid && invoice.PaymentDeadline < DateTime.Now)
                        {
                            status = "[red]Overdue Payment, removed[/]";
                            paymentStatus = "[red]Overdue Payment[/]";
                        }
                        else
                        {
                            status = "[red]Removed by user[/]";
                            paymentStatus = "[red]Removed[/]";
                        }

                        roomId = "[red]removed[/]";
                        checkInDate = "[red]removed[/]";
                        checkOutDate = "[red]removed[/]";
                        registrationDate = "[red]removed[/]";
                        daysRemaining = "[red]removed[/]";
                        paymentDeadline = "[red]removed[/]";
                    }
                    else if (booking.IsCheckedOut)
                    {
                        status = "[blue]Checked Out[/]";
                    }
                    else if (booking.IsCheckedIn)
                    {
                        status = "[green]Checked In[/]";
                    }

                    else
                    {
                        status = "[grey]Not Checked In Yet[/]";
                    }

                    var activeInvoice = booking.Invoices?.OrderByDescending(i => i.PaymentDeadline).FirstOrDefault();
                    if (activeInvoice != null && !booking.IsCanceled)
                    {
                        if (activeInvoice.IsPaid)
                        {
                            paymentStatus = "[green]Paid[/]";
                            daysRemaining = "[green]Paid[/]";
                            paymentDeadline = "[green]Paid[/]";
                        }
                        else if (activeInvoice.PaymentDeadline < DateTime.Now)
                        {
                            paymentStatus = "[red]Overdue Payment[/]";
                            daysRemaining = "[red]Overdue[/]";
                        }
                        else
                        {
                            paymentStatus = "[yellow]Not Paid[/]";
                            int remainingDays = (activeInvoice.PaymentDeadline - DateTime.Now).Days;
                            daysRemaining = $"[yellow]{remainingDays} days[/]";
                            paymentDeadline = $"[yellow]{activeInvoice.PaymentDeadline:yyyy-MM-dd}[/]";
                        }
                    }

                    table.AddRow(
                        booking.BookingId.ToString(),
                        $"{booking.Guest?.FirstName ?? "Unknown"} {booking.Guest?.LastName ?? "Unknown"}",
                        roomId,
                        checkInDate,
                        checkOutDate,
                        registrationDate,
                        status,
                        paymentStatus,
                        daysRemaining,
                        paymentDeadline
                    );
                }


                AnsiConsole.Markup("[bold yellow]Booking Status Overview[/]\n");
                AnsiConsole.Write(table);

                Console.WriteLine("Press 'R' to refresh or 'C' to continue...");
                var key = Console.ReadKey(true).Key;

                if (key == ConsoleKey.C) break;
                if (key != ConsoleKey.R) continue;
            }
        }


        public void DisplayGuestTable(IEnumerable<Guest> guests)
        {
            var guestBookingStatus = _guestRepository.GetGuestBookingStatus();

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
                bool hasActiveBooking = guestBookingStatus.ContainsKey(guest.GuestId) && guestBookingStatus[guest.GuestId];

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
            var activeBookings = _bookingRepository.GetActiveBookings();

            DisplayTable(activeBookings, "Active Bookings:", includePaymentAndStatus: true);
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
            while (true)
            {
                Console.Clear();
                Console.WriteLine("CANCELED BOOKINGS HISTORY");
                Console.WriteLine(new string('-', 80));

                var canceledBookings = _bookingRepository.GetCanceledBookingsHistory();

                if (!canceledBookings.Any())
                {
                    Console.WriteLine("No canceled bookings found.");
                }
                else
                {
                    var table = new Table();
                    table.Border(TableBorder.Rounded);

                    table.AddColumn("[bold yellow]Booking ID[/]");
                    table.AddColumn("[bold yellow]Guest Name[/]");
                    table.AddColumn("[bold yellow]Room ID[/]");
                    table.AddColumn("[bold yellow]Canceled Date[/]");
                    table.AddColumn("[bold yellow]Reason[/]");

                    foreach (var canceledBooking in canceledBookings)
                    {
                        table.AddRow(
                            canceledBooking.BookingId.ToString(),
                            canceledBooking.GuestName ?? "[grey]Unknown[/]",
                            canceledBooking.RoomId.ToString(),
                            canceledBooking.CanceledDate.ToString("yyyy-MM-dd"),
                            canceledBooking.Reason ?? "[grey]No reason provided[/]"
                        );
                    }

                    AnsiConsole.Write(table);
                }

                var action = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("[italic yellow]Canceled Bookings Management[/]")
                .AddChoices(new[]
                {
                    "Refresh",
                    "Back"
                })
                .HighlightStyle(new Style(foreground: Color.Green))
        );

                switch (action)
                {

                    case "Refresh":
                        continue;

                    case "Back":
                        return; 

                    default:
                        AnsiConsole.Markup("[red]Invalid option. Try again.[/]\n");
                        break;
                }
            }
        }

        public void DisplayBookingTable(IEnumerable<Booking> bookings, string title, bool includePaymentAndStatus = true)
        {
            Console.Clear();
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
                table.AddColumn("[bold]Payment Deadline[/]");
                table.AddColumn("[bold]Booking Status[/]");
            }

            foreach (var booking in bookings)
            {
                if (booking.Guest == null || booking.Guest.IsDeleted)
                {
                    continue;
                }

                var guestName = $"{booking.Guest.FirstName ?? "Unknown"} {booking.Guest.LastName ?? "Unknown"}";
                var roomId = booking.RoomId.ToString();
                var checkInDate = booking.CheckInDate?.ToString("yyyy-MM-dd") ?? "[grey]N/A[/]";
                var checkOutDate = booking.CheckOutDate?.ToString("yyyy-MM-dd") ?? "[grey]N/A[/]";

                string amount = "[red]No Invoice[/]";
                string paymentStatus = "[grey]N/A[/]";
                string paymentDeadline = "[grey]N/A[/]";
                string bookingStatus = "[grey]N/A[/]";

                if (includePaymentAndStatus)
                {
                    var invoice = booking.Invoices?.OrderByDescending(i => i.PaymentDeadline).FirstOrDefault();
                    if (invoice != null)
                    {
                        amount = $"{invoice.TotalAmount:C}";
                        paymentStatus = invoice.IsPaid ? "[green]Paid[/]" : "[red]Not Paid[/]";
                        paymentDeadline = invoice.PaymentDeadline.ToString("yyyy-MM-dd");
                    }

                    bookingStatus = booking.IsCheckedIn
                        ? "[green]Checked In[/]"
                        : booking.IsCheckedOut
                            ? "[red]Checked Out[/]"
                            : "[yellow]Pending[/]";
                }

                if (includePaymentAndStatus)
                {
                    table.AddRow(
                        booking.BookingId.ToString(),
                        guestName,
                        roomId,
                        checkInDate,
                        checkOutDate,
                        amount,
                        paymentStatus,
                        paymentDeadline,
                        bookingStatus
                    );
                }
                else
                {
                    table.AddRow(
                        booking.BookingId.ToString(),
                        guestName,
                        roomId,
                        checkInDate,
                        checkOutDate
                    );
                }
            }

            AnsiConsole.Markup($"[bold yellow]{title}[/]\n");
            AnsiConsole.Write(table);
            Console.WriteLine("Press any key "); 
            Console.ReadKey();
        }


        public void DisplayRooms()
        {
            while (true)
            {
                Console.Clear();

                var rooms = _roomRepository.GetAllRooms();

                if (rooms == null || !rooms.Any())
                {
                    Console.WriteLine("No rooms found in the database.");
                    return;
                }

                var table = new Table()
                    .Border(TableBorder.Rounded)
                    .AddColumn("[bold]Room ID[/]")
                    .AddColumn("[bold]Type[/]")
                    .AddColumn("[bold]Price/Night[/]")
                    .AddColumn("[bold]Size (sqm)[/]")
                    .AddColumn("[bold]Max People[/]")
                    .AddColumn("[bold]Extra Beds[/]")
                    .AddColumn("[bold]Status[/]");

                foreach (var room in rooms)
                {
                    string status;
                    if (room.IsDeleted)
                    {
                        status = "[red]Removed[/]";
                    }
                    else if (!room.IsAvailable)
                    {
                        status = "[yellow]Inactive[/]";
                    }
                    else if (room.Bookings != null && room.Bookings.Any())
                    {
                        status = "[green]Active[/]";
                    }
                    else
                    {
                        status = "[blue]No bookings attached[/]";
                    }

                    table.AddRow(
                        room.RoomId.ToString(),
                        room.Type,
                        room.PricePerNight.ToString("C"),
                        room.SizeInSquareMeters.ToString(),
                        room.TotalPeople.ToString("F1"),
                        room.ExtraBeds.ToString(),
                        status
                    );
                }

                Console.WriteLine("\nRegistered rooms");
                AnsiConsole.Write(table);

                Console.WriteLine("\nPress 'c' to continue");
                var key = Console.ReadKey(intercept: true);
                if (key.Key == ConsoleKey.C)
                {
                    break; 
                }
            }
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

            var activeRooms = bookedRooms
                .Where(room => room.Bookings.Any(booking => !booking.IsCanceled));

            foreach (var room in activeRooms)
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
