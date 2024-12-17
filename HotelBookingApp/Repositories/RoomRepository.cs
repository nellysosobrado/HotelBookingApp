using HotelBookingApp.Data;
using HotelBookingApp.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HotelBookingApp.Repositories
{
    public class RoomRepository
    {
        private readonly AppDbContext _context;

        public RoomRepository(AppDbContext context)
        {
            _context = context;
        }

        public void AddRoom(Room room)
        {
            _context.Rooms.Add(room);
            _context.SaveChanges();
        }

        public Room GetRoomById(int roomId)
        {
            return _context.Rooms.FirstOrDefault(r => r.RoomId == roomId);
        }

        public List<Room> GetAllRooms()
        {
            return _context.Rooms.ToList();
        }

        public void UpdateRoom(Room room)
        {
            _context.SaveChanges();
        }

        public List<dynamic> GetRoomsWithBookings()
        {
            return _context.Rooms
                .GroupJoin(
                    _context.Bookings,
                    room => room.RoomId,
                    booking => booking.RoomId,
                    (room, bookings) => new
                    {
                        Room = room,
                        Booking = bookings.FirstOrDefault()
                    })
                .ToList<dynamic>();
        }
    }
}
