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
        private readonly List<Booking> _bookings; 

        public BookingRepository(AppDbContext context, List<Booking> bookings)
        {
            _appDbContext = context;
            _bookings = _appDbContext.Bookings.ToList();
        }
        public IEnumerable<Booking> GetEditableBookings()
        {
            var canceledBookingIds = _appDbContext.CanceledBookingsHistory
                .Select(cb => cb.BookingId)
                .ToHashSet();

            return _appDbContext.Bookings
                .Where(b => !canceledBookingIds.Contains(b.BookingId) &&
                            !b.BookingCompleted &&
                            b.CheckInDate.HasValue &&
                            !b.IsCheckedOut)
                .Include(b => b.Guest)
                .Include(b => b.Room)
                .ToList();
        }

        public IEnumerable<Booking> GetActiveBookings()
        {
            var canceledBookingIds = _appDbContext.CanceledBookingsHistory
                .Select(cb => cb.BookingId)
                .ToHashSet();

            return _appDbContext.Bookings
                .Where(b => !canceledBookingIds.Contains(b.BookingId) && !b.IsCheckedOut)
                .Include(b => b.Guest)
                .Include(b => b.Room)
                .ToList();
        }

        public IEnumerable<Booking> GetCompletedBookings()
        {
            return _appDbContext.Bookings
                .Where(b => b.IsCheckedOut)
                .Include(b => b.Guest)
                .Include(b => b.Room)
                .ToList();
        }
        public IEnumerable<Booking> GetRemovedBookings()
        {
            var canceledBookingIds = _appDbContext.CanceledBookingsHistory
                .Select(cb => cb.BookingId)
                .ToHashSet();

            return _appDbContext.Bookings
                .Where(b => canceledBookingIds.Contains(b.BookingId))
                .Include(b => b.Guest)
                .Include(b => b.Room)
                .ToList();
        }



        public void CancelBooking(Booking booking, string reason)
        {
            var canceledBooking = new CanceledBookingHistory
            {
                BookingId = booking.BookingId,
                GuestName = $"{booking.Guest.FirstName} {booking.Guest.LastName}",
                RoomId = booking.RoomId,
                CanceledDate = DateTime.Now,
                Reason = reason
            };

            _appDbContext.CanceledBookingsHistory.Add(canceledBooking);
            _appDbContext.Bookings.Remove(booking); 
            _appDbContext.SaveChanges();
        }

        public void CancelUnpaidBookings(IEnumerable<Booking> bookings)
        {
            foreach (var booking in bookings)
            {
                var canceledBooking = new CanceledBookingHistory
                {
                    BookingId = booking.BookingId,
                    GuestName = $"{booking.Guest.FirstName} {booking.Guest.LastName}",
                    RoomId = booking.RoomId,
                    CanceledDate = DateTime.Now,
                    Reason = "Invoice unpaid past deadline",
                    IsCanceled = false
                };

                _appDbContext.CanceledBookingsHistory.Add(canceledBooking);

                booking.IsCanceled = true;
                _appDbContext.Bookings.Update(booking);
            }

            _appDbContext.SaveChanges();
        }


        public void SoftDeleteBooking(Booking booking)
        {
            booking.IsCanceled = true;
            _appDbContext.Bookings.Update(booking);
            _appDbContext.SaveChanges();
        }

        public IEnumerable<CanceledBookingHistory> GetCanceledBookingsHistory()
        {
    return _appDbContext.CanceledBookingsHistory
        .Where(cb => !cb.IsCanceled)
        .ToList();
        }

        public void AddCanceledBooking(Booking booking, string reason)
        {
            var canceledBooking = new CanceledBookingHistory
            {
                BookingId = booking.BookingId,
                GuestName = $"{booking.Guest.FirstName} {booking.Guest.LastName}",
                RoomId = booking.RoomId,
                CanceledDate = DateTime.Now,
                Reason = reason
            };

            _appDbContext.CanceledBookingsHistory.Add(canceledBooking);
            _appDbContext.SaveChanges();
        }

        public IEnumerable<Booking> GetBookingsByGuestId(int guestId, bool includeRoom = false)
        {
            var query = _appDbContext.Bookings.Where(b => b.GuestId == guestId);

            if (includeRoom)
            {
                query = query.Include(b => b.Room);
            }

            return query.ToList();
        }


        public void AddBooking(Booking booking)
        {
            _bookings.Add(booking);
        }
        public void UpdateGuest(Guest guest)
        {
            _appDbContext.Guests.Update(guest);
            SaveChanges();
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
                .Include(b => b.Invoices)    
                    .ThenInclude(i => i.Payments) 
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
        public bool CancelBookingById(int bookingId)
        {
            var booking = GetBookingById(bookingId);

            if (booking == null)
            {
                return false; 
            }

            booking.BookingCompleted = false;

            var room = _appDbContext.Rooms.FirstOrDefault(r => r.RoomId == booking.RoomId);
            if (room != null)
            {
                room.IsAvailable = true;
            }

            _appDbContext.SaveChanges();
            return true; 
        }

        public void SaveChanges()
        {
            try
            {
                _appDbContext.SaveChanges();
            }
            catch (DbUpdateException dbEx)
            {
                var innerMessage = dbEx.InnerException?.Message ?? "No inner exception details available";
                throw new Exception($"An error occurred while saving changes to the database: {innerMessage}", dbEx);
            }
            catch (Exception ex)
            {
                throw new Exception($"An unexpected error occurred: {ex.Message}", ex);
            }
        }


        public List<Booking> GetExpiredUnpaidBookings()
        {
            var tenDaysAgo = DateTime.Now.AddDays(-10);

            return _appDbContext.Bookings
                .Where(b => b.BookingCompleted == false
                            && b.Invoices.Any(i => !i.IsPaid && i.PaymentDeadline < tenDaysAgo))
                .ToList();
        }

        public void CancelBooking(Booking booking)
        {
            booking.BookingCompleted = false;
            booking.IsCheckedIn = false;
            booking.IsCheckedOut = true;

            var room = _appDbContext.Rooms.FirstOrDefault(r => r.RoomId == booking.RoomId);
            if (room != null)
            {
                room.IsAvailable = true;
            }

            _appDbContext.SaveChanges();
        }

        public void CheckInGuest(Booking booking)
        {
            booking.IsCheckedIn = true;
            booking.CheckInDate = DateTime.Now;
            UpdateBooking(booking);
        }

        public Invoice GenerateInvoiceForBooking(Booking booking)
        {
            var invoice = new Invoice
            {
                BookingId = booking.BookingId,
                TotalAmount = CalculateTotalAmount(booking),
                IsPaid = false,
                PaymentDeadline = DateTime.Now.AddDays(7)
            };

            AddInvoice(invoice);
            return invoice;
        }

        public void ProcessPayment(Invoice invoice, decimal amount)
        {
            var payment = new Payment
            {
                InvoiceId = invoice.InvoiceId,
                PaymentDate = DateTime.Now,
                AmountPaid = amount
            };

            _appDbContext.Payments.Add(payment);

            invoice.IsPaid = true;
            _appDbContext.Invoices.Update(invoice);

            _appDbContext.SaveChanges();
        }


        public void UpdateRoom(Room room)
        {
            _appDbContext.Rooms.Update(room);
            _appDbContext.SaveChanges();
        }
        public IEnumerable<Booking> GetCanceledBookings()
        {
            return _appDbContext.Bookings
                .Where(b => !b.BookingCompleted) 
                .ToList();
        }

    }
}
