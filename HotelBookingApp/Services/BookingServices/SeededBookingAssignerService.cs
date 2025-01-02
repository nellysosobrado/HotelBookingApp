using HotelBookingApp.Data;
using HotelBookingApp.Entities;
using System;
using System.Linq;

namespace HotelBookingApp.Services.BookingServices
{
    public static class SeededBookingAssignerService
    {
        public static void AssignRoomsToExistingGuests(AppDbContext context)
        {
            var guests = context.Guests.ToList();
            if (!guests.Any())
            {
                Console.WriteLine("No guests found in the database.");
                return;
            }

            foreach (var guest in guests)
            {
                var existingBooking = context.Bookings.FirstOrDefault(b => b.GuestId == guest.GuestId);
                if (existingBooking != null)
                {
                    Console.WriteLine(new string('-', 100));
                    Console.WriteLine($"Booking for {guest.FirstName} {guest.LastName} has been created");


                    continue;
                }

                var availableRoom = context.Rooms
                    .FirstOrDefault(r => r.IsAvailable && !context.Bookings
                        .Any(b => b.RoomId == r.RoomId &&
                                  b.CheckInDate < DateTime.Now.AddDays(3) &&
                                  b.CheckOutDate > DateTime.Now));

                if (availableRoom == null)
                {
                    Console.WriteLine($"No available rooms for guest {guest.FirstName} {guest.LastName}.");
                    continue;
                }


                var newBooking = new Booking
                {
                    GuestId = guest.GuestId,
                    RoomId = availableRoom.RoomId,
                    CheckInDate = DateTime.Now.AddDays(1),
                    CheckOutDate = DateTime.Now.AddDays(4),
                    IsCheckedIn = false,
                    IsCheckedOut = false,
                    BookingCompleted = false
                };


                availableRoom.IsAvailable = false;

                context.Bookings.Add(newBooking);
                context.SaveChanges();
                Console.WriteLine($"Booking created for guest {guest.FirstName} {guest.LastName} in room {availableRoom.RoomId}.");
            }
            Console.WriteLine(new string('-', 100));
            Console.WriteLine("Notification: 4 Seeded guests, 4 Seeded rooms has been created as test subjects.");
        }
    }
}