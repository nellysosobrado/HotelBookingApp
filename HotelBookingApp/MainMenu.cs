﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System;

namespace HotelBookingApp
{
    public class MainMenu
    {
        private readonly BookingManager _bookingManager;
        private readonly RegisterNewBooking _registerNewBooking;

        public MainMenu(BookingManager bookingManager, RegisterNewBooking registerNewBooking)
        {
            _bookingManager = bookingManager;
            _registerNewBooking = registerNewBooking;
        }

        public void Run()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("HOTEL BOOKING APP");
                Console.WriteLine("Choose an option:");
                Console.WriteLine("1. Check In Guest");
                Console.WriteLine("2. Check Out Guest");
                Console.WriteLine("3. View Booking Details");
                Console.WriteLine("4. Register New Booking");
                Console.WriteLine("5. Exit");

                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        Console.WriteLine("Enter Booking ID to check in:");
                        if (int.TryParse(Console.ReadLine(), out int checkInId))
                        {
                            _bookingManager.CheckInGuest(checkInId);
                        }
                        else
                        {
                            Console.WriteLine("Invalid Booking ID.");
                        }
                        break;

                    case "2":
                        Console.WriteLine("Enter Booking ID to check out:");
                        if (int.TryParse(Console.ReadLine(), out int checkOutId))
                        {
                            _bookingManager.CheckOutGuest(checkOutId);
                        }
                        else
                        {
                            Console.WriteLine("Invalid Booking ID.");
                        }
                        break;

                    case "3":
                        Console.WriteLine("Enter Booking ID to view details:");
                        if (int.TryParse(Console.ReadLine(), out int viewId))
                        {
                            _bookingManager.ViewBookingDetails(viewId);
                        }
                        else
                        {
                            Console.WriteLine("Invalid Booking ID.");
                        }
                        break;

                    case "4":
                        _registerNewBooking.Execute();
                        break;

                    case "5":
                        Console.WriteLine("Exiting program...");
                        return;

                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }

                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
            }
        }
    }
}
