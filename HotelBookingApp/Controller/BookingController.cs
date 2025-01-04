using HotelBookingApp.Repositories;
using Spectre.Console;
using HotelBookingApp.Controllers;
using HotelBookingApp.Services.DisplayServices;
using HotelBookingApp.Services.BookingServices;
using HotelBookingApp.Services.GuestServices;
using HotelBookingApp.Interfaces;
using Microsoft.VisualBasic;

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
        //public void DisplayTables()
        //{
        //    Console.Clear();

        
        //        Console.Clear();

        //        var activeBookings = _bookingRepository.GetActiveBookings().ToList();
        //        var completedBookings = _bookingRepository.GetCompletedBookings().ToList();
        //        var removedBookings = _bookingRepository.GetRemovedBookings().ToList();

        //        if (activeBookings.Any())
        //        {
        //            _tableDisplayService.DisplayBookingTable(activeBookings, "Active Bookings:");
        //            Console.WriteLine(new string('-', 100));
        //        }
        //        else
        //        {
        //            AnsiConsole.MarkupLine("[gray]No active bookings found.[/]");
        //            Console.WriteLine(new string('-', 100));
        //        }

        //        if (completedBookings.Any())
        //        {
        //            _tableDisplayService.DisplayBookingTable(completedBookings, "Completed Bookings:", includePaymentAndStatus: true);
        //            Console.WriteLine(new string('-', 100));
        //        }
        //        else
        //        {
        //            AnsiConsole.MarkupLine("[gray]No completed bookings found.[/]");
        //            Console.WriteLine(new string('-', 100));
        //        }

        //        if (removedBookings.Any())
        //        {
        //            _tableDisplayService.DisplayBookingTable(removedBookings, "Unbooked Bookings:", includePaymentAndStatus: false);
        //            Console.WriteLine(new string('-', 100));
        //        }
        //        else
        //        {
        //            AnsiConsole.MarkupLine("[gray]No unbooked bookings found.[/]");
        //            Console.WriteLine(new string('-', 100));
        //        }

        //        _unpaidBookingService.DisplayCanceledBookingHistory();
        //        Console.WriteLine(new string('-', 100));

        //        _displayRegisteredGuestsService.DisplayAllRegisteredGuests();
        //        Console.WriteLine(new string('-', 100));
            
        //}
        //Display all active bookings
        public void Run()
        {
           // DisplayTables();

            //var activeBookings = _bookingRepository.GetActiveBookings().ToList();
            var action = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("[bold green]What would you like to do?[/]")
                        .AddChoices(new[]
                        {
                  
                    "Register Booking",
                    "Read all bookings",
                    "Update Booking",
                    "Delete Booking",
                    "Check In/Check Out",
                    "Guest Payments",
                    "Back"
                        })
                        .HighlightStyle(new Style(foreground: Color.Green))
                );
            //CRUD CREATE, READ, UPDATE, DELETE

                switch (action)
                {
                    
                case "Register Booking":
                    _guestController.RegisterBooking();
                        break;
                case "Read all bookings":
                    _tableDisplayService.DisplayBookingStatuses();
                    break;
                case "Update Booking":
                        _bookingEditService.EditBooking();
                        break;
                    case "Delete Booking":
                        _unbookBooking.UnbookBookings();
                        break;
                case "Check In/Check Out":
                    _checkInOutService.Execute();
                    break;
                case "Guest Payments":
                        _paymentService.PayInvoiceBeforeCheckout();
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

