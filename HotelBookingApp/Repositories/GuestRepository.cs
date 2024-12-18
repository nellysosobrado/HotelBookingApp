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

        public void RegisterNewGuestWithBooking(Guest newGuest, Booking newBooking, Invoice newInvoice)
        {
            // Lägg till gästen i databasen
            _appDbContext.Guests.Add(newGuest);
            _appDbContext.SaveChanges(); // Detta sätter GuestId på newGuest

            if (newBooking != null)
            {
                newBooking.GuestId = newGuest.GuestId; // Koppla bokningen till gästen
                _appDbContext.Bookings.Add(newBooking);
                _appDbContext.SaveChanges(); // Detta sätter BookingId på newBooking

                if (newInvoice != null)
                {
                    newInvoice.BookingId = newBooking.BookingId; // Koppla fakturan till bokningen
                    _appDbContext.Invoices.Add(newInvoice);
                    _appDbContext.SaveChanges();
                }
            }
        }
        public decimal CalculateTotalAmount(Booking booking)
        {
            var room = _appDbContext.Rooms.FirstOrDefault(r => r.RoomId == booking.RoomId);
            if (room == null)
            {
                throw new Exception("Room not found.");
            }

            if (!booking.CheckInDate.HasValue || !booking.CheckOutDate.HasValue)
            {
                throw new Exception("Check-in or check-out date is missing.");
            }

            var duration = (booking.CheckOutDate.Value - booking.CheckInDate.Value).Days;
            if (duration <= 0)
            {
                throw new Exception("Invalid booking duration.");
            }

            return room.PricePerNight * duration;
        }


        public void AddInvoice(Invoice invoice)
        {
            _appDbContext.Invoices.Add(invoice);
            _appDbContext.SaveChanges();
        }

     
    }
}
