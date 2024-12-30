using HotelBookingApp.Controllers;
using HotelBookingApp.Services.BookingServices;
using HotelBookingApp.Services.GuestServices;
using Spectre.Console;
using System;

namespace HotelBookingApp.Utilities
{
    public class DisplayMainMenu
    {
        private readonly BookingController _bookingController;
        private readonly RoomController _roomController;

        public DisplayMainMenu(
            GuestController guestController,
            BookingController bookingController,
            RoomController roomController,
            CheckInOutService checkInOutService,
            BookingEditService bookingEditService,
            PaymentService paymentSerice,
            UnpaidBookingService unpaidBookingService)
        {
            _bookingController = bookingController;
            _roomController = roomController;
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
                            "Exit")
                );
                switch (choice)
                {
                    case "Rooms":
                        _roomController.ViewAllRooms();
                        break;
                    case "Bookings":
                        _bookingController.BookingOptions();
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
