using HotelBookingApp.Entities;
using HotelBookingApp.Repositories;
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

        public RoomController(RoomRepository roomRepository, FindRoomByDate findRoomByDate, FindRoomByTotalPeople findRoomByTotalPeople, DeleteRoomService deleteRoomService, EditRoomService editRoomService, RegisterRoomService registerRoomService)
        {
            _roomRepository = roomRepository;
            _findRoomByDate = findRoomByDate;
            _findRoomByTotalPeople = findRoomByTotalPeople;
            _deleteRoomService = deleteRoomService;
            _editRoomService = editRoomService;
            _registerRoomService = registerRoomService;
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
        private void DisplayBookedActiveRooms(IEnumerable<Room> bookedRooms, string title)
        {
            if (bookedRooms == null || !bookedRooms.Any())
            {
                AnsiConsole.Markup($"[red]No {title} found.[/]\n");
                return;
            }

            var table = new Table()
                .Border(TableBorder.Rounded)
                .AddColumn("[bold]Room ID[/]")
                .AddColumn("[bold]Booked By[/]")
                .AddColumn("[bold]Start Date[/]")
                .AddColumn("[bold]End Date[/]");

            foreach (var room in bookedRooms)
            {
                foreach (var booking in room.Bookings.Where(b => !b.IsCanceled))
                {
                    table.AddRow(
                        room.RoomId.ToString(),
                        $"{booking.Guest?.FirstName ?? "Unknown"} {booking.Guest?.LastName ?? "Unknown"}",
                        booking.CheckInDate?.ToString("yyyy-MM-dd") ?? "N/A",
                        booking.CheckOutDate?.ToString("yyyy-MM-dd") ?? "N/A"
                    );
                }
            }

            AnsiConsole.Markup($"[bold green]{title}[/]\n");
            AnsiConsole.Write(table);
        }


        private void ExistingRooms(IEnumerable<Room> rooms, string title, bool includeDeleted)
        {
            if (rooms == null || !rooms.Any())
            {
                AnsiConsole.Markup($"[red]No rooms found for {title}[/].\n");
                return;
            }

            var table = new Table()
                .Border(TableBorder.Rounded)
                .AddColumn("[bold]Room ID[/]")
                .AddColumn("[bold]Type[/]")
                .AddColumn("[bold]Price[/]")
                .AddColumn("[bold]Size (sqm)[/]");

            if (!includeDeleted)
            {
                table.AddColumn("[bold]Max People[/]");
            }

            foreach (var room in rooms)
            {
                if (includeDeleted)
                {
                    table.AddRow(
                        room.RoomId.ToString(),
                        room.Type,
                        room.PricePerNight.ToString("C"),
                        room.SizeInSquareMeters.ToString()
                    );
                }
                else
                {
                    table.AddRow(
                        room.RoomId.ToString(),
                        room.Type,
                        room.PricePerNight.ToString("C"),
                        room.SizeInSquareMeters.ToString(),
                        room.TotalPeople.ToString("F1")
                    );
                }
            }

            AnsiConsole.Markup($"[bold green]{title}[/]\n");
            AnsiConsole.Write(table);
        }
        public void DisplayAllRooms()
        {
            var rooms = _roomRepository.GetAllRooms().Where(r => !r.IsDeleted).ToList();

            if (!rooms.Any())
            {
                AnsiConsole.Markup("[red]No active rooms found in the database.[/]\n");
                return;
            }

            var table = new Table()
                .Border(TableBorder.Rounded)
                .AddColumn("[bold]Room ID[/]")
                .AddColumn("[bold]Type[/]")
                .AddColumn("[bold]Price[/]")
                .AddColumn("[bold]Size (sqm)[/]")
                .AddColumn("[bold]Max People[/]");

            foreach (var room in rooms)
            {
                table.AddRow(
                    room.RoomId.ToString(),
                    room.Type,
                    room.PricePerNight.ToString("C"),
                    room.SizeInSquareMeters.ToString(),
                    room.TotalPeople.ToString("F1")
                );
            }

            AnsiConsole.Markup("[bold green]All Existing Rooms in the Database:[/]\n");
            AnsiConsole.Write(table);
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
                    ExistingRooms(activeRooms, "Overview of registered rooms", includeDeleted: false);
                    DisplayBookedActiveRooms(bookedActiveRooms, "Booked Active Rooms");
                    ExistingRooms(removedRooms, "Removed Rooms", includeDeleted: true);
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
