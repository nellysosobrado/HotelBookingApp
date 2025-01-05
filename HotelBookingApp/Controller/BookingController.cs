using HotelBookingApp.Repositories;
using Spectre.Console;
using HotelBookingApp.Controllers;
using HotelBookingApp.Services.DisplayServices;
using HotelBookingApp.Services.BookingServices;
using HotelBookingApp.Services.GuestServices;
using HotelBookingApp.Interfaces;

namespace HotelBookingApp;

public class BookingController : IMenuDisplay
{
    private readonly GuestController _guestController;
    private readonly TableDisplayService _tableDisplayService;
    private readonly CheckInOutService _checkInOutService;
    private readonly BookingEditService _bookingEditService;
    private readonly UnbookBooking _unbookBooking;
    private readonly PaymentService _paymentService;
    private readonly GuestBookings _guestBookings;

    public BookingController(
        GuestController guestController,
        TableDisplayService tableDisplayService,
        CheckInOutService checkInOutService,
        BookingEditService bookingEditService,
        UnbookBooking unbookBooking,
        PaymentService paymentService,
        GuestBookings guestBookings
        )
    {
        _guestController = guestController;
        _tableDisplayService = tableDisplayService;
        _checkInOutService = checkInOutService;
        _bookingEditService = bookingEditService;
        _unbookBooking = unbookBooking;
        _paymentService = paymentService;
        _guestBookings = guestBookings;
    }
    public void MenuOptions()
    {
        var action = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[italic yellow]Bookings[/]")
                    .WrapAround(true)
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
            switch (action)
            {
                
            case "Register Booking":
                _guestBookings.RegisterBooking();
                    break;
            case "Read all bookings":
                _tableDisplayService.DisplayBookings();
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


