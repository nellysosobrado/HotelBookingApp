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
        public void Run()
        {
            while (true)
            {
                Console.Clear();

                //var allRooms = _roomRepository.GetAllRooms(includeDeleted: true);
                //var activeRooms = allRooms.Where(r => !r.IsDeleted).ToList();
                //var bookedActiveRooms = _roomRepository.GetRoomsWithActiveBookings();
                //var removedBookings = _booking.GetRemovedBookings().ToList();
                //var removedRooms = allRooms.Where(r => r.IsDeleted).ToList();

                //if (!allRooms.Any())
                //{
                //    AnsiConsole.Markup("[red]No rooms found in the database.[/]\n");
                //}
                //else
                //{
                //    //_tableDisplayService.DisplayRooms(activeRooms, "Overview of registered rooms", includeDeleted: false);
                //    //Console.WriteLine(new string('-', 125));

                //    //_tableDisplayService.DisplayBookedRooms(bookedActiveRooms, "Booked Active Rooms");
                //    //Console.WriteLine(new string('-', 125));

                //    //_tableDisplayService.DisplayBookingTable(removedBookings, "Unbooked Bookings:", includePaymentAndStatus: false);
                //    //Console.WriteLine(new string('-', 100));

                //    //_tableDisplayService.DisplayRooms(removedRooms, "Removed Rooms", includeDeleted: true);
                //    //Console.WriteLine(new string('-', 125));
                //}

                var action = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("[italic yellow] Rooms[/]")
                        .AddChoices(new[]
                        {
                    "Register a new room",
                    "Read all rooms",
                    "Update a room",
                    "Delete a Room",
                    "Find Available Room By Date",
                    "Find Available Room By Total People",
                    "Back"
                        })
                        .HighlightStyle(new Style(foreground: Color.Green)));

                switch (action)
                {
                    //CRUD CREATE, READ, UPDATE, DELETE
                    case "Register a new room":
                        _registerRoomService.Execute();
                        break;

                    case "Read all rooms":
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
