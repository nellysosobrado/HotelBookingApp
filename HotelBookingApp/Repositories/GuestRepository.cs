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

        public Dictionary<int, bool> GetGuestBookingStatus()
        {
            var activeBookingGuestIds = _bookingRepository.GetActiveBookings()
                .Select(b => b.GuestId)
                .ToHashSet();

            return _appDbContext.Guests.ToDictionary(
                guest => guest.GuestId,
                guest => activeBookingGuestIds.Contains(guest.GuestId)
            );
        }

        public List<Guest> GetAllGuests()
        {
            return _appDbContext.Guests.ToList();
        }

        public void RemoveGuest(Guest guest)
        {
            _appDbContext.Guests.Remove(guest);
            _appDbContext.SaveChanges();
        }


        public void RegisterNewGuestWithBookingAndInvoice(Guest guest, Booking booking, Invoice invoice)
        {
            _appDbContext.Guests.Add(guest);
            _appDbContext.SaveChanges();

            booking.GuestId = guest.GuestId;
            _appDbContext.Bookings.Add(booking);
            _appDbContext.SaveChanges();

            invoice.BookingId = booking.BookingId;
            _appDbContext.Invoices.Add(invoice);
            _appDbContext.SaveChanges();
        }
        public HashSet<DateTime> GetBookedDates(int month, int year, string roomType)
        {
            var bookings = _appDbContext.Bookings
                .Include(b => b.Room)
                .Where(b => b.Room.Type == roomType &&
                            b.CheckInDate.HasValue && b.CheckOutDate.HasValue &&
                            b.CheckInDate.Value.Month == month &&
                            b.CheckInDate.Value.Year == year)
                .ToList();

            HashSet<DateTime> bookedDates = new HashSet<DateTime>();
            foreach (var booking in bookings)
            {
                for (DateTime date = booking.CheckInDate.Value; date <= booking.CheckOutDate.Value; date = date.AddDays(1))
                {
                    bookedDates.Add(date);
                }
            }

            return bookedDates;
        }
        public void AddGuest(Guest guest)
        {
            _appDbContext.Guests.Add(guest);
            _appDbContext.SaveChanges();
        }

        public void RegisterNewGuestWithBooking(Guest guest, Booking booking)
        {
            _appDbContext.Guests.Add(guest);
            _appDbContext.SaveChanges();

            booking.GuestId = guest.GuestId;
            _appDbContext.Bookings.Add(booking);
            _appDbContext.SaveChanges();
        }


        public Guest GetGuestById(int guestId)
        {
            return _appDbContext.Guests.FirstOrDefault(g => g.GuestId == guestId);
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
                            b.BookingCompleted
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

        public List<Room> GetAvailableRooms(DateTime startDate, DateTime endDate, string roomType)
        {
            var allRooms = _roomRepository.GetAllRooms()
                .Where(r => r.Type.Equals(roomType, StringComparison.OrdinalIgnoreCase))
                .ToList();

            Console.WriteLine($"All rooms for type '{roomType}': {string.Join(", ", allRooms.Select(r => r.RoomId))}");

            var bookedRoomIds = _bookingRepository.GetAllBookings()
                .Where(b => b.Room.Type.Equals(roomType, StringComparison.OrdinalIgnoreCase))
                .Where(b => b.CheckInDate <= endDate && b.CheckOutDate >= startDate)
                .Select(b => b.RoomId)
                .ToHashSet();

            Console.WriteLine($"Booked room IDs for type '{roomType}': {string.Join(", ", bookedRoomIds)}");

            var availableRooms = allRooms.Where(r => !bookedRoomIds.Contains(r.RoomId)).ToList();

            Console.WriteLine($"Available room IDs for type '{roomType}': {string.Join(", ", availableRooms.Select(r => r.RoomId))}");

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
