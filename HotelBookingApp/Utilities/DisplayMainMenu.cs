using HotelBookingApp.Controllers;
using HotelBookingApp.Utilities;
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

        public DisplayMainMenu(DisplayRoomMenu roomMenu, DisplayBookingMenu bookingMenu, DisplayGuestMenu guestMenu, GuestController guestController, BookingController bookingController)
        {
            _displayRoomMenu = roomMenu;
            _displayBookingMenu = bookingMenu;
            _displayGuestMenu = guestMenu;
            _guestController = guestController;
            _bookingController = bookingController;
        }

        public void Run()
        {
            while (true)
            {
                Console.Clear();

                Console.WriteLine("\nHOTEL BOOKING APP - MAIN MENU");
                Console.WriteLine(new string('-', 40));
                Console.WriteLine("1. Register New Guest");
                Console.WriteLine("2. Check In Guest");
                Console.WriteLine("3. Check Out Guest");
                Console.WriteLine("4. Pay Invoice Before Checkout");
                Console.WriteLine("5. Display all guests");
                Console.WriteLine(new string('-', 40));

                Console.WriteLine("6. Rooms");
                Console.WriteLine("7. Exit");
                Console.WriteLine("8. Guest");
                Console.WriteLine("9. Exit");
                Console.WriteLine(new string('-', 40));

                Console.Write("Choose an option: ");
                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        _guestController.RegisterNewGuest();
                        break;

                    case "2":
                        _bookingController.CheckIn();
                        break;

                    case "3":
                        _bookingController.CheckOut();
                        break;

                    case "4":
                        _bookingController.PayInvoiceBeforeCheckout();
                        break;
                    case "5":
                        _bookingController.DisplayAllGuestInfo();
                        break;
                    case "6":
                        _displayRoomMenu.Menu();
                        break;
                    case "7":
                        _displayBookingMenu.Menu();
                        break;
                    case "8":
                        _displayGuestMenu.Menu();
                        break;

                    
                    case "9":
                        Console.WriteLine("Exiting Program...");
                        return;

                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        Console.WriteLine("Press any key to continue...");
                        Console.ReadKey();
                        break;
                }
            }
        }
    }
}
