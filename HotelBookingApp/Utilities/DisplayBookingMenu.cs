using HotelBookingApp.Data;
using HotelBookingApp.Interfaces.InterfaceBooking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBookingApp.Utilities
{
    public class DisplayBookingMenu
    {
        private readonly BookingController _bookingController;
        public DisplayBookingMenu(BookingController bookingController)
        {
            _bookingController = bookingController;
        }

        public void Menu()
        {
            Console.WriteLine("Booking Menu");
            string[] options = { 
                "Display all bookings", 
                "edit booking", 
                "Unpaid bookings that havent been paid after 10 days",
                "View canceled bookings history",
                "main menu"
            };

            while (true)
            {
                int selectedOption = NavigateMenu(options);
                Console.Clear();

                switch(selectedOption)
                {
                    case 0:
                        _bookingController.DisplayAllGuestInfo();
                        break;
                    case 1:
                        _bookingController.EditBooking();
                        break;
                    case 2:
                        _bookingController.DisplayExpiredBookings();
                        break;
                    case 3:
                        _bookingController.DisplayCanceledBookings();
                        break;

                    default:
                        Console.WriteLine("error");
                        return;
                }
            }
        }
        public int NavigateMenu(string[] options) 
        {
            int selectedOption = 0;

            while (true)
            {
                Console.Clear();
                Console.WriteLine("BookingService.cs");

                for (int i = 0; i < options.Length; i++)
                {
                    if (i == selectedOption)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"> {options[i]}");
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.WriteLine($"  {options[i]}");
                    }
                }

                ConsoleKey key = Console.ReadKey(true).Key;
                switch (key)
                {
                    case ConsoleKey.UpArrow:
                        selectedOption = (selectedOption - 1 + options.Length) % options.Length;
                        break;
                    case ConsoleKey.DownArrow:
                        selectedOption = (selectedOption + 1) % options.Length;
                        break;
                    case ConsoleKey.Enter:
                        return selectedOption;
                }
            }
        }
    }
}
