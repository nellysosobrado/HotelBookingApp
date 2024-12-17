using HotelBookingApp.Data;
using HotelBookingApp.Entities;
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
            _appDbContext.SaveChanges();
        }

        public List<dynamic> GetRoomsWithBookings()
        {
            return _appDbContext.Rooms
                .GroupJoin(
                    _appDbContext.Bookings,
                    room => room.RoomId,
                    booking => booking.RoomId,
                    (room, bookings) => new
                    {
                        Room = room,
                        Booking = bookings.FirstOrDefault()
                    })
                .ToList<dynamic>();
        }
        public void DeleteRoom(int roomId)
        {
            var room = _appDbContext.Rooms.FirstOrDefault(r => r.RoomId == roomId);
            if (room != null)
            {
                _appDbContext.Rooms.Remove(room);
                _appDbContext.SaveChanges();
            }
        }
    }
}
