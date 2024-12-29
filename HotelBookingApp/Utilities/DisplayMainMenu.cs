using HotelBookingApp.Controllers;
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

        public DisplayMainMenu(
            GuestController guestController,
            BookingController bookingController,
            RoomController roomController,
            CheckInOutService checkInOutService)
        {
            _guestController = guestController;
            _bookingController = bookingController;
            _roomController = roomController;
            _checkInOutService = checkInOutService;
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
                    case "Register New Guest":
                        _guestController.RegisterNewGuest();
                        break;

                    case "Check in/Check out Guest":
                        _checkInOutService.Execute();
                        break;

                    case "Edit Guest information":
                        _bookingController.EditBooking();
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
                        _bookingController.PayInvoiceBeforeCheckout();
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
