using HotelBookingApp.Data;
using HotelBookingApp.Entities;
using Microsoft.EntityFrameworkCore;
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

        public List<dynamic> GetRoomsWithBookings()
        {
            return _appDbContext.Rooms
                .Include(r => r.Bookings)
                .Select(r => new
                {
                    Room = r,
                    Booking = r.Bookings.FirstOrDefault(b => !b.IsCheckedOut) 
                })
                .ToList<dynamic>(); 
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

    }
}
