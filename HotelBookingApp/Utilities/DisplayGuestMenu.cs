using HotelBookingApp.Data;
using HotelBookingApp.Controllers;
using System;

namespace HotelBookingApp.Utilities
{
    public class DisplayGuestMenu
    {
        private readonly GuestController _guestController;
        private readonly BookingController _bookingController;

        public DisplayGuestMenu( GuestController guestController, BookingController bookingController)
        {
            _guestController = guestController;
            _bookingController = bookingController;
        }

        public void Menu()
        {
            string[] options = {
                "Display guest information",
                "Update Guest Information",
                "Main Menu"
            };

            while (true)
            {
                int selectedOption = NavigateMenu(options);
                Console.Clear();

                switch (selectedOption)
                {
                    case 0:
                        _bookingController.DisplayAllGuestInfo();

                        break;
                    case 1:
                        _guestController.UpdateGuestInformation();
                        break;

                    case 2:
                        Console.WriteLine("Returning to Main Menu...");
                        return;
                    default:
                        Console.WriteLine("Error: Invalid selection.");
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
