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
            _appDbContext.Rooms.Add(room);
            _appDbContext.SaveChanges();
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
            _appDbContext.Rooms.Update(room);
            _appDbContext.SaveChanges();
        }

        public IEnumerable<(Room Room, Booking? Booking)> GetRoomsWithBookings()
        {
            return _appDbContext.Rooms
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
            if (room != null)
            {
                _appDbContext.Rooms.Remove(room);
                _appDbContext.SaveChanges();
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
