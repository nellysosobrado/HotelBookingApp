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
    public class FindRoomByTotalPeople
    {
        private readonly RoomRepository _roomRepository;

        public FindRoomByTotalPeople(RoomRepository roomRepository)
        {
            _roomRepository = roomRepository;
        }

        public void Execute()
        {
            Console.Clear();
            AnsiConsole.MarkupLine("[bold green]Find Available Room By Total People[/]");

            int totalPeople = GetTotalPeople();

            var availableRooms = GetAvailableRooms(totalPeople);

            DisplayAvailableRooms(availableRooms, totalPeople);
        }

        private int GetTotalPeople()
        {
            return AnsiConsole.Prompt(
                new TextPrompt<int>("[yellow]Enter total number of people (1-4):[/]")
                    .ValidationErrorMessage("[red]Please enter a number between 1 and 4.[/]")
                    .Validate(input => input >= 1 && input <= 4));
        }

        private List<Room> GetAvailableRooms(int totalPeople)
        {
            return _roomRepository.GetAllRooms(includeDeleted: false)
                .Where(r => r.TotalPeople >= totalPeople && !r.IsDeleted)
                .ToList();
        }

        private void DisplayAvailableRooms(List<Room> availableRooms, int totalPeople)
        {
            if (!availableRooms.Any())
            {
                AnsiConsole.MarkupLine($"[red]No available rooms found for {totalPeople} people.[/]");
            }
            else
            {
                var table = new Table()
                    .Border(TableBorder.Rounded)
                    .AddColumn("[blue]Room ID[/]")
                    .AddColumn("[blue]Room Type[/]")
                    .AddColumn("[blue]Price per Night[/]")
                    .AddColumn("[blue]Size (sqm)[/]")
                    .AddColumn("[blue]Max People[/]");

                foreach (var room in availableRooms)
                {
                    table.AddRow(
                        room.RoomId.ToString(),
                        room.Type,
                        room.PricePerNight.ToString("C"),
                        room.SizeInSquareMeters.ToString(),
                        room.TotalPeople.ToString()
                    );
                }

                AnsiConsole.Write(table);
            }

            Console.WriteLine("\nPress any key to go back...");
            Console.ReadKey();
        }
    }

}
