using HotelBookingApp.Data;
using HotelBookingApp.Entities;
using HotelBookingApp.Interfaces;
using HotelBookingApp.Repositories;
using HotelBookingApp.Services.BookingServices;
using HotelBookingApp.Services.GuestServices;
using HotelBookingApp.Services.RoomServices;
using Microsoft.EntityFrameworkCore;
using Spectre.Console;
using System;
using System.Linq;

namespace HotelBookingApp.Controllers
{
    public class GuestController : IMenuDisplay
    {

        private readonly DisplayRegisteredGuestsService _displayRegisteredGuestsService;
        private readonly GuestRemovalService _guestRemovalService;
        private readonly UpdateGuestInformation _updateGuestInformation;
        private readonly RegisterGuest _registerGuest;

        public GuestController(AppDbContext context,
            DisplayRegisteredGuestsService displayRegisteredGuestsService,
            GuestRemovalService guestRemovalService,
            UpdateGuestInformation updateGuestInformation,
            RegisterGuest registerGuest
            )
        {
            _displayRegisteredGuestsService = displayRegisteredGuestsService;
            _guestRemovalService = guestRemovalService;
            _updateGuestInformation = updateGuestInformation;
            _registerGuest = registerGuest;
        }
        public void MenuOptions()
        {
            var action = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[italic yellow]Guests[/]")
                    .WrapAround(true)
                    .AddChoices(new[]
                    {
                    "Register a new guest",
                    "Display guests",
                    "Update guest information",
                    "Remove guest",
                    "Back"
                    })
                    .HighlightStyle(new Style(foreground: Color.Green))
            );

            switch (action)
            {
                case "Register a new guest":
                    _registerGuest.NewGuest();
                    break;
                case "Display guests":
                    _displayRegisteredGuestsService.DisplayGuests();
                    break;
                case "Update guest information":
                    _updateGuestInformation.Run();
                    break;
                case "Remove guest":
                    _guestRemovalService.Execute();
                    break;
                case "Back":
                    return;
                default:
                    AnsiConsole.Markup("[red]Invalid option. Try again.[/]\n");
                    break;
            }
        }
    }
}
