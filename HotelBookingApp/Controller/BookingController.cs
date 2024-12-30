using HotelBookingApp.Repositories;
using Spectre.Console;
using HotelBookingApp.Controllers;
using HotelBookingApp.Services.DisplayServices;
using HotelBookingApp.Services.BookingServices;
using HotelBookingApp.Services.GuestServices;

namespace HotelBookingApp
{
    public class BookingController
    {
        private readonly BookingRepository _bookingRepository;
        private readonly GuestController _guestController;
        private readonly TableDisplayService _tableDisplayService;
        private readonly CheckInOutService _checkInOutService;
        private readonly BookingEditService _bookingEditService;
        private readonly UnbookBooking _unbookBooking;
        private readonly PaymentService _paymentService;
        private readonly GuestRemovalService _guestRemovalService;
        private readonly UnpaidBookingService _unpaidBookingService;


        public BookingController(
            BookingRepository bookingRepository,
            GuestController guestController,
            TableDisplayService tableDisplayService,
            CheckInOutService checkInOutService,
            BookingEditService bookingEditService,
            UnbookBooking unbookBooking,
            PaymentService paymentService,
            GuestRemovalService guestRemovalService,
            UnpaidBookingService UnpaidBookingService)
        {
            _bookingRepository = bookingRepository;
            _guestController = guestController;
            _tableDisplayService = tableDisplayService;
            _checkInOutService = checkInOutService;
            _bookingEditService = bookingEditService;
            _unbookBooking = unbookBooking;
            _paymentService = paymentService;   
            _guestRemovalService = guestRemovalService;
            _unpaidBookingService = UnpaidBookingService;
        }


        public void DisplayAllRegisteredGuests()
        {
            Console.Clear();
            Console.WriteLine("ALL REGISTERED GUESTS");
            Console.WriteLine(new string('-', 60));

            var guests = _bookingRepository.GetAllBookings()
                .Select(b => b.Guest)
                .Distinct()
                .ToList();

            if (!guests.Any())
            {
                AnsiConsole.MarkupLine("[red]No registered guests found.[/]");
            }
            else
            {
                var table = new Table();

                table.AddColumn("Guest ID");
                table.AddColumn("Name");
                table.AddColumn("Email");
                table.AddColumn("Phone Number");

                foreach (var guest in guests)
                {
                    table.AddRow(
                        guest.GuestId.ToString(),
                        $"{guest.FirstName} {guest.LastName}",
                        guest.Email,
                        guest.PhoneNumber);
                }

                AnsiConsole.Write(table);
            }
            Console.ReadKey();
        }
        public void BookingOptions()
        {
            Console.Clear();
            //AnsiConsole.MarkupLine("[bold yellow]Processing unpaid bookings...[/]");
            //_unpaidBookingService.HandleUnpaidBookings();

            while (true)
            {
                Console.Clear();

                var activeBookings = _bookingRepository.GetAllBookings()
                    .Where(b => !b.IsCanceled && !b.IsCheckedOut)
                    .ToList();
                var completedBookings = _bookingRepository.GetAllBookings()
                    .Where(b => b.IsCheckedOut)
                    .ToList();
                var removedBookings = _bookingRepository.GetAllBookings()
                    .Where(b => b.IsCanceled)
                    .ToList();

                _tableDisplayService.DisplayBookingTable(activeBookings, "Active Bookings:");
                Console.WriteLine(new string('-', 100));

                _tableDisplayService.DisplayBookingTable(completedBookings, "Completed Bookings:", includePaymentAndStatus: true);
                Console.WriteLine(new string('-', 100));

                _tableDisplayService.DisplayBookingTable(removedBookings, "Unbooked Bookings:", includePaymentAndStatus: false);
                Console.WriteLine(new string('-', 100));

                _unpaidBookingService.DisplayCanceledBookingHistory();
               Console.WriteLine(new string('-', 100));

                var action = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("[bold green]What would you like to do?[/]")
                        .AddChoices(new[] { "Check In/Check Out", "Register New Booking", "Edit Booking", "Unbook Booking", "Guest Payments", "Display All Registered Guests", "Remove Guest", "Back" })
                        .HighlightStyle(new Style(foreground: Color.Green))
                );

                switch (action)
                {
                    case "Check In/Check Out":
                        _checkInOutService.Execute();
                        break;
                    case "Register New Booking":
                        _guestController.RegisterNewGuest();
                        break;
                    case "Edit Booking":
                        _bookingEditService.EditBooking();
                        break;
                    case "Unbook Booking":
                        _unbookBooking.UnbookBookings();
                        break;
                    case "Guest Payments":
                        _paymentService.PayInvoiceBeforeCheckout();
                        break;
                    case "Display All Registered Guests":
                        DisplayAllRegisteredGuests();
                        break;
                    case "Remove Guest":
                        _guestRemovalService.Execute();
                        break;
                    case "Back":
                        return;
                    default:
                        AnsiConsole.Markup("[red]Invalid option. Try again.[/]\n");
                        break;
                }
            }
        }
    }
}
