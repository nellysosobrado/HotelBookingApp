using System;
using System.Linq;

namespace HotelBookingApp
{
    public static class ExistingGuests
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
                // Kontrollera om gästen redan har en bokning
                var existingBooking = context.Bookings.FirstOrDefault(b => b.GuestId == guest.GuestId);
                if (existingBooking != null)
                {
                    Console.WriteLine($"Guest {guest.FirstName} {guest.LastName} already has a booking.");
                    continue;
                }

                // Hämta ett ledigt rum
                var availableRoom = context.Rooms.FirstOrDefault(r => r.IsAvailable);
                if (availableRoom == null)
                {
                    Console.WriteLine($"No available rooms for guest {guest.FirstName} {guest.LastName}.");
                    continue;
                }

                // Skapa en ny bokning
                var newBooking = new Booking
                {
                    GuestId = guest.GuestId,
                    RoomId = availableRoom.RoomId,
                    CheckInDate = DateTime.Now.AddDays(-1),
                    CheckOutDate = DateTime.Now.AddDays(3),
                    IsCheckedIn = true,
                    IsCheckedOut = false
                };

                // Uppdatera rummets status till upptaget
                availableRoom.IsAvailable = false;

                // Lägg till bokningen i databasen och uppdatera rummets status
                context.Bookings.Add(newBooking);
                context.SaveChanges(); // Spara ändringar omedelbart
                Console.WriteLine($"Booking created for guest {guest.FirstName} {guest.LastName} in room {availableRoom.RoomId}.");
            }

            Console.WriteLine("All bookings for existing guests have been processed.");
        }
    }
}
