using HotelBookingApp.Data;
using HotelBookingApp.Entities;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
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
        public IEnumerable<Room> GetAvailableRoomsByDate(DateTime startDate, DateTime endDate, string roomType)
        {
            return _appDbContext.Rooms
                .Where(r => r.Type == roomType && r.IsAvailable && !_appDbContext.Bookings
                    .Any(b => b.RoomId == r.RoomId &&
                              ((b.CheckInDate <= endDate && b.CheckInDate >= startDate) ||
                               (b.CheckOutDate <= endDate && b.CheckOutDate >= startDate))))
                .ToList();
        }

        public bool IsRoomAvailable(int roomId, DateTime startDate, DateTime endDate)
        {
            return !_appDbContext.Bookings.Any(b =>
                b.RoomId == roomId &&
                b.CheckInDate.HasValue &&
                b.CheckOutDate.HasValue &&
                b.CheckInDate.Value < endDate &&
                b.CheckOutDate.Value > startDate);
        }

        public IEnumerable<Room> GetRoomsByCapacity(int totalPeople)
        {
            return _appDbContext.Rooms
                .Where(r => r.TotalPeople >= totalPeople && r.IsAvailable)
                .ToList();
        }

        public List<Room> GetAllRooms(bool includeDeleted = false)
        {
            return includeDeleted
                ? _appDbContext.Rooms.ToList() 
                : _appDbContext.Rooms.Where(r => !r.IsDeleted).ToList(); 
        }


        public IEnumerable<Room> GetRoomsWithBookings()
        {
            return _appDbContext.Rooms
                .Include(r => r.Bookings) 
                .ThenInclude(b => b.Guest) 
                .ToList();
        }

        public RepositoryResult AddRoom(Room room)
        {
            var validator = new RoomValidator();
            var validationResult = validator.Validate(room);

            if (!validationResult.IsValid)
            {
                return RepositoryResult.Failure(validationResult.Errors.Select(e => e.ErrorMessage).ToList());
            }

            _appDbContext.Rooms.Add(room);
            _appDbContext.SaveChanges();
            return RepositoryResult.Success();
        }

        public Room GetRoomById(int roomId)
        {
            return _appDbContext.Rooms.FirstOrDefault(r => r.RoomId == roomId);
        }

        public List<Room> GetAllRooms()
        {
            return _appDbContext.Rooms.ToList();
        }

        public RepositoryResult UpdateRoom(Room room)
        {
            var validator = new RoomValidator();
            var validationResult = validator.Validate(room);

            if (!validationResult.IsValid)
            {
                return RepositoryResult.Failure(validationResult.Errors.Select(e => e.ErrorMessage).ToList());
            }

            _appDbContext.Rooms.Update(room);
            _appDbContext.SaveChanges();
            return RepositoryResult.Success();
        }

        public RepositoryResult DeleteRoom(int roomId)
        {
            var room = GetRoomById(roomId);

            if (room == null)
            {
                return RepositoryResult.Failure(new List<string> { "Room not found." });
            }

            _appDbContext.Rooms.Remove(room);
            _appDbContext.SaveChanges();
            return RepositoryResult.Success();
        }
    }


    public class RepositoryResult
    {
        public bool IsSuccess { get; private set; }
        public List<string> Errors { get; private set; }

        private RepositoryResult(bool isSuccess, List<string> errors = null)
        {
            IsSuccess = isSuccess;
            Errors = errors ?? new List<string>();
        }

        public static RepositoryResult Success()
        {
            return new RepositoryResult(true);
        }

        public static RepositoryResult Failure(List<string> errors)
        {
            return new RepositoryResult(false, errors);
        }
    }
}
