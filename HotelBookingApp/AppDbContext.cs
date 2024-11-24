using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBookingApp
{
    public class AppDbContext : DbContext
    {
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Guest> Guests { get; set; }
        public DbSet<Booking> Bookings { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Seed-data!!!
            modelBuilder.Entity<Room>().HasData(
                new Room { RoomId = 1, Type = "Single", ExtraBeds = 0 },
                new Room { RoomId = 2, Type = "Double", ExtraBeds = 1 },
                new Room { RoomId = 3, Type = "Double", ExtraBeds = 2 },
                new Room { RoomId = 4, Type = "Single", ExtraBeds = 0 }
            );

            modelBuilder.Entity<Guest>().HasData(
                new Guest { GuestId = 1, FirstName = "Alice", LastName = "Smith", Email = "alice@example.com", PhoneNumber = "1234567890" },
                new Guest { GuestId = 2, FirstName = "Bob", LastName = "Johnson", Email = "bob@example.com", PhoneNumber = "2345678901" },
                new Guest { GuestId = 3, FirstName = "Charlie", LastName = "Brown", Email = "charlie@example.com", PhoneNumber = "3456789012" },
                new Guest { GuestId = 4, FirstName = "Diana", LastName = "Green", Email = "diana@example.com", PhoneNumber = "4567890123" }
            );
        }
    }
}
