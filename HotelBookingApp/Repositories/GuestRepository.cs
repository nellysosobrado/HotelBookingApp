using HotelBookingApp.Data;
using System.Collections.Generic;
using System.Linq;

namespace HotelBookingApp.Repositories
{
    public class GuestRepository
    {
        private readonly AppDbContext _context;

        public GuestRepository(AppDbContext context)
        {
            _context = context;
        }

        public List<Guest> GetAllGuests()
        {
            return _context.Guests.ToList();
        }

        public Guest GetGuestById(int guestId)
        {
            return _context.Guests.FirstOrDefault(g => g.GuestId == guestId);
        }

        public void AddGuest(Guest guest)
        {
            _context.Guests.Add(guest);
            _context.SaveChanges();
        }

        public void UpdateGuest(Guest guest)
        {
            _context.SaveChanges();
        }

        public List<object> GetGuestsWithBookings()
        {
            return _context.Guests
                .GroupJoin(
                    _context.Bookings,
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

    }
}
