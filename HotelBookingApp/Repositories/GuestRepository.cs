using HotelBookingApp.Data;
using HotelBookingApp.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace HotelBookingApp.Repositories
{
    public class GuestRepository
    {
        private readonly AppDbContext _appDbContext;

        public GuestRepository(AppDbContext context)
        {
            _appDbContext = context;
        }

        public List<Guest> GetAllGuests()
        {
            return _appDbContext.Guests.ToList();
        }

        public Guest GetGuestById(int guestId)
        {
            return _appDbContext.Guests.FirstOrDefault(g => g.GuestId == guestId);
        }

        public void AddGuest(Guest guest)
        {
            _appDbContext.Guests.Add(guest);
            _appDbContext.SaveChanges();
        }

        public void UpdateGuest(Guest guest)
        {
            _appDbContext.SaveChanges();
        }

        public List<object> GetGuestsWithBookings()
        {
            return _appDbContext.Guests
                .GroupJoin(
                    _appDbContext.Bookings,
                    guest => guest.GuestId,
                    booking => booking.GuestId,
                    (guest, bookings) => new
                    {
                        Guest = guest,
                        Bookings = bookings.Select(b => new
                        {
                            b.RoomId,
                            b.IsCheckedIn,
                            b.IsCheckedOut,
                            b.BookingStatus
                        }).ToList()
                    }
                )
                .Cast<object>() 
                .ToList();
        }
        public void RemoveGuest(int guestId)
        {
            var guest = _appDbContext.Guests.FirstOrDefault(g => g.GuestId == guestId);
            if (guest != null)
            {
                _appDbContext.Guests.Remove(guest);
                _appDbContext.SaveChanges();
            }
        }
        public void AddBooking(Booking booking)
        {
            _appDbContext.Bookings.Add(booking);
            _appDbContext.SaveChanges();
        }

        public Room GetRoomById(int roomId)
        {
            return _appDbContext.Rooms.FirstOrDefault(r => r.RoomId == roomId);
        }

        public List<Room> GetAvailableRooms(DateTime startDate, DateTime endDate, int guestCount)
        {
            return _appDbContext.Rooms
                .Where(room => room.IsAvailable &&
                               room.TotalPeople >= guestCount &&
                               !_appDbContext.Bookings.Any(b => b.RoomId == room.RoomId &&
                                                               b.CheckInDate < endDate &&
                                                               b.CheckOutDate > startDate))
                .ToList();
        }


    }
}
