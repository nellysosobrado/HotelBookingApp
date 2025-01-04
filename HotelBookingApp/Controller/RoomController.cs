using HotelBookingApp.Entities;
using HotelBookingApp.Interfaces;
using HotelBookingApp.Repositories;
using HotelBookingApp.Services.DisplayServices;
using HotelBookingApp.Services.RoomServices;
using Spectre.Console;
using System;
using System.Linq;

namespace HotelBookingApp.Controllers
{
    public class RoomController : IMenuDisplay
    {
        private readonly RoomRepository _roomRepository;
        private readonly FindRoomByDate _findRoomByDate;
        private readonly FindRoomByTotalPeople _findRoomByTotalPeople;
        private readonly DeleteRoomService _deleteRoomService;
        private readonly EditRoomService _editRoomService;
        private readonly RegisterRoomService _registerRoomService;
        private readonly TableDisplayService _tableDisplayService;
        private readonly BookingRepository _bookingRepository;

        public RoomController(RoomRepository roomRepository,
            FindRoomByDate findRoomByDate,
            FindRoomByTotalPeople findRoomByTotalPeople,
            DeleteRoomService deleteRoomService,
            EditRoomService editRoomService,
            RegisterRoomService registerRoomService,
            TableDisplayService tableDisplayService,
            BookingRepository bookingRepository)
        {
            _roomRepository = roomRepository;
            _findRoomByDate = findRoomByDate;
            _findRoomByTotalPeople = findRoomByTotalPeople;
            _deleteRoomService = deleteRoomService;
            _editRoomService = editRoomService;
            _registerRoomService = registerRoomService;
            _tableDisplayService = tableDisplayService;
            _bookingRepository = bookingRepository;
        }
        public void MenuOptions()
        {
            while (true)
            {
                Console.Clear();

                var action = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("[italic yellow] Rooms[/]")
                        .AddChoices(new[]
                        {
                    "Register a new room",
                    "Display rooms",
                    "Update a room",
                    "Delete a Room",
                    "Find Available Room By Date",
                    "Find Available Room By Total People",
                    "Back"
                        })
                        .HighlightStyle(new Style(foreground: Color.Green)));

                switch (action)
                {
                    case "Register a new room":
                        _registerRoomService.Execute();
                        break;

                    case "Display rooms":
                        _tableDisplayService.DisplayRooms();
                        break;

                    case "Update a room":
                        _editRoomService.Execute();
                        break;

                    case "Delete a Room":
                        _deleteRoomService.Execute();
                        break;

                    case "Find Available Room By Date":
                        _findRoomByDate.Execute();
                        break;

                    case "Find Available Room By Total People":
                        _findRoomByTotalPeople.Execute();
                        break;

                    case "Back":
                        return;

                    default:
                        AnsiConsole.Markup("[red]Invalid option selected.[/]\n");
                        break;
                }
            }
        }
    }
}
