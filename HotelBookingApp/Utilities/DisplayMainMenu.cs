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

                // Visa rubrik
                AnsiConsole.Write(new Panel("[bold yellow]HOTEL BOOKING APP - MAIN MENU[/]").Expand());

                // Visa huvudmeny med val
                var choice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("[bold blue]Select an option:[/]")
                        .PageSize(10)
                        .AddChoices(
                            "1. Register New Guest",
                            "2. Check In Guest",
                            "3. Check Out Guest",
                            "4. Pay Invoice Before Checkout",
                            "5. Display Guests",
                            "6. Display Rooms",
                            "7. Modify Rooms",
                            "8. Modify Bookings",
                            "9. Modify Guest",
                            "10. Exit"
                        ));

                // Hantera användarens val
                switch (choice)
                {
                    case "1. Register New Guest":
                        _guestController.RegisterNewGuest();
                        break;

                    case "2. Check In Guest":
                        _bookingController.CheckIn();
                        break;

                    case "3. Check Out Guest":
                        _bookingController.CheckOut();
                        break;

                    case "4. Pay Invoice Before Checkout":
                        _bookingController.PayInvoiceBeforeCheckout();
                        break;

                    case "5. Display Guests":
                        _bookingController.DisplayAllGuestInfo();
                        break;

                    case "6. Display Rooms":
                        _roomController.ViewAllRooms();
                        break;

                    case "7. Modify Rooms":
                        _displayRoomMenu.Menu();
                        break;

                    case "8. Modify Bookings":
                        _displayBookingMenu.Menu();
                        break;

                    case "9. Modify Guest":
                        _displayGuestMenu.Menu();
                        break;

                    case "10. Exit":
                        AnsiConsole.Markup("[bold green]Exiting program...[/]\n");
                        return;

                    default:
                        AnsiConsole.Markup("[bold red]Invalid choice. Please try again.[/]\n");
                        break;
                }

                // Vänta på att användaren trycker på en knapp innan den återgår till huvudmenyn
                AnsiConsole.Markup("\n[bold yellow]Press any key to return to the main menu...[/]");
                Console.ReadKey();
            }
        }
    }
}
