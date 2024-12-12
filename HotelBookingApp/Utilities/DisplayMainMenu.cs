using HotelBookingApp.Entities;
using Microsoft.EntityFrameworkCore;
using System;

namespace HotelBookingApp.Utilities
{
    public class DisplayMainMenu
    {
        private readonly RoomService _roomService;
        private readonly GuestServices _guestServices;
        private readonly DisplayBookingMenu _displayBookingMenu;
        public DisplayMainMenu( RoomService roomService, GuestServices guestServices, DisplayBookingMenu displayBookingMenu)
        {
            _roomService = roomService;
            _guestServices = guestServices;
            _displayBookingMenu = displayBookingMenu;
        }

        public void Run()
        {
            while (true)
            {
                Console.Clear();

                Console.WriteLine("\nHOTEL BOOKING APP");
                Console.WriteLine("Choose an option:");
                Console.WriteLine("1. Booking");
                Console.WriteLine("2. Room");
                Console.WriteLine("3. Guest");
                Console.WriteLine("4. Exit");

                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        _displayBookingMenu.Menu();
                        break;

                    case "2":
                        _roomService.Menu();
                        break;

                    case "3":
                        _guestServices.Menu();
                        break;

                    case "4":
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
