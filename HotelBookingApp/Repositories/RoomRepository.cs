﻿using HotelBookingApp.Data;
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
        private readonly BookingRepository _bookingRepository;
        public RoomRepository(AppDbContext context, BookingRepository bookingRepository)
        {
            _appDbContext = context;
            _bookingRepository = bookingRepository;
        }
        public IEnumerable<Room> GetAvailableRoomsByCapacityAndDates(int totalPeople, DateTime startDate, DateTime endDate)
        {
            var canceledBookingIds = _appDbContext.CanceledBookingsHistory
                .Select(cb => cb.BookingId)
                .ToHashSet();

            return _appDbContext.Rooms
                .Where(r => r.TotalPeople >= totalPeople && !r.IsDeleted) 
                .Where(r => r.Bookings.All(b =>
                    canceledBookingIds.Contains(b.BookingId) || 
                    b.CheckOutDate < startDate ||             
                    b.CheckInDate > endDate ||               
                    b.IsCheckedOut))                        
                .ToList();
        }

        public List<Room> GetAvailableRooms(DateTime startDate, DateTime endDate, string roomType)
        {
            var rooms = GetRoomsWithBookings();

            var canceledBookingIds = _bookingRepository.GetCanceledBookingsHistory()
                .Select(cb => cb.BookingId)
                .ToHashSet();

            return rooms
                .Where(room =>
                    !room.IsDeleted && 
                    room.Type.Equals(roomType, StringComparison.OrdinalIgnoreCase) && 
                    !room.Bookings.Any(booking =>
                        !canceledBookingIds.Contains(booking.BookingId) && 
                        booking.CheckInDate.HasValue &&
                        booking.CheckOutDate.HasValue &&
                        !(booking.CheckOutDate.Value <= startDate || booking.CheckInDate.Value >= endDate))) 
                .ToList();
        }
        public IEnumerable<Booking> GetActiveBookingsForRoom(int roomId, bool includeGuest = false)
        {
            var canceledBookingIds = _appDbContext.CanceledBookingsHistory
                .Select(cb => cb.BookingId)
                .ToHashSet();

            var query = _appDbContext.Bookings
                .Where(b => b.RoomId == roomId && !canceledBookingIds.Contains(b.BookingId) && !b.IsCheckedOut);

            if (includeGuest)
            {
                query = query.Include(b => b.Guest);
            }

            return query.ToList();
        }


        public IEnumerable<Room> GetRoomsWithActiveBookings(bool includeCheckedOut = false)
        {
            var canceledBookingIds = _appDbContext.CanceledBookingsHistory
                .Select(cb => cb.BookingId)
                .ToHashSet();

            return _appDbContext.Rooms
                .Where(r => !r.IsDeleted)
                .Include(r => r.Bookings.Where(b =>
                    !canceledBookingIds.Contains(b.BookingId) &&
                    (includeCheckedOut || !b.IsCheckedOut)))
                .ThenInclude(b => b.Guest)
                .Where(r => r.Bookings.Any()) 
                .ToList();
        }
        public IEnumerable<(int RoomId, string BookedBy, string StartDate, string EndDate)> GetActiveBookings(IEnumerable<Room> rooms)
        {
            var canceledBookingIds = _appDbContext.CanceledBookingsHistory
                .Select(cb => cb.BookingId)
                .ToHashSet();

            return rooms
                .Where(r => r.Bookings != null && r.Bookings.Any(b => !canceledBookingIds.Contains(b.BookingId)))
                .SelectMany(r => r.Bookings.Where(b => !canceledBookingIds.Contains(b.BookingId)), (room, booking) => (
                    RoomId: room.RoomId,
                    BookedBy: $"{booking.Guest?.FirstName ?? "Unknown"} {booking.Guest?.LastName ?? "Unknown"}",
                    StartDate: booking.CheckInDate?.ToString("yyyy-MM-dd") ?? "N/A",
                    EndDate: booking.CheckOutDate?.ToString("yyyy-MM-dd") ?? "N/A"
                ));
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
            var overlappingBookings = _appDbContext.Bookings.Where(b =>
                b.RoomId == roomId &&
                b.CheckInDate.HasValue &&
                b.CheckOutDate.HasValue &&
                b.CheckInDate.Value < endDate &&
                b.CheckOutDate.Value > startDate).ToList();

            if (overlappingBookings.Any())
            {
                foreach (var booking in overlappingBookings)
                {
                    Console.WriteLine($"Overlapping booking: {booking.BookingId}, CheckIn: {booking.CheckInDate}, CheckOut: {booking.CheckOutDate}");
                }
                return false;
            }

            return true;
        }

        public IEnumerable<Room> GetRoomsByCapacity(int totalPeople)
        {
            // Hämta alla avbokade bokningar
            var canceledBookingIds = _appDbContext.CanceledBookingsHistory
                .Select(cb => cb.BookingId)
                .ToHashSet();

            return _appDbContext.Rooms
                .Where(r => r.TotalPeople >= totalPeople && !r.IsDeleted) 
                .Where(r => r.Bookings.All(b =>
                    canceledBookingIds.Contains(b.BookingId) || 
                    b.CheckOutDate < DateTime.Now ||            
                    b.IsCheckedOut))                            
                .ToList();
        }




        public List<Room> GetAllRooms(bool includeDeleted = false)
        {
            return includeDeleted
                ? _appDbContext.Rooms.ToList() 
                : _appDbContext.Rooms.Where(r => !r.IsDeleted).ToList(); 
        }

        public IEnumerable<Room> GetRoomsWithBookings(bool includeDeleted = false)
        {
            return _appDbContext.Rooms
                .Where(r => includeDeleted || !r.IsDeleted)
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
            try
            {
                _appDbContext.Rooms.Update(room);
                _appDbContext.SaveChanges();
                return RepositoryResult.Success(); 
            }
            catch (Exception ex)
            {
                return RepositoryResult.Failure(new List<string> { ex.Message }); 
            }
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
