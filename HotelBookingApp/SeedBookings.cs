using System;

namespace HotelBookingApp
{
    public static class SeedData
    {
        public static void SeedBookings(AppDbContext context)
        {
            // Kontrollera om bokningar redan finns
            if (context.Bookings.Any())
            {
                Console.WriteLine("Bookings already exist. No seed data added.");
                return;
            }

            // Koppla gäster till rum och skapa bokningar
            var guestAlice = context.Guests.FirstOrDefault(g => g.FirstName == "Alice");
            var room101 = context.Rooms.FirstOrDefault(r => r.RoomId == 101);

            if (guestAlice != null && room101 != null)
            {
                context.Bookings.Add(new Booking
                {
                    GuestId = guestAlice.GuestId,
                    RoomId = room101.RoomId,
                    CheckInDate = DateTime.Now.AddDays(-3),
                    CheckOutDate = DateTime.Now.AddDays(2),
                    IsCheckedIn = true,
                    IsCheckedOut = false
                });
            }

            var guestBob = context.Guests.FirstOrDefault(g => g.FirstName == "Bob");
            var room102 = context.Rooms.FirstOrDefault(r => r.RoomId == 102);

            if (guestBob != null && room102 != null)
            {
                context.Bookings.Add(new Booking
                {
                    GuestId = guestBob.GuestId,
                    RoomId = room102.RoomId,
                    CheckInDate = DateTime.Now.AddDays(-5),
                    CheckOutDate = DateTime.Now.AddDays(-1),
                    IsCheckedIn = false,
                    IsCheckedOut = true
                });
            }

            context.SaveChanges();
            Console.WriteLine("Seed data for bookings added successfully.");
        }
    }
}
