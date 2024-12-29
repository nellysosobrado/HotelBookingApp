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


        public void ViewAllRooms()
        {
            while (true)
            {
                Console.Clear();

                var allRooms = _roomRepository.GetAllRooms(includeDeleted: true);
                var activeRooms = allRooms.Where(r => !r.IsDeleted).ToList();
                var removedRooms = allRooms.Where(r => r.IsDeleted).ToList();

                if (!allRooms.Any())
                {
                    AnsiConsole.Markup("[red]No rooms found in the database.[/]\n");
                }
                else
                {
                    if (activeRooms.Any())
                    {
                        AnsiConsole.Markup("[bold green]Overview of all existing rooms:[/]\n");
                        var activeRoomsTable = new Table()
                            .Border(TableBorder.Rounded)
                            .AddColumn("[bold]Room ID[/]")
                            .AddColumn("[bold]Type[/]")
                            .AddColumn("[bold]Price[/]")
                            .AddColumn("[bold]Size (sqm)[/]")
                            .AddColumn("[bold]Max People[/]");

                        foreach (var room in activeRooms)
                        {
                            activeRoomsTable.AddRow(
                                room.RoomId.ToString(),
                                room.Type,
                                room.PricePerNight.ToString("C"),
                                room.SizeInSquareMeters.ToString(),
                                room.TotalPeople.ToString("F1")
                            );
                        }

                        AnsiConsole.Write(activeRoomsTable);
                        DisplayBookedRooms(activeRooms, "Single");
                        DisplayBookedRooms(activeRooms, "Double");
                    }
                    else
                    {
                        AnsiConsole.Markup("[red]No active rooms found in the database.[/]\n");
                    }

                    if (removedRooms.Any())
                    {
                        AnsiConsole.Markup("\n[bold yellow]All rooms that have been deleted[/]\n");
                        var removedRoomsTable = new Table()
                            .Border(TableBorder.Rounded)
                            .AddColumn("[bold]Room ID[/]")
                            .AddColumn("[bold]Type[/]")
                            .AddColumn("[bold]Price[/]")
                            .AddColumn("[bold]Size (sqm)[/]");

                        foreach (var room in removedRooms)
                        {
                            removedRoomsTable.AddRow(
                                room.RoomId.ToString(),
                                room.Type,
                                room.PricePerNight.ToString("C"),
                                room.SizeInSquareMeters.ToString()
                            );
                        }

                        AnsiConsole.Write(removedRoomsTable);
                    }
                    else
                    {
                        AnsiConsole.Markup("\n[red]No removed rooms found in the database.[/]\n");
                    }
                }

                var action = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("What would you like to do?")
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

        private void DisplayBookedRooms(IEnumerable<Room> rooms, string roomType)
        {
            if (rooms == null)
            {
                AnsiConsole.Markup("[red]No rooms found in the database.[/]\n");
                return;
            }

            var bookedRooms = rooms
                .Where(r =>
                    r.Type != null &&
                    r.Type.Equals(roomType, StringComparison.OrdinalIgnoreCase) &&
                    r.Bookings != null &&
                    r.Bookings.Any(b => !b.IsCheckedOut))
                .ToList();

            if (bookedRooms.Any())
            {
                AnsiConsole.Markup($"\n[bold green]Overview of all booked '{roomType}' rooms[/]\n");
                var table = new Table()
                    .Border(TableBorder.Rounded)
                    .AddColumn("[bold]Room ID[/]")
                    .AddColumn("[bold]Booked By[/]")
                    .AddColumn("[bold]Start Date[/]")
                    .AddColumn("[bold]End Date[/]");

                foreach (var room in bookedRooms)
                {
                    foreach (var booking in room.Bookings.Where(b => !b.IsCheckedOut))
                    {
                        table.AddRow(
                            room.RoomId.ToString(),
                            $"{booking.Guest.FirstName} {booking.Guest.LastName}",
                            booking.CheckInDate?.ToString("yyyy-MM-dd") ?? "[grey]N/A[/]",
                            booking.CheckOutDate?.ToString("yyyy-MM-dd") ?? "[grey]N/A[/]"
                        );
                    }
                }

                AnsiConsole.Write(table);
            }
            else
            {
                AnsiConsole.Markup($"\n[red]No {roomType} Rooms are currently booked.[/]\n");
            }
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
  
    }
}
