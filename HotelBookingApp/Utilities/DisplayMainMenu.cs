using HotelBookingApp.Controllers;
using Spectre.Console;
using System;

namespace HotelBookingApp.Utilities
{
    public class DisplayMainMenu
    {
        private readonly DisplayRoomMenu _displayRoomMenu;
        private readonly DisplayGuestMenu _displayGuestMenu;
        private readonly DisplayBookingMenu _displayBookingMenu;
        private readonly GuestController _guestController;
        private readonly BookingController _bookingController;
        private readonly RoomController _roomController;

        public DisplayMainMenu(
            DisplayRoomMenu roomMenu,
            DisplayBookingMenu bookingMenu,
            DisplayGuestMenu guestMenu,
            GuestController guestController,
            BookingController bookingController,
            RoomController roomController)
        {
            _displayRoomMenu = roomMenu;
            _displayBookingMenu = bookingMenu;
            _displayGuestMenu = guestMenu;
            _guestController = guestController;
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
                        .Title("[bold yellow] Hotel Booking Application[/]")
                        .PageSize(10)
                        .HighlightStyle(new Style(Color.Green, decoration: Decoration.Bold))
                        .AddChoices(
                            "Register New Guest",
                            "Check in/Check out Guest",
                            //"Payment",
                            "Guests",
                            "Rooms",
                            "Bookings",
                            //"Settings",
                            "Exit")
                );

                switch (choice)
                {
                    case "Register New Guest":
                        _guestController.RegisterNewGuest();
                        break;

                    case "Check in/Check out Guest":
                        _bookingController.CheckInOrCheckOut();
                        break;

                    //case "Payment":
                    //    _bookingController.PayInvoiceBeforeCheckout();
                    //    break;

                    case "Guests":
                         _bookingController.DisplayAllGuestInfo();
                       
                        break;

                    case "Rooms":
                        _roomController.ViewAllRooms();
                        break;
                    case "Bookings":
                        _bookingController.DisplayAllBookings();
                        break;

                    //case "Settings":
                    //    Settings();
                    //    break;

                    case "Exit":
                        AnsiConsole.MarkupLine("[bold green]Thank you for using the Hotel Booking App. Goodbye![/]");
                        return;

                    default:
                        AnsiConsole.MarkupLine("[bold red]Invalid choice. Please try again.[/]");
                        break;
                }
            }
        }

        //public void Settings()
        //{
        //    while (true)
        //    {
        //        Console.Clear();

        //        var choice = AnsiConsole.Prompt(
        //            new SelectionPrompt<string>()
        //                .Title("[bold yellow]Select a setting to modify:[/]")
        //                .PageSize(5)
        //                .HighlightStyle(new Style(Color.Green, decoration: Decoration.Bold))
        //                .AddChoices(
        //                    "Modify Bookings",
        //                    "Modify Guest",
        //                    "Back to Main Menu"
        //                ));

        //        switch (choice)
        //        {

        //            case "Modify Bookings":
        //                _displayBookingMenu.Menu();
        //                break;

        //            case "Modify Guest":
        //                _displayGuestMenu.Menu();
        //                break;

        //            case "Back to Main Menu":
        //                return;

        //            default:
        //                AnsiConsole.MarkupLine("[bold red]Invalid choice. Please try again.[/]");
        //                break;
        //        }
        //    }
        //}
    }
}
