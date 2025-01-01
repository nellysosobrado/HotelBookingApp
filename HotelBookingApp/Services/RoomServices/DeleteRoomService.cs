using HotelBookingApp.Entities;
using HotelBookingApp.Repositories;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HotelBookingApp.Services.RoomServices
{
    public class DeleteRoomService
    {
        private readonly RoomRepository _roomRepository;

        public DeleteRoomService(RoomRepository roomRepository)
        {
            _roomRepository = roomRepository;
        }

        public void Execute()
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

            DisplayAllRooms(rooms);

            int roomId = GetRoomId();
            var room = _roomRepository.GetRoomById(roomId);

            if (room == null)
            {
                AnsiConsole.Markup("[red]Room not found. Please enter a valid Room ID.[/]\n");
                Console.WriteLine("\nPress any key to return...");
                Console.ReadKey();
                return;
            }

            if (ValidateRoomDeletion(room))
            {
                MarkRoomAsDeleted(room);
            }
        }

        private void DisplayAllRooms(IEnumerable<Room> rooms)
        {
            var table = new Table()
                .Border(TableBorder.Rounded)
                .AddColumn("[bold]Room ID[/]")
                .AddColumn("[bold]Type[/]")
                .AddColumn("[bold]Price[/]")
                .AddColumn("[bold]Size (sqm)[/]");

            foreach (var room in rooms)
            {
                bool hasActiveBooking = room.Bookings != null && room.Bookings.Any();

                table.AddRow(
                    room.RoomId.ToString(),
                    room.Type,
                    room.PricePerNight.ToString("C"),
                    room.SizeInSquareMeters.ToString()
                );
            }

            AnsiConsole.Markup("[bold green]Available Rooms:[/]\n");
            AnsiConsole.Write(table);
        }

        private int GetRoomId()
        {
            return AnsiConsole.Ask<int>("Enter [green]Room ID to delete[/]:");
        }

        private bool ValidateRoomDeletion(Room room)
        {
            var activeBookings = _roomRepository.GetActiveBookingsForRoom(room.RoomId);

            if (activeBookings.Any())
            {
                AnsiConsole.Markup("[red]The room cannot be deleted because it is associated with active bookings.[/]\n");

                Console.WriteLine("\nPlease unbook all associated bookings first. Press any key to return...");
                Console.ReadKey();
                return false;
            }

            return true;
        }

        private void MarkRoomAsDeleted(Room room)
        {
            if (room.IsDeleted)
            {
                AnsiConsole.Markup("[red]The room is already marked as deleted. No further action is required.[/]\n");
                Console.WriteLine("\nPress any key to return...");
                Console.ReadKey();
                return;
            }

            try
            {
                room.IsDeleted = true;
                _roomRepository.UpdateRoom(room);

                AnsiConsole.Markup($"[green]Room ID {room.RoomId} marked as deleted successfully![/]\n");
            }
            catch (Exception ex)
            {
                AnsiConsole.Markup($"[red]Error deleting room: {ex.Message}[/]\n");
            }

            Console.WriteLine("\nPress any key to return...");
            Console.ReadKey();
        }
    }
}
