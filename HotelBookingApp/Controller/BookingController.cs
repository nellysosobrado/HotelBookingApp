using HotelBookingApp.Repositories;
using Spectre.Console;
using HotelBookingApp.Controllers;
using HotelBookingApp.Services.DisplayServices;
using HotelBookingApp.Services.BookingServices;
using HotelBookingApp.Services.GuestServices;
using HotelBookingApp.Interfaces;

namespace HotelBookingApp
{
    public class BookingController : IMenuDisplay
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
        private readonly GuestRepository _guestRepository;
        private readonly DisplayRegisteredGuestsService _displayRegisteredGuestsService;

        public BookingController(
            BookingRepository bookingRepository,
            GuestController guestController,
            TableDisplayService tableDisplayService,
            CheckInOutService checkInOutService,
            BookingEditService bookingEditService,
            UnbookBooking unbookBooking,
            PaymentService paymentService,
            GuestRemovalService guestRemovalService,
            UnpaidBookingService UnpaidBookingService,
            GuestRepository GuestRepository,
            DisplayRegisteredGuestsService displayRegisteredGuestsService)
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
            _guestRepository = GuestRepository;
            _displayRegisteredGuestsService = displayRegisteredGuestsService;
        }
        public void DisplayTables()
        {
            Console.Clear();

        
                Console.Clear();

                var activeBookings = _bookingRepository.GetActiveBookings().ToList();
                var completedBookings = _bookingRepository.GetCompletedBookings().ToList();
                var removedBookings = _bookingRepository.GetRemovedBookings().ToList();

                if (activeBookings.Any())
                {
                    _tableDisplayService.DisplayBookingTable(activeBookings, "Active Bookings:");
                    Console.WriteLine(new string('-', 100));
                }
                else
                {
                    AnsiConsole.MarkupLine("[gray]No active bookings found.[/]");
                    Console.WriteLine(new string('-', 100));
                }

                if (completedBookings.Any())
                {
                    _tableDisplayService.DisplayBookingTable(completedBookings, "Completed Bookings:", includePaymentAndStatus: true);
                    Console.WriteLine(new string('-', 100));
                }
                else
                {
                    AnsiConsole.MarkupLine("[gray]No completed bookings found.[/]");
                    Console.WriteLine(new string('-', 100));
                }

                if (removedBookings.Any())
                {
                    _tableDisplayService.DisplayBookingTable(removedBookings, "Unbooked Bookings:", includePaymentAndStatus: false);
                    Console.WriteLine(new string('-', 100));
                }
                else
                {
                    AnsiConsole.MarkupLine("[gray]No unbooked bookings found.[/]");
                    Console.WriteLine(new string('-', 100));
                }

                _unpaidBookingService.DisplayCanceledBookingHistory();
                Console.WriteLine(new string('-', 100));

                _displayRegisteredGuestsService.DisplayAllRegisteredGuests();
                Console.WriteLine(new string('-', 100));
            
        }

        public void Run()
        {
            DisplayTables();
            

                var action = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("[bold green]What would you like to do?[/]")
                        .AddChoices(new[]
                        {
                    "Check In/Check Out",
                    "Register New Guest/Booking",
                    "Edit Booking",
                    "Unbook Booking",
                    "Guest Payments",
                    "Remove Guest",
                    "Back"
                        })
                        .HighlightStyle(new Style(foreground: Color.Green))
                );

                switch (action)
                {
                    case "Check In/Check Out":
                        _checkInOutService.Execute();
                        break;
                    case "Register New Guest/Booking":
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

