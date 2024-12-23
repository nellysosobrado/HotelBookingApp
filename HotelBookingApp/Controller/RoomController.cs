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
            AnsiConsole.Write(new Panel("[bold yellow]REGISTER NEW ROOM[/]").Expand());

            string roomType = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Select [green]Room Type[/]:")
                    .AddChoices("Single", "Double"));

            decimal price;
            while (true)
            {
                try
                {
                    price = AnsiConsole.Ask<decimal>("Enter [green]Price Per Night[/]:");
                    if (price <= 0)
                        throw new ArgumentException("[red]Price must be greater than 0.[/]");
                    break;
                }
                catch (Exception ex)
                {
                    AnsiConsole.MarkupLine($"[red]{ex.Message}[/]");
                }
            }

            int size;
            while (true)
            {
                try
                {
                    size = AnsiConsole.Ask<int>("Enter [green]Size in Square Meters[/]:");
                    if (size <= 0)
                        throw new ArgumentException("[red]Size must be greater than 0.[/]");
                    break;
                }
                catch (Exception ex)
                {
                    AnsiConsole.MarkupLine($"[red]{ex.Message}[/]");
                }
            }

            int extraBeds = 0;
            int maxPeople = roomType == "Double" ? 2 : 1;

            if (roomType == "Double")
            {
                while (true)
                {
                    try
                    {
                        extraBeds = AnsiConsole.Ask<int>("Enter [green]Number of Extra Beds[/] (0-2):");
                        if (extraBeds < 0 || extraBeds > 2)
                            throw new ArgumentException("[red]Extra beds must be between 0 and 2.[/]");
                        maxPeople += extraBeds;
                        break;
                    }
                    catch (Exception ex)
                    {
                        AnsiConsole.MarkupLine($"[red]{ex.Message}[/]");
                    }
                }
            }

            var newRoom = new Room
            {
                Type = roomType,
                PricePerNight = price,
                SizeInSquareMeters = size,
                ExtraBeds = extraBeds,
                IsAvailable = true,
                TotalPeople = maxPeople
            };

            var validator = new RoomValidator();
            var validationResult = validator.Validate(newRoom);

            if (!validationResult.IsValid)
            {
                AnsiConsole.Write(
                    new Panel("[bold red]Validation errors:[/]").Expand());
                foreach (var error in validationResult.Errors)
                {
                    AnsiConsole.MarkupLine($"[red]- {error.ErrorMessage}[/]");
                }
                return;
            }

            _roomRepository.AddRoom(newRoom);

            var table = new Table()
                .Border(TableBorder.Rounded)
                .AddColumn("[green]Property[/]")
                .AddColumn("[cyan]Value[/]")
                .AddRow("Room Type", roomType)
                .AddRow("Price Per Night", price.ToString("C"))
                .AddRow("Size in Square Meters", size.ToString())
                .AddRow("Extra Beds", extraBeds.ToString())
                .AddRow("Max People Allowed", maxPeople.ToString());

            AnsiConsole.Write(new Panel(table).Header("[green]Room Added Successfully![/]"));

            Console.WriteLine("\nPress any key to return to the menu...");
            Console.ReadKey();
        }
        public void EditRoom()
        {
            Console.Clear();
            AnsiConsole.Markup("[bold yellow]=== EDIT ROOM ===[/]\n");

            Console.Write("Enter Room ID: ");
            if (!int.TryParse(Console.ReadLine(), out var roomId))
            {
                AnsiConsole.Markup("[red]Invalid Room ID.[/]\n");
                return;
            }

            var room = _roomRepository.GetRoomById(roomId);
            if (room == null)
            {
                AnsiConsole.Markup("[red]Room not found.[/]\n");
                return;
            }

            AnsiConsole.Markup("[bold]Leave blank to keep current value.[/]\n");

            Console.Write($"Current Type: {room.Type}\nEnter new Type: ");
            var newType = Console.ReadLine()?.Trim();
            if (!string.IsNullOrWhiteSpace(newType))
                room.Type = newType;

            Console.Write($"Current Price: {room.PricePerNight}\nEnter new Price: ");
            if (decimal.TryParse(Console.ReadLine(), out var price))
                room.PricePerNight = price;

            Console.Write($"Current Size: {room.SizeInSquareMeters}\nEnter new Size: ");
            if (int.TryParse(Console.ReadLine(), out var size))
                room.SizeInSquareMeters = size;

            _roomRepository.UpdateRoom(room);
        }

        public void ViewAllRooms()
        {
            Console.Clear();
            AnsiConsole.Markup("[bold yellow]Rooms[/]\n");

            var rooms = _roomRepository.GetRoomsWithBookings();

            if (!rooms.Any())
            {
                AnsiConsole.Markup("[red]No rooms found.[/]\n");
                return;
            }

            var table = new Table();
            table.Border(TableBorder.Rounded);
            table.AddColumn("[bold yellow]Room ID[/]");
            table.AddColumn("[bold yellow]Type[/]");
            table.AddColumn("[bold yellow]Size (sqm)[/]");
            table.AddColumn("[bold yellow]Price Per Night[/]");
            table.AddColumn("[bold yellow]Extra Beds[/]");
            table.AddColumn("[bold yellow]Booking Status[/]");

            foreach (var room in rooms)
            {
                var activeBooking = room.Bookings.FirstOrDefault(b => !b.IsCheckedOut);

                string bookingStatus = activeBooking != null
                    ? $"Booked by: {activeBooking.Guest.FirstName} {activeBooking.Guest.LastName}"
                    : "Not booked by anyone, room is available";

                table.AddRow(
                    room.RoomId.ToString(),
                    room.Type,
                    $"{room.SizeInSquareMeters} sqm",
                    $"{room.PricePerNight:C}",
                    room.ExtraBeds.ToString(),
                    bookingStatus
                );
            }

            AnsiConsole.Write(table);
            AnsiConsole.Markup("\n[bold yellow]Press any key to return...[/]");
            Console.ReadKey();
        }

        public void DeleteRoom() 
        {
            Console.Clear();
            AnsiConsole.Markup("[bold yellow]=== DELETE ROOM ===[/]\n");

            Console.Write("Enter Room ID to delete: ");
            if (!int.TryParse(Console.ReadLine(), out var roomId))
            {
                AnsiConsole.Markup("[red]Invalid Room ID.[/]\n");
                return;
            }

            var room = _roomRepository.GetRoomById(roomId);
            if (room == null)
            {
                AnsiConsole.Markup("[red]Room not found.[/]\n");
                return;
            }

            Console.WriteLine($"Are you sure you want to delete Room ID {room.RoomId} (Type: {room.Type})? (Y/N)");
            var confirmation = Console.ReadLine()?.Trim().ToUpper();

            if (confirmation == "Y")
            {
                _roomRepository.DeleteRoom(roomId);
                AnsiConsole.Markup($"[green]Room ID {roomId} has been successfully deleted.[/]\n");
            }
            else
            {
                AnsiConsole.Markup("[yellow]Operation cancelled.[/]\n");
            }
        }

        private decimal GetValidDecimal(string prompt, string errorMessage)
        {
            while (true)
            {
                Console.Write($"{prompt}: ");
                if (decimal.TryParse(Console.ReadLine(), out var value) && value > 0)
                    return value;

                AnsiConsole.Markup($"[red]{errorMessage}[/]\n");
            }
        }

        private int GetValidInt(string prompt, string errorMessage, int? minValue = null, int? maxValue = null)
        {
            while (true)
            {
                Console.Write($"{prompt}: ");
                if (int.TryParse(Console.ReadLine(), out var value) &&
                    (!minValue.HasValue || value >= minValue.Value) &&
                    (!maxValue.HasValue || value <= maxValue.Value))
                    return value;

                AnsiConsole.Markup($"[red]{errorMessage}[/]\n");
            }
        }
    }
}
