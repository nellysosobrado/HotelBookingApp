using Microsoft.EntityFrameworkCore;
using System;

namespace HotelBookingApp.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Guest> Guests { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<Payment> Payments { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Define relations for Invoice and Payment tables
            modelBuilder.Entity<Invoice>()
                .HasOne(i => i.Booking)
                .WithMany(b => b.Invoices)
                .HasForeignKey(i => i.BookingId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Payment>()
                .HasOne(p => p.Invoice)
                .WithMany(i => i.Payments)
                .HasForeignKey(p => p.InvoiceId)
                .OnDelete(DeleteBehavior.Cascade);

            // Seed-data
            SeedData(modelBuilder);
        }
        private void SeedData(ModelBuilder modelBuilder)
        {
            // Dynamiskt generera datum baserat på dagens datum
            DateTime today = DateTime.Now.Date;
            DateTime futureCheckInDate = today.AddDays(1); // Check-in startar imorgon
            DateTime futureCheckOutDate = futureCheckInDate.AddDays(3); // Tre dagar efter incheckning

            // Seed Rooms
            modelBuilder.Entity<Room>().HasData(
                new Room { RoomId = 1, Type = "Single", ExtraBeds = 0, IsAvailable = false, PricePerNight = 1500, SizeInSquareMeters = 20 },
                new Room { RoomId = 2, Type = "Double", ExtraBeds = 1, IsAvailable = false, PricePerNight = 3500, SizeInSquareMeters = 80 },
                new Room { RoomId = 3, Type = "Double", ExtraBeds = 2, IsAvailable = true, PricePerNight = 3500, SizeInSquareMeters = 70 },
                new Room { RoomId = 4, Type = "Single", ExtraBeds = 0, IsAvailable = true, PricePerNight = 1500, SizeInSquareMeters = 215 }
            );

            // Seed Guests
            modelBuilder.Entity<Guest>().HasData(
                new Guest { GuestId = 1, FirstName = "Person1", LastName = "Lastname1", Email = "234p@example.com", PhoneNumber = "234" },
                new Guest { GuestId = 2, FirstName = "Person2", LastName = "Lastname2", Email = "342p@example.com", PhoneNumber = "3453" },
                new Guest { GuestId = 3, FirstName = "Person3", LastName = "Lastname3", Email = "p234@example.com", PhoneNumber = "3453" },
                new Guest { GuestId = 4, FirstName = "Person4", LastName = "Lastname4", Email = "243p@example.com", PhoneNumber = "43" }
            );

            // Seed Bookings
            modelBuilder.Entity<Booking>().HasData(
                new Booking
                {
                    BookingId = 1,
                    GuestId = 1,
                    RoomId = 1,
                    IsCheckedIn = false,
                    IsCheckedOut = true,
                    BookingStatus = true, // test Check out!
                    CheckInDate = futureCheckInDate,
                    CheckOutDate = futureCheckOutDate
                },
                new Booking
                {
                    BookingId = 2,
                    GuestId = 2,
                    RoomId = 2,
                    IsCheckedIn = true,
                    IsCheckedOut = false,
                    BookingStatus = false, // Not completed
                    CheckInDate = futureCheckInDate,
                    CheckOutDate = futureCheckOutDate
                }
            );

            // Seed Invoices
            var invoices = new[]
            {
        new Invoice
        {
            InvoiceId = 1,
            BookingId = 1,
            TotalAmount = 1500m,
            IsPaid = false,
            PaymentDeadline = futureCheckOutDate.AddDays(7)
        },
        new Invoice
        {
            InvoiceId = 2,
            BookingId = 2,
            TotalAmount = 10500.00m,
            IsPaid = false,
            PaymentDeadline = futureCheckOutDate.AddDays(7)
        }
    };

            modelBuilder.Entity<Invoice>().HasData(invoices);

            // Seed Payments dynamically linked to invoices
            var payments = invoices.Select(invoice => new Payment
            {
                PaymentId = invoice.InvoiceId,
                InvoiceId = invoice.InvoiceId,
                PaymentDate = today,
                AmountPaid = invoice.TotalAmount
            }).ToArray();

            modelBuilder.Entity<Payment>().HasData(payments);
        }





    }
}
