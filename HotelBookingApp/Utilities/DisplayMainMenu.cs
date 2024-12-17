﻿using HotelBookingApp.Entities;
using HotelBookingApp.Utilities;
using Microsoft.EntityFrameworkCore;
using System;

namespace HotelBookingApp.Utilities
{
    public class DisplayMainMenu
    {
        private readonly DisplayRoomMenu _displayRoomMenu;
        private readonly DisplayGuestMenu _displayGuestMenu;
        private readonly DisplayBookingMenu _displayBookingMenu;
        public DisplayMainMenu( DisplayRoomMenu Menu, DisplayBookingMenu displayBookingMenu, DisplayGuestMenu displayGuestMenu)
        {
            _displayRoomMenu = Menu;
            _displayBookingMenu = displayBookingMenu;
            _displayGuestMenu = displayGuestMenu;
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
                        _displayRoomMenu.Menu();
                        break;

                    case "3":
                        _displayGuestMenu.Menu();
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
