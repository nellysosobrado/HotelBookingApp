using HotelBookingApp.Data;
using HotelBookingApp.Entities;
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
    public class GuestController
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
        public void GuestOptions()
        {
            var action = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[italic yellow]Guests[/]")
                    .AddChoices(new[]
                    {
                    "Create new guest",
                    "Read all guestst",
                    "Update Guest Information",
                    "Remove Guest",
                    "Back"
                    })
                    .HighlightStyle(new Style(foreground: Color.Green))
            );

            switch (action)
            {
                case "Create new guest":
                    _registerGuest.NewGuest();
                    break;
                case "Read all guestst":
                    _displayRegisteredGuestsService.DisplayGuests();
                    break;
                case "Update Guest Information":
                    _updateGuestInformation.Run();
                    break;
                case "Remove Guest":
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
