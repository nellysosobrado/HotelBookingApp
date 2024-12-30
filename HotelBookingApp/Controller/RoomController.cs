using HotelBookingApp.Entities;
using HotelBookingApp.Repositories;
using HotelBookingApp.Services.DisplayServices;
using HotelBookingApp.Services.RoomServices;
using Spectre.Console;
using System;
using System.Linq;

namespace HotelBookingApp.Controllers
{
    public class RoomController
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
        public void RegisterANewRoom()
        {
            _registerRoomService.Execute();

        }
        public void EditRoom()
        {
            _editRoomService.Execute();
        }
        public void FindRoomByAvailableDate()
        {
            _findRoomByDate.Execute();
        }
        public void FindRoomByTotalPeople()
        {
            _findRoomByTotalPeople.Execute();
        }
        public void DeleteRoom()
        {
            _deleteRoomService.Execute();
        }
        public void ViewAllRooms()
        {
            while (true)
            {
                Console.Clear();

                var allRooms = _roomRepository.GetAllRooms(includeDeleted: true);
                var activeRooms = allRooms.Where(r => !r.IsDeleted).ToList();
                var bookedActiveRooms = activeRooms
                    .Where(r => r.Bookings != null && r.Bookings.Any(b => !b.IsCanceled))
                    .ToList();
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
                        .AddChoices(new[] { "Register a new room", "Edit a Room", "Delete a Room", "Find Available Room By Date", "Find Available Room By Total People", "Go Back" })
                        .HighlightStyle(new Style(foreground: Color.Green)));

                switch (action)
                {
                    case "Register a new room":
                        RegisterANewRoom();
                        break;

                    case "Edit a Room":
                        EditRoom();
                        break;

                    case "Delete a Room":
                        DeleteRoom();
                        break;

                    case "Find Available Room By Date":
                        FindRoomByAvailableDate();
                        break;

                    case "Find Available Room By Total People":
                        FindRoomByTotalPeople();
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
