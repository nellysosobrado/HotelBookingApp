using HotelBookingApp.Entities;
using HotelBookingApp.Repositories;
using Spectre.Console;
using System;
using System.Linq;

namespace HotelBookingApp.Controllers
{
    public class RoomController
    {
        private readonly RoomRepository _roomRepository;

        public RoomController(RoomRepository roomRepository)
        {
            _roomRepository = roomRepository;
        }

        public void AddNewRoom()
        {
            Console.Clear();

            DisplayAllRooms();

            string roomType = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Select [green]Room Type[/]:")
                    .AddChoices("Single", "Double")
                    .HighlightStyle(new Style(foreground: Color.Green))
                    );

            decimal price = AnsiConsole.Ask<decimal>("Enter [green]Price Per Night[/]:");

            int size = AnsiConsole.Ask<int>("Enter [green]Size in Square Meters[/]:");

            int extraBeds = roomType == "Double"
                ? AnsiConsole.Ask<int>("Enter [green]Number of Extra Beds (0-2)[/]:", 0)
                : 0;

            int maxPeople = (roomType == "Double" ? 2 : 1) + extraBeds;

            var newRoom = new Room
            {
                Type = roomType,
                PricePerNight = price,
                SizeInSquareMeters = size,
                ExtraBeds = extraBeds,
                IsAvailable = true,
                TotalPeople = maxPeople
            };

            var result = _roomRepository.AddRoom(newRoom);

            if (result.IsSuccess)
            {
                AnsiConsole.Markup("[green]New room has been added successfully![/]\n");
            }
            else
            {
                AnsiConsole.Markup("[red]Validation Errors:[/]\n");
                foreach (var error in result.Errors)
                {
                    AnsiConsole.Markup($"[red]- {error}[/]\n");
                }
            }

            Console.WriteLine("\nPress any key to return...");
            Console.ReadKey();
        }


        public void EditRoom()
        {
            Console.Clear();
            DisplayAllRooms();

            int roomId = AnsiConsole.Ask<int>("Enter [green]Room ID[/]:");
            var originalRoom = _roomRepository.GetRoomById(roomId);

            if (originalRoom == null)
            {
                AnsiConsole.Markup("[red]Room not found.[/]\n");
                Console.WriteLine("\nPress any key to return...");
                Console.ReadKey();
                return;
            }

            var tempRoom = new Room
            {
                RoomId = originalRoom.RoomId,
                Type = originalRoom.Type,
                PricePerNight = originalRoom.PricePerNight,
                SizeInSquareMeters = originalRoom.SizeInSquareMeters,
                ExtraBeds = originalRoom.ExtraBeds,
                TotalPeople = originalRoom.TotalPeople,
                IsAvailable = originalRoom.IsAvailable
            };

            while (true)
            {
                Console.Clear();
                AnsiConsole.Markup($"[bold green] Room '{tempRoom.RoomId}'[/]\n");

                var roomDetails = new Table()
                    .AddColumn("[bold]Editable[/]")
                    .AddColumn("[bold]Description[/]")
                    .AddRow("Type", tempRoom.Type)
                    .AddRow("Price Per Night", tempRoom.PricePerNight.ToString("C"))
                    .AddRow("Size (sqm)", tempRoom.SizeInSquareMeters.ToString());
                AnsiConsole.Write(roomDetails);

                var action = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("What would you like to edit?")
                        .AddChoices(new[] { "Type", "Price Per Night", "Size", "Confirm Update", "Cancel and Go Back" })
                        .HighlightStyle(new Style(foreground: Color.Green))); 

                switch (action)
                {
                    case "Type":
                        string newType = AnsiConsole.Prompt(
                            new SelectionPrompt<string>()
                                .Title($"Enter new Type:")
                                .AddChoices("Single", "Double")
                                .HighlightStyle(new Style(foreground: Color.Green))); 
                        tempRoom.Type = newType;
                        break;

                    case "Price Per Night":
                        tempRoom.PricePerNight = AnsiConsole.Ask<decimal>("Enter new Price Per Night:");
                        break;

                    case "Size":
                        tempRoom.SizeInSquareMeters = AnsiConsole.Ask<int>("Enter new Size in Square Meters:");
                        break;

                    case "Confirm Update":
                        var validator = new RoomValidator();
                        var validationResult = validator.Validate(tempRoom);

                        if (validationResult.IsValid)
                        {
                            originalRoom.Type = tempRoom.Type;
                            originalRoom.PricePerNight = tempRoom.PricePerNight;
                            originalRoom.SizeInSquareMeters = tempRoom.SizeInSquareMeters;

                            var result = _roomRepository.UpdateRoom(originalRoom);
                            if (result.IsSuccess)
                            {
                                AnsiConsole.Markup("[green]Room has been updated![/]\n");
                                Console.WriteLine("\nPress any key to return...");
                                Console.ReadKey();
                                return; 
                            }
                            else
                            {
                                AnsiConsole.Markup("[red]Error saving the room.[/]\n");
                            }
                        }
                        else
                        {
                            AnsiConsole.Markup("[red]Validation Errors:[/]\n");
                            foreach (var error in validationResult.Errors)
                            {
                                AnsiConsole.Markup($"[red]- {error.ErrorMessage}[/]\n");
                            }
                            AnsiConsole.Markup("[yellow]Fix the errors before finishing![/]\n");
                            Console.WriteLine("\nPress any key to continue...");
                            Console.ReadKey();
                        }
                        break;

                    case "Cancel and Go Back":
                        AnsiConsole.Markup("[yellow]Editing cancelled. No changes have been saved.[/]\n");
                        Console.WriteLine("\nPress any key to return...");
                        Console.ReadKey();
                        return;

                    default:
                        AnsiConsole.Markup("[red]Invalid option selected.[/]\n");
                        break;
                }
            }
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
                            .AddColumn("[bold]Size (sqm)[/]");

                        foreach (var room in activeRooms)
                        {
                            activeRoomsTable.AddRow(
                                room.RoomId.ToString(),
                                room.Type,
                                room.PricePerNight.ToString("C"),
                                room.SizeInSquareMeters.ToString()
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
                        AnsiConsole.Markup("\n[bold yellow]All rooms that has been deleted[/]\n");
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
                        .AddChoices(new[] { "Add a Room", "Edit a Room", "Delete a Room", "Find Room by Available Date", "Go Back" })
                        .HighlightStyle(new Style(foreground: Color.Green)));

                switch (action)
                {
                    case "Add a Room":
                        AddNewRoom();
                        break;

                    case "Edit a Room":
                        EditRoom();
                        break;

                    case "Delete a Room":
                        DeleteRoom();
                        break;

                    case "Find Room by Available Date":
                        FindRoomByAvailableDate();
                        break;

                    case "Go Back":
                        return;

                    default:
                        AnsiConsole.Markup("[red]Invalid option selected.[/]\n");
                        break;
                }
            }
        }

        private void FindRoomByAvailableDate()
        {
            Console.Clear();

            DisplayAllRooms();

            var startDate = AnsiConsole.Ask<DateTime>("Enter [green]Start Date (yyyy-MM-dd)[/]:");
            var endDate = AnsiConsole.Ask<DateTime>("Enter [green]End Date (yyyy-MM-dd)[/]:");

            var dateRangeValidator = new DateRangeValidator();
            var dateValidationResult = dateRangeValidator.Validate((startDate, endDate));

            if (!dateValidationResult.IsValid)
            {
                AnsiConsole.Markup("[red]Validation Errors:[/]\n");
                foreach (var error in dateValidationResult.Errors)
                {
                    AnsiConsole.Markup($"[red]- {error.ErrorMessage}[/]\n");
                }
                Console.ReadKey();
                return;
            }

            var rooms = _roomRepository.GetRoomsWithBookings();
            var availableRooms = rooms.Where(room =>
                !room.Bookings.Any(booking =>
                    booking.CheckInDate.HasValue &&
                    booking.CheckOutDate.HasValue &&
                    !(booking.CheckOutDate.Value < startDate || booking.CheckInDate.Value > endDate)))
                .ToList();

            if (!availableRooms.Any())
            {
                AnsiConsole.Markup("[red]No rooms available for the selected dates.[/]\n");
            }
            else
            {
                var table = new Table()
                    .Border(TableBorder.Rounded)
                    .AddColumn("[bold]Room ID[/]")
                    .AddColumn("[bold]Type[/]")
                    .AddColumn("[bold]Price[/]")
                    .AddColumn("[bold]Size (sqm)[/]");

                foreach (var room in availableRooms)
                {
                    table.AddRow(
                        room.RoomId.ToString(),
                        room.Type,
                        room.PricePerNight.ToString("C"),
                        room.SizeInSquareMeters.ToString()
                    );
                }

                AnsiConsole.Markup("[bold green]Available Rooms for the Selected Dates:[/]\n");
                AnsiConsole.Write(table);
            }

            Console.WriteLine("\nPress any key to go back..");
            Console.ReadKey();
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
                AnsiConsole.Markup($"\n[bold green]All '{roomType}' Rooms bookings:[/]\n");
                var table = new Table()
                    .Border(TableBorder.Rounded)
                    .AddColumn("[bold]Room ID[/]")
                    .AddColumn("[bold]Guest[/]")
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
                .AddColumn("[bold]Size (sqm)[/]");

            foreach (var room in rooms)
            {
                table.AddRow(
                    room.RoomId.ToString(),
                    room.Type,
                    room.PricePerNight.ToString("C"),
                    room.SizeInSquareMeters.ToString()
                );
            }

            AnsiConsole.Markup("[bold green]All Existing Rooms in the Database:[/]\n");
            AnsiConsole.Write(table);
        }
        public void DeleteRoom()
        {
            Console.Clear();

            var rooms = _roomRepository.GetAllRooms().Where(r => !r.IsDeleted).ToList();
            if (!rooms.Any())
            {
                AnsiConsole.Markup("[red]No available rooms to delete.[/]\n");
                Console.WriteLine("\nPress any key to return...");
                Console.ReadKey();
                return;
            }

            DisplayAllRooms();

            int roomId = AnsiConsole.Ask<int>("Enter [green]Room ID to delete[/]:");

            var room = _roomRepository.GetRoomById(roomId);

            if (room == null)
            {
                AnsiConsole.Markup("[red]Room not found. Please enter a valid Room ID.[/]\n");
                Console.WriteLine("\nPress any key to return...");
                Console.ReadKey();
                return;
            }

            var validator = new RoomDeleteValidator();
            var validationResult = validator.Validate(room);

            if (!validationResult.IsValid)
            {
                AnsiConsole.Markup("[red]Validation Errors:[/]\n");
                foreach (var error in validationResult.Errors)
                {
                    AnsiConsole.Markup($"[red]- {error.ErrorMessage}[/]\n");
                }

                Console.WriteLine("\nPress any key to return...");
                Console.ReadKey();
                return;
            }

            room.IsDeleted = true;
            var result = _roomRepository.UpdateRoom(room);

            if (result.IsSuccess)
            {
                AnsiConsole.Markup($"[green]Room ID {roomId} marked as deleted successfully![/]\n");
            }
            else
            {
                AnsiConsole.Markup($"[red]Error deleting room: {result.Errors.FirstOrDefault()}[/]\n");
            }

            Console.WriteLine("\nPress any key to return...");
            Console.ReadKey();
        }
    }
}
