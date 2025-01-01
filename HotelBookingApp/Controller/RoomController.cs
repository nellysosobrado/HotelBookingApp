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

        public RoomController(RoomRepository roomRepository,
            FindRoomByDate findRoomByDate,
            FindRoomByTotalPeople findRoomByTotalPeople,
            DeleteRoomService deleteRoomService,
            EditRoomService editRoomService,
            RegisterRoomService registerRoomService,
            TableDisplayService tableDisplayService)
        {
            _roomRepository = roomRepository;
            _findRoomByDate = findRoomByDate;
            _findRoomByTotalPeople = findRoomByTotalPeople;
            _deleteRoomService = deleteRoomService;
            _editRoomService = editRoomService;
            _registerRoomService = registerRoomService;
            _tableDisplayService = tableDisplayService;
        }
        public void Run()
        {
            while (true)
            {
                Console.Clear();

                var allRooms = _roomRepository.GetAllRooms(includeDeleted: true);
                var activeRooms = allRooms.Where(r => !r.IsDeleted).ToList();
                var bookedActiveRooms = _roomRepository.GetRoomsWithActiveBookings(); 
                var removedRooms = allRooms.Where(r => r.IsDeleted).ToList();

                if (!allRooms.Any())
                {
                    AnsiConsole.Markup("[red]No rooms found in the database.[/]\n");
                }
                else
                {
                    _tableDisplayService.DisplayRooms(activeRooms, "Overview of registered rooms", includeDeleted: false);
                    Console.WriteLine(new string('-', 125));

                    _tableDisplayService.DisplayBookedRooms(bookedActiveRooms, "Booked Active Rooms");
                    Console.WriteLine(new string('-', 125));

                    _tableDisplayService.DisplayRooms(removedRooms, "Removed Rooms", includeDeleted: true);
                    Console.WriteLine(new string('-', 125));
                }

                var action = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .AddChoices(new[]
                        {
                    "Register a new room",
                    "Edit a Room",
                    "Delete a Room",
                    "Find Available Room By Date",
                    "Find Available Room By Total People",
                    "Go Back"
                        })
                        .HighlightStyle(new Style(foreground: Color.Green)));

                switch (action)
                {
                    case "Register a new room":
                        _registerRoomService.Execute();
                        break;

                    case "Edit a Room":
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

                    case "Go Back":
                        return;

                    default:
                        AnsiConsole.Markup("[red]Invalid option selected.[/]\n");
                        break;
                }
            }
        }
    }
}
