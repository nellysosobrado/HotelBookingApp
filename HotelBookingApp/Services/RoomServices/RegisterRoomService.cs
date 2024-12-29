using HotelBookingApp.Entities;
using HotelBookingApp.Repositories;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBookingApp.Services.RoomServices
{
    public class RegisterRoomService
    {
        private readonly RoomRepository _roomRepository;

        public RegisterRoomService(RoomRepository roomRepository)
        {
            _roomRepository = roomRepository;
        }

        public void Execute()
        {
            Console.Clear();

            DisplayAllRooms();

            DisplayRoomTypeInfo();

            string roomType = GetRoomType();
            decimal price = GetRoomPrice();
            int size = GetRoomSize();
            int maxPeople = GetMaxPeople(roomType);
            int extraBeds = CalculateExtraBeds(roomType, maxPeople);

            var newRoom = new Room
            {
                Type = roomType,
                PricePerNight = price,
                SizeInSquareMeters = size,
                ExtraBeds = extraBeds,
                IsAvailable = true,
                TotalPeople = maxPeople
            };

            SaveRoom(newRoom);
        }

        private void DisplayAllRooms()
        {
            var rooms = _roomRepository.GetRoomsWithBookings();
            if (!rooms.Any())
            {
                AnsiConsole.Markup("[red]No rooms available to display.[/]\n");
                return;
            }

            var table = new Table()
                .Border(TableBorder.Rounded)
                .AddColumn("[bold]Room ID[/]")
                .AddColumn("[bold]Type[/]")
                .AddColumn("[bold]Price[/]")
                .AddColumn("[bold]Size (sqm)[/]")
                .AddColumn("[bold]Total People[/]");

            foreach (var room in rooms)
            {
                table.AddRow(
                    room.RoomId.ToString(),
                    room.Type,
                    room.PricePerNight.ToString("C"),
                    room.SizeInSquareMeters.ToString(),
                    room.TotalPeople.ToString()
                );
            }

            AnsiConsole.Markup("[bold green]All Rooms:[/]\n");
            AnsiConsole.Write(table);
        }

        private void DisplayRoomTypeInfo()
        {
            AnsiConsole.Markup("Room Types:\n");
            AnsiConsole.Markup("[green]Single Room[/]: Max 1-2 People\n");
            AnsiConsole.Markup("[green]Double Room[/]: Max 1-4 People\n");
        }

        private string GetRoomType()
        {
            return AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("\nSelect [green]Room Type[/]:")
                    .AddChoices("Single", "Double")
                    .HighlightStyle(new Style(foreground: Color.Green))
            );
        }

        private decimal GetRoomPrice()
        {
            return AnsiConsole.Prompt(
                new TextPrompt<decimal>("Enter [green]Price Per Night[/]:")
                    .Validate(input => input > 0 ? ValidationResult.Success() : ValidationResult.Error("[red]Price must be greater than 0.[/]"))
            );
        }

        private int GetRoomSize()
        {
            return AnsiConsole.Prompt(
                new TextPrompt<int>("Enter [green]Size in Square Meters[/]:")
                    .Validate(input => input > 0 ? ValidationResult.Success() : ValidationResult.Error("[red]Size must be greater than 0.[/]"))
            );
        }

        private int GetMaxPeople(string roomType)
        {
            return roomType == "Single"
                ? AnsiConsole.Prompt(
                    new TextPrompt<int>("Enter [green]Number of People (1-2)[/]:")
                        .Validate(input => input >= 1 && input <= 2 ? ValidationResult.Success() : ValidationResult.Error("[red]Please enter a number between 1 and 2.[/]"))
                )
                : AnsiConsole.Prompt(
                    new TextPrompt<int>("Enter [green]Number of People (1-4)[/]:")
                        .Validate(input => input >= 1 && input <= 4 ? ValidationResult.Success() : ValidationResult.Error("[red]Please enter a number between 1 and 4.[/]"))
                );
        }

        private int CalculateExtraBeds(string roomType, int maxPeople)
        {
            return roomType == "Double" && maxPeople > 2 ? maxPeople - 2 : 0;
        }

        private void SaveRoom(Room newRoom)
        {
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
    }

}
