﻿using HotelBookingApp.Entities;
using HotelBookingApp.Repositories;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HotelBookingApp.Services
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

            if (!ValidateRoomDeletion(room)) return;

            MarkRoomAsDeleted(room);
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
            var activeBookings = room.Bookings?.Where(b => !b.IsCanceled).ToList();

            if (activeBookings != null && activeBookings.Any())
            {
                AnsiConsole.Markup("[red]The room cannot be deleted because it is associated with active bookings.[/]\n");
                foreach (var booking in activeBookings)
                {
                    AnsiConsole.Markup($"[red]- Booking ID {booking.BookingId}: Check-In {booking.CheckInDate:yyyy-MM-dd}, Check-Out {booking.CheckOutDate:yyyy-MM-dd}[/]\n");
                }
                Console.WriteLine("\nPress any key to return...");
                Console.ReadKey();
                return false;
            }

            return true;
        }

        private void MarkRoomAsDeleted(Room room)
        {
            room.IsDeleted = true;
            var result = _roomRepository.UpdateRoom(room);

            if (result.IsSuccess)
            {
                AnsiConsole.Markup($"[green]Room ID {room.RoomId} marked as deleted successfully![/]\n");
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
