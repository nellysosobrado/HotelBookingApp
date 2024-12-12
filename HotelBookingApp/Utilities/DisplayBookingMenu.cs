﻿using HotelBookingApp.Data;
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
        private readonly AppDbContext _appDbContext;
        private readonly BookingController _bookingController;
        public DisplayBookingMenu(AppDbContext appDbContext, BookingController bookingController)
        {
            _appDbContext = appDbContext;
            _bookingController = bookingController;
        }

        public void Menu()
        {
            Console.WriteLine("You are in DisplayBookingMenu.cs");
            string[] options = { 
            "Check in guest"
            };

            while (true)
            {
                int selectedOption = NavigateMenu(options);
                Console.Clear();

                switch(selectedOption)
                {
                    case 0:
                        _bookingController.CheckIn();
                        break;
                    default:
                        Console.WriteLine("error");
                        break;
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
