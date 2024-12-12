using HotelBookingApp.Entities;
using HotelBookingApp.Services.BookingService;
using Microsoft.EntityFrameworkCore;
using System;

namespace HotelBookingApp.Utilities
{
    public class DisplayMainMenu
    {
        private readonly BookingService _bookingService;
        private readonly RoomService _roomService;
        private readonly GuestServices _guestServices;
        private readonly DisplayBookingMenu _displayBookingMenu;
        public DisplayMainMenu(BookingService bookingManager, RoomService roomService, GuestServices guestServices, DisplayBookingMenu displayBookingMenu)
        {
            _bookingService = bookingManager;
            _roomService = roomService;
            _guestServices = guestServices;
            _displayBookingMenu = displayBookingMenu;
        }

        public void Run()
        {
            while (true)
            {
                Console.Clear();

                DisplayGuestTable();

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

        private void DisplayGuestTable()
        {
            Console.WriteLine("=== VIEW ALL GUESTS ===");

            var bookings = _bookingService.GetGuestsWithCheckInStatus();

            if (!bookings.Any())
            {
                Console.WriteLine("No guests found.");
                return;
            }

            Console.WriteLine(new string('-', 60));
            Console.WriteLine($"{"BookingId",-15}{"Name",-30}{"Checked In",-15}");
            Console.WriteLine(new string('-', 60));

            foreach (var booking in bookings)
            {
                string checkedInStatus = booking.IsCheckedIn ? "Yes" : "No";
                Console.WriteLine($"{booking.BookingId,-15}{booking.Guest.FirstName + " " + booking.Guest.LastName,-30}{checkedInStatus,-15}");
            }

            Console.WriteLine(new string('-', 60));
        }






    }
}
