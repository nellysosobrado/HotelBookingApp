using Microsoft.EntityFrameworkCore;
using System;

namespace HotelBookingApp
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
            // Rooms
            modelBuilder.Entity<Room>().HasData(
                new Room { RoomId = 1, Type = "Single", ExtraBeds = 0 },
                new Room { RoomId = 2, Type = "Double", ExtraBeds = 1 },
                new Room { RoomId = 3, Type = "Double", ExtraBeds = 2 },
                new Room { RoomId = 4, Type = "Single", ExtraBeds = 0 }
            );

            // Guests
            modelBuilder.Entity<Guest>().HasData(
                new Guest { GuestId = 1, FirstName = "Alice", LastName = "Smith", Email = "alice@example.com", PhoneNumber = "1234567890" },
                new Guest { GuestId = 2, FirstName = "Bob", LastName = "Johnson", Email = "bob@example.com", PhoneNumber = "2345678901" },
                new Guest { GuestId = 3, FirstName = "Charlie", LastName = "Brown", Email = "charlie@example.com", PhoneNumber = "3456789012" },
                new Guest { GuestId = 4, FirstName = "Diana", LastName = "Green", Email = "diana@example.com", PhoneNumber = "4567890123" }
            );

            // Bookings
            modelBuilder.Entity<Booking>().HasData(
                new Booking { BookingId = 1, GuestId = 1, RoomId = 1, IsCheckedIn = false, IsCheckedOut = false, BookingStatus = false },
                new Booking { BookingId = 2, GuestId = 2, RoomId = 2, IsCheckedIn = true, IsCheckedOut = false, BookingStatus = false }
            );

            // Invoices
            modelBuilder.Entity<Invoice>().HasData(
                new Invoice { InvoiceId = 1, BookingId = 1, TotalAmount = 500.00m, IsPaid = false, PaymentDeadline = DateTime.Now.AddDays(7) },
                new Invoice { InvoiceId = 2, BookingId = 2, TotalAmount = 300.00m, IsPaid = true, PaymentDeadline = DateTime.Now.AddDays(10) }
            );

            // Payments
            modelBuilder.Entity<Payment>().HasData(
                new Payment { PaymentId = 1, InvoiceId = 2, PaymentDate = DateTime.Now.AddDays(-2), AmountPaid = 300.00m }
            );
        }
    }
}
