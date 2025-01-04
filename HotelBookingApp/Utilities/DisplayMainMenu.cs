using HotelBookingApp.Controllers;
using HotelBookingApp.Interfaces;
using HotelBookingApp.Services.BookingServices;
using HotelBookingApp.Services.GuestServices;
using Spectre.Console;
using System;

namespace HotelBookingApp.Utilities
{
    public class DisplayMainMenu : IMenuDisplay
    {
        private readonly BookingController _bookingController;
        private readonly RoomController _roomController;
        private readonly GuestController _guestController;

        public DisplayMainMenu(
            GuestController guestController,
            BookingController bookingController,
            RoomController roomController,
            CheckInOutService checkInOutService,
            BookingEditService bookingEditService,
            PaymentService paymentSerice,
            UnpaidBookingService unpaidBookingService
            )
        {
            _bookingController = bookingController;
            _roomController = roomController;
            _guestController = guestController;
        }

        public void Run()
        {
            while (true)
            {
                Console.Clear();

                var choice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("[italic yellow] Hotel Booking Application[/]")
                        .PageSize(10)
                        .HighlightStyle(new Style(Color.Green, decoration: Decoration.Bold))
                        .AddChoices(
                            "Rooms",
                            "Bookings",
                            "Guests",
                            "Exit")
                );
                switch (choice)
                {
                    case "Rooms":
                        _roomController.Run();
                        break;
                    case "Bookings":
                        _bookingController.Run();
                        break;
                    case "Guests":
                        _guestController.Run();
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
