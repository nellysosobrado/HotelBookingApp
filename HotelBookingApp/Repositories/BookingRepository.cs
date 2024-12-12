using HotelBookingApp.Data;
using HotelBookingApp.Entities;
using HotelBookingApp;
using System;
using System.Linq;

public class BookingRepository
{
    private readonly AppDbContext _context;

    public BookingRepository(AppDbContext context)
    {
        _context = context;
    }

    public Booking GetBookingById(int bookingId)
    {
        return _context.Bookings.FirstOrDefault(b => b.BookingId == bookingId);
    }

    public Guest GetGuestById(int guestId)
    {
        return _context.Guests.FirstOrDefault(g => g.GuestId == guestId);
    }

    public Room GetRoomById(int roomId)
    {
        return _context.Rooms.FirstOrDefault(r => r.RoomId == roomId);
    }

    public void UpdateBooking(Booking booking)
    {
        _context.SaveChanges();
    }
}
