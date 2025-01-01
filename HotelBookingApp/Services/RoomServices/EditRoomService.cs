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
    public class EditRoomService
    {
        private readonly RoomRepository _roomRepository;

        public EditRoomService(RoomRepository roomRepository)
        {
            _roomRepository = roomRepository;
        }

        public void Execute()
        {
            Console.Clear();
            DisplayAllRooms();

            int roomId = GetRoomId();
            var originalRoom = _roomRepository.GetRoomById(roomId);

            if (originalRoom == null)
            {
                AnsiConsole.Markup("[red]Room not found.[/]\n");
                Console.WriteLine("\nPress any key to return...");
                Console.ReadKey();
                return;
            }

            var tempRoom = CloneRoom(originalRoom);

            while (true)
            {
                Console.Clear();
                DisplayRoomDetails(tempRoom);

                var action = GetEditAction();

                if (HandleEditAction(action, tempRoom, originalRoom))
                    return; 
            }
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

        private int GetRoomId()
        {
            return AnsiConsole.Ask<int>("Enter [green]Room ID[/]:");
        }

        private Room CloneRoom(Room originalRoom)
        {
            return new Room
            {
                RoomId = originalRoom.RoomId,
                Type = originalRoom.Type,
                PricePerNight = originalRoom.PricePerNight,
                SizeInSquareMeters = originalRoom.SizeInSquareMeters,
                ExtraBeds = originalRoom.ExtraBeds,
                TotalPeople = originalRoom.TotalPeople,
                IsAvailable = originalRoom.IsAvailable
            };
        }

        private void DisplayRoomDetails(Room room)
        {
            AnsiConsole.Markup($"[bold green]Editing Room '{room.RoomId}'[/]\n");

            var roomDetails = new Table()
                .AddColumn("[bold]Editable[/]")
                .AddColumn("[bold]Description[/]")
                .AddRow("Type", room.Type)
                .AddRow("Price Per Night", room.PricePerNight.ToString("C"))
                .AddRow("Size (sqm)", room.SizeInSquareMeters.ToString())
                .AddRow("Extra Beds", room.ExtraBeds.ToString())
                .AddRow("Total People", room.TotalPeople.ToString());

            AnsiConsole.Write(roomDetails);
        }

        private string GetEditAction()
        {
            return AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("What would you like to edit?")
                    .AddChoices(new[] { "Type", "Price Per Night", "Size", "Extra Beds", "Total People", "Confirm Update", "Cancel and Go Back" })
                    .HighlightStyle(new Style(foreground: Color.Green))
            );
        }

        private bool HandleEditAction(string action, Room tempRoom, Room originalRoom)
        {
            switch (action)
            {
                case "Type":
                    tempRoom.Type = AnsiConsole.Prompt(
                        new SelectionPrompt<string>()
                            .Title($"Enter new Type:")
                            .AddChoices("Single", "Double")
                            .HighlightStyle(new Style(foreground: Color.Green))
                    );
                    break;

                case "Price Per Night":
                    tempRoom.PricePerNight = AnsiConsole.Ask<decimal>("Enter new Price Per Night:");
                    break;

                case "Size":
                    tempRoom.SizeInSquareMeters = AnsiConsole.Ask<int>("Enter new Size in Square Meters:");
                    break;

                case "Extra Beds":
                    EditExtraBeds(tempRoom);
                    break;

                case "Total People":
                    EditTotalPeople(tempRoom);
                    break;

                case "Confirm Update":
                    return ConfirmUpdate(tempRoom, originalRoom);

                case "Cancel and Go Back":
                    AnsiConsole.Markup("[yellow]Editing cancelled. No changes have been saved.[/]\n");
                    Console.WriteLine("\nPress any key to return...");
                    Console.ReadKey();
                    return true;

                default:
                    AnsiConsole.Markup("[red]Invalid option selected.[/]\n");
                    break;
            }

            return false; // Continue editing
        }

        private void EditExtraBeds(Room room)
        {
            if (room.Type == "Double")
            {
                int maxExtraBeds = room.SizeInSquareMeters > 50 ? 2 : 1;
                room.ExtraBeds = AnsiConsole.Prompt(
                    new TextPrompt<int>($"Enter number of Extra Beds (0-{maxExtraBeds}):")
                        .Validate(input => input >= 0 && input <= maxExtraBeds
                            ? ValidationResult.Success()
                            : ValidationResult.Error($"[red]Extra Beds must be between 0 and {maxExtraBeds}.[/]"))
                );
            }
            else
            {
                AnsiConsole.Markup("[yellow]Extra beds can only be added to Double rooms.[/]\n");
                Console.WriteLine("\nPress any key to continue...");
                Console.ReadKey();
            }
        }

        private void EditTotalPeople(Room room)
        {
            if (room.Type == "Single")
            {
                room.TotalPeople = AnsiConsole.Prompt(
                    new TextPrompt<int>("Enter Total People (1-2):")
                        .Validate(input => input >= 1 && input <= 2
                            ? ValidationResult.Success()
                            : ValidationResult.Error("[red]Total People for Single rooms must be between 1 and 2.[/]"))
                );
            }
            else if (room.Type == "Double")
            {
                room.TotalPeople = AnsiConsole.Prompt(
                    new TextPrompt<int>("Enter Total People (1-4):")
                        .Validate(input => input >= 1 && input <= 4
                            ? ValidationResult.Success()
                            : ValidationResult.Error("[red]Total People for Double rooms must be between 1 and 4.[/]"))
                );
            }
            else
            {
                AnsiConsole.Markup("[red]Invalid room type.[/]\n");
                Console.WriteLine("\nPress any key to continue...");
                Console.ReadKey();
            }
        }

        private bool ConfirmUpdate(Room tempRoom, Room originalRoom)
        {
            var validator = new RoomValidator();
            var validationResult = validator.Validate(tempRoom);

            if (validationResult.IsValid)
            {
                originalRoom.Type = tempRoom.Type;
                originalRoom.PricePerNight = tempRoom.PricePerNight;
                originalRoom.SizeInSquareMeters = tempRoom.SizeInSquareMeters;
                originalRoom.ExtraBeds = tempRoom.ExtraBeds;
                originalRoom.TotalPeople = tempRoom.TotalPeople;

                var result = _roomRepository.UpdateRoom(originalRoom);
                if (result.IsSuccess)
                {
                    AnsiConsole.Markup("[green]Room has been updated![/]\n");
                    Console.WriteLine("\nPress any key to return...");
                    Console.ReadKey();
                    return true;
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
                Console.WriteLine("\nPress any key to continue...");
                Console.ReadKey();
            }

            return false; 
        }
    }

}
