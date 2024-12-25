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
        private readonly RoomRepository _roomRepository;
        private readonly BookingRepository _bookingRepository;

        public GuestRepository(AppDbContext context, RoomRepository roomRepository, BookingRepository bookingRepository)
        {
            _appDbContext = context;
            _roomRepository = roomRepository;
            _bookingRepository = bookingRepository;
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
            // Hämta alla rum
            var allRooms = _roomRepository.GetAllRooms();

            var bookedRooms = _bookingRepository.GetAllBookings()
                .Where(b => b.CheckInDate.HasValue && b.CheckOutDate.HasValue)
                .Where(b =>
                    (b.CheckInDate.Value.Date < endDate && b.CheckOutDate.Value.Date >= startDate)) 
                .Select(b => b.RoomId)  
                .ToList();

            var availableAfterCheckoutRooms = _bookingRepository.GetAllBookings()
                .Where(b => b.CheckOutDate.HasValue && b.CheckOutDate.Value.Date < startDate)
                .Select(b => b.RoomId)
                .ToList();

            var availableRooms = allRooms.Where(room =>
                !bookedRooms.Contains(room.RoomId) || availableAfterCheckoutRooms.Contains(room.RoomId) 
                && room.TotalPeople >= guestCount).ToList();

            return availableRooms;
        }



        public void RegisterNewGuestWithBooking(Guest guest, Booking booking, Invoice invoice)
        {
           
            _appDbContext.Guests.Add(guest);
            _appDbContext.SaveChanges();

           
            booking.GuestId = guest.GuestId;
            _appDbContext.Bookings.Add(booking);
            _appDbContext.SaveChanges(); 

            if (invoice != null)
            {
                invoice.BookingId = booking.BookingId; 
                _appDbContext.Invoices.Add(invoice);
            }


            _appDbContext.SaveChanges();
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
