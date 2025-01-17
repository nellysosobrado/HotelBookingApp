﻿using HotelBookingApp.Controllers;
using HotelBookingApp.Interfaces;
using HotelBookingApp.Services.BookingServices;
using HotelBookingApp.Services.GuestServices;
using Spectre.Console;
using System;

namespace HotelBookingApp.Utilities
{
    public class MainMenu : IMenuDisplay
    {
        private readonly BookingController _bookingController;
        private readonly RoomController _roomController;
        private readonly GuestController _guestController;

        public MainMenu(
            GuestController guestController,
            BookingController bookingController,
            RoomController roomController,
            CheckInOutService checkInOutService,
            BookingEditService bookingEditService,
            PaymentService paymentSerice,
            UnpaidBookingService unpaidBookingService
            )
        {
            _bookingController = bookingController;
            _roomController = roomController;
            _guestController = guestController;
        }

        public void MenuOptions()
        {
            while (true)
            {
                Console.Clear();

                var choice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("[italic yellow] Hotel Booking Application[/]")
                        .PageSize(10)  
                        .HighlightStyle(new Style(Color.Green, decoration: Decoration.Bold))
                        .AddChoices("Rooms", "Bookings", "Guests", "Exit")  
                        .WrapAround(true)  
                );

                switch (choice)
                {
                    case "Rooms":
                        _roomController.MenuOptions();
                        break;
                    case "Bookings":
                        _bookingController.MenuOptions();
                        break;
                    case "Guests":
                        _guestController.MenuOptions();
                        break;
                    case "Exit":
                        AnsiConsole.MarkupLine("[bold green]Thank you for using the Hotel Booking App. Goodbye![/]");
                        return;
                    default:
                        AnsiConsole.MarkupLine("[bold red]Invalid choice. Please try again.[/]");
                        break;
                }
            }
        }

    }
}
