using HotelBookingApp;
using HotelBookingApp.Data;
using HotelBookingApp.Entities;
using System;
using System.Linq;

namespace HotelBookingApp;
public class BookingRepository
{
    private readonly AppDbContext _appDbContext;

    public BookingRepository(AppDbContext context)
    {
        _appDbContext = context;
    }

    public Booking GetBookingById(int bookingId)
    {
        return _appDbContext.Bookings.FirstOrDefault(b => b.BookingId == bookingId);
    }

    public Booking GetActiveBookingByGuestId(int guestId)
    {
        return _appDbContext.Bookings.FirstOrDefault(b => b.GuestId == guestId && b.IsCheckedIn && !b.IsCheckedOut);
    }

    public Guest GetGuestById(int guestId)
    {
        return _appDbContext.Guests.FirstOrDefault(g => g.GuestId == guestId);
    }

    public Room GetRoomById(int roomId)
    {
        return _appDbContext.Rooms.FirstOrDefault(r => r.RoomId == roomId);
    }

    public Invoice GetInvoiceByBookingId(int bookingId)
    {
        return _appDbContext.Invoices.FirstOrDefault(i => i.BookingId == bookingId);
    }

    public void AddInvoice(Invoice invoice)
    {
        _appDbContext.Invoices.Add(invoice);
        SaveChanges();
    }

    public void AddPayment(Payment payment)
    {
        _appDbContext.Payments.Add(payment);
        SaveChanges();
    }

    public void UpdateBooking(Booking booking)
    {
        _appDbContext.Bookings.Update(booking);
        SaveChanges();
    }

    public void UpdateInvoice(Invoice invoice)
    {
        _appDbContext.Invoices.Update(invoice);
        SaveChanges();
    }
    public void UpdateBookingAndInvoice(Booking booking, Invoice invoice)
    {
        _appDbContext.Bookings.Update(booking);
        _appDbContext.Invoices.Update(invoice);
        SaveChanges();
    }


    public decimal CalculateTotalAmount(Booking booking)
    {
        var room = GetRoomById(booking.RoomId);
        if (room == null)
        {
            throw new Exception("Room not found.");
        }

        var checkOutDate = booking.CheckOutDate ?? DateTime.Now;
        var duration = (checkOutDate.Date - booking.CheckInDate.Value.Date).Days;

        if (duration <= 0)
        {
            throw new Exception("Invalid duration for booking. Check-out date must be after check-in date.");
        }

        return duration * room.PricePerNight;
    }

    private void SaveChanges()
    {
        try
        {
            _appDbContext.SaveChanges();
        }
        catch (Exception ex)
        {
            throw new Exception($"An error occurred while saving changes to the database: {ex.Message}");
        }
    }
}
