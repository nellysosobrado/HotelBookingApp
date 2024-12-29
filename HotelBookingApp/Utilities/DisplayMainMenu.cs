using HotelBookingApp.Controllers;
using HotelBookingApp.Services;
using HotelBookingApp.Services.BookingServices;
using Spectre.Console;
using System;

namespace HotelBookingApp.Utilities
{
    public class DisplayMainMenu
    {
        private readonly GuestController _guestController;
        private readonly BookingController _bookingController;
        private readonly RoomController _roomController;
        private readonly CheckInOutService _checkInOutService;
        private readonly BookingEditService _bookingEditService;
        private readonly PaymentService _paymentService;
        private readonly UnpaidBookingService _unpaidBookingService;

        public DisplayMainMenu(
            GuestController guestController,
            BookingController bookingController,
            RoomController roomController,
            CheckInOutService checkInOutService,
            BookingEditService bookingEditService,
            PaymentService paymentSerice,
            UnpaidBookingService unpaidBookingService)
        {
            _guestController = guestController;
            _bookingController = bookingController;
            _roomController = roomController;
            _checkInOutService = checkInOutService;
            _bookingEditService = bookingEditService;
            _paymentService = paymentSerice;
            _unpaidBookingService = unpaidBookingService;
        }

        public void Run()
        {
            while (true)
            {
                Console.Clear();

                var choice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("[bold yellow] Hotel Booking Application[/]")
                        .PageSize(10)
                        .HighlightStyle(new Style(Color.Green, decoration: Decoration.Bold))
                        .AddChoices(
                            "HandleUnpaidBookings",
                            "Pay",
                            "Register New Guest",
                            "Check in/Check out Guest",
                            "Edit Guest information",
                            "Display all registered guests",
                            "Rooms",
                            "Bookings",
                            "Exit")
                );

                switch (choice)
                {
                    case "HandleUnpaidBookings":
                        _unpaidBookingService.HandleUnpaidBookings();
                        //_guestController.RegisterNewGuest();
                        break;

                    case "Register New Guest":
                        _guestController.RegisterNewGuest();
                        break;

                    case "Check in/Check out Guest":
                        _checkInOutService.Execute();
                        break;

                    case "Edit Guest information":
                        _bookingEditService.EditBooking();
                        break;

                    case "Display all registered guests":
                         _bookingController.DisplayAllRegisteredGuests();
                       
                        break;

                    case "Rooms":
                        _roomController.ViewAllRooms();
                        break;
                    case "Bookings":
                        _bookingController.BookingManagement();
                        break;
                    case "Pay":
                        _paymentService.PayInvoiceBeforeCheckout();
                        break;

                    case "Exit":
                        AnsiConsole.MarkupLine("[bold green]Thank you for using the Hotel Booking App. Goodbye![/]");
                        return;

                    default:
                        AnsiConsole.MarkupLine("[bold red]Invalid choice. Please try again.[/]");
                        break;
                }
            }
        }
    }
}
