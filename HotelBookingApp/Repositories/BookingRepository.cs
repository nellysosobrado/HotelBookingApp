using HotelBookingApp;
using HotelBookingApp.Data;
using HotelBookingApp.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HotelBookingApp.Repositories
{
    public class BookingRepository
    {
        private readonly AppDbContext _appDbContext;

        public BookingRepository(AppDbContext context)
        {
            _appDbContext = context;
        }

        public Booking GetBookingById(int bookingId)
        {
            return _appDbContext.Bookings
                .Include(b => b.Guest)
                .Include(b => b.Room)
                .FirstOrDefault(b => b.BookingId == bookingId);
        }

        public Booking GetActiveBookingByGuestId(int guestId)
        {
            return _appDbContext.Bookings
                .Include(b => b.Room)
                .Include(b => b.Guest)
                .FirstOrDefault(b => b.GuestId == guestId && b.IsCheckedIn && !b.IsCheckedOut);
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

        public List<Room> GetAvailableRooms(DateTime startDate, DateTime endDate, int guestCount)
        {
            return _appDbContext.Rooms
                .Where(room => room.TotalPeople >= guestCount &&
                               !_appDbContext.Bookings.Any(b => b.RoomId == room.RoomId &&
                                                               b.CheckInDate <= endDate &&
                                                               b.CheckOutDate >= startDate))
                .ToList();
        }

        public List<Booking> GetAllBookings()
        {
            return _appDbContext.Bookings
                .Include(b => b.Guest)
                .Include(b => b.Room)
                .ToList();
        }

        public List<Booking> GetNonPaidBookings()
        {
            return _appDbContext.Invoices
                .Where(i => !i.IsPaid)
                .Join(_appDbContext.Bookings,
                    invoice => invoice.BookingId,
                    booking => booking.BookingId,
                    (invoice, booking) => booking)
                .Include(b => b.Guest)
                .ToList();
        }

        public List<Booking> GetPaidBookings()
        {
            return _appDbContext.Invoices
                .Where(i => i.IsPaid)
                .Join(_appDbContext.Bookings,
                    invoice => invoice.BookingId,
                    booking => booking.BookingId,
                    (invoice, booking) => booking)
                .Include(b => b.Guest)
                .ToList();
        }

        public void DeleteBooking(Booking booking)
        {
            _appDbContext.Bookings.Remove(booking);
            SaveChanges();
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
}
