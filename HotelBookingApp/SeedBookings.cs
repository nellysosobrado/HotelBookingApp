//using Bogus;
//using HotelBookingApp.Data;
//using HotelBookingApp;
//using HotelBookingApp.Entities;
//using System.Linq;

//public static class SeedData
//{
//    public static void SeedBookings(AppDbContext context)
//    {
//        if (context.Bookings.Any())
//        {
//            Console.WriteLine("Bookings already exist. No seed data added.");
//            return;
//        }

//        // Skapa dynamiska gäster
//        var guestFaker = new Faker<Guest>()
//            .RuleFor(g => g.FirstName, f => f.Name.FirstName())
//            .RuleFor(g => g.LastName, f => f.Name.LastName())
//            .RuleFor(g => g.Email, f => f.Internet.Email())
//            .RuleFor(g => g.PhoneNumber, f => f.Phone.PhoneNumber());

//        var guests = guestFaker.Generate(10); // Generera 10 gäster
//        context.Guests.AddRange(guests);

//        // Skapa dynamiska rum
//        var roomFaker = new Faker<Room>()
//            .RuleFor(r => r.Type, f => f.PickRandom(new[] { "Single", "Double" }))
//            .RuleFor(r => r.ExtraBeds, f => f.Random.Int(0, 2))
//            .RuleFor(r => r.IsAvailable, f => f.Random.Bool())
//            .RuleFor(r => r.PricePerNight, f => f.Random.Int(1000, 5000))
//            .RuleFor(r => r.SizeInSquareMeters, f => f.Random.Int(20, 100));

//        var rooms = roomFaker.Generate(10); // Generera 10 rum
//        context.Rooms.AddRange(rooms);

//        // Skapa dynamiska bokningar
//        var bookingFaker = new Faker<Booking>()
//            .RuleFor(b => b.GuestId, f => f.PickRandom(guests).GuestId)
//            .RuleFor(b => b.RoomId, f => f.PickRandom(rooms).RoomId)
//            .RuleFor(b => b.CheckInDate, f => f.Date.Recent())
//            .RuleFor(b => b.CheckOutDate, (f, b) => (b.CheckInDate ?? DateTime.Now).AddDays(f.Random.Int(1, 10)))
//            .RuleFor(b => b.IsCheckedIn, f => f.Random.Bool())
//            .RuleFor(b => b.IsCheckedOut, (f, b) => b.IsCheckedIn && f.Random.Bool());

//        var bookings = bookingFaker.Generate(10); // Generera 10 bokningar
//        context.Bookings.AddRange(bookings);

//        context.SaveChanges();
//        Console.WriteLine("Seed data for bookings added successfully.");
//    }
//}
