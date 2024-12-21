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

        public IEnumerable<Room> GetRoomsWithBookings()
        {
            return _appDbContext.Rooms
                .Include(r => r.Bookings)
                .Where(r => r.Bookings.Any(b => b.BookingStatus == false)) 
                .ToList();
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

        public void DisplayRoomsTable(IEnumerable<Room> rooms)
        {
            var table = new Table();
            table.AddColumn("Room ID");
            table.AddColumn("Room Type");
            table.AddColumn("Price Per Night");
            table.AddColumn("Guest Name");

            foreach (var room in rooms)
            {
                var activeBooking = room.Bookings.FirstOrDefault(b => b.BookingStatus == false);
                if (activeBooking != null)
                {
                    table.AddRow(
                        room.RoomId.ToString(),
                        room.Type,
                        room.PricePerNight.ToString("C"),
                        $"{activeBooking.Guest.FirstName} {activeBooking.Guest.LastName}");
                }
            }

            AnsiConsole.Write(table);
        }

    }
}
