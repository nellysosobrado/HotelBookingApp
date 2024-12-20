using HotelBookingApp.Data;
using HotelBookingApp.Entities;
using Microsoft.EntityFrameworkCore;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HotelBookingApp.Repositories
{
    public class RoomRepository
    {
        private readonly AppDbContext _appDbContext;

        public RoomRepository(AppDbContext context)
        {
            _appDbContext = context;
        }

        public void AddRoom(Room room)
        {
            var validator = new RoomValidator();
            var validationResult = validator.Validate(room);

            if (!validationResult.IsValid)
            {
                AnsiConsole.Markup("[red]Validation errors:[/]\n");
                foreach (var error in validationResult.Errors)
                {
                    AnsiConsole.Markup($"[red]- {error.ErrorMessage}[/]\n");
                }
                return;
            }

            try
            {
                _appDbContext.Rooms.Add(room);
                _appDbContext.SaveChanges();

                AnsiConsole.Markup($"[green]Room id:{room.RoomId} added successfully![/]\n");
            }
            catch (Exception ex)
            {
                AnsiConsole.Markup($"[red]Error adding room: {ex.Message}[/]\n");
            }
        }

        public Room GetRoomById(int roomId)
        {
            return _appDbContext.Rooms.FirstOrDefault(r => r.RoomId == roomId);
        }

        public List<Room> GetAllRooms()
        {
            return _appDbContext.Rooms.ToList();
        }

        public void UpdateRoom(Room room)
        {
            var validator = new RoomValidator();
            var validationResult = validator.Validate(room);

            if (!validationResult.IsValid)
            {
                AnsiConsole.Markup("[red]Validation errors:[/]\n");
                foreach (var error in validationResult.Errors)
                {
                    AnsiConsole.Markup($"[red]- {error.ErrorMessage}[/]\n");
                }
                return;
            }

            try
            {
                _appDbContext.Rooms.Update(room);
                _appDbContext.SaveChanges();
                AnsiConsole.Markup("[green]Room updated successfully![/]\n");
            }
            catch (Exception ex)
            {
                AnsiConsole.Markup($"[red]Error updating room: {ex.Message}[/]\n");
            }
        }

        public IEnumerable<(Room Room, Booking? Booking)> GetRoomsWithBookings()
        {
            return _appDbContext.Rooms
                .Include(room => room.Bookings) // Inkludera relaterade bokningar om de finns
                .GroupJoin(
                    _appDbContext.Bookings,
                    room => room.RoomId,
                    booking => booking.RoomId,
                    (room, bookings) => new { Room = room, Booking = bookings.FirstOrDefault() }
                )
                .AsEnumerable()
                .Select(e => (e.Room, e.Booking));
        }

        public void DeleteRoom(int roomId)
        {
            var room = GetRoomById(roomId);
            if (room == null)
            {
                AnsiConsole.Markup("[red]Room not found.[/]\n");
                return;
            }

            try
            {
                _appDbContext.Rooms.Remove(room);
                _appDbContext.SaveChanges();
                AnsiConsole.Markup($"[green]Room with ID {roomId} has been deleted successfully.[/]\n");
            }
            catch (Exception ex)
            {
                AnsiConsole.Markup($"[red]Error deleting room: {ex.Message}[/]\n");
            }
        }

        public void DisplayRoomsTable(IEnumerable<(Room Room, Booking? Booking)> rooms)
        {
            var table = new Table()
                .AddColumns("[green]Room ID[/]", "[blue]Type[/]", "[yellow]Price Per Night[/]", "[cyan]Booked By[/]");

            foreach (var (room, booking) in rooms)
            {
                var bookedBy = booking?.Guest != null
                    ? $"{booking.Guest.FirstName} {booking.Guest.LastName}"
                    : "Not Booked";

                table.AddRow(
                    room.RoomId.ToString(),
                    room.Type ?? "N/A",
                    room.PricePerNight.ToString("C"),
                    bookedBy
                );
            }

            AnsiConsole.Write(table);
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }
    }
}
