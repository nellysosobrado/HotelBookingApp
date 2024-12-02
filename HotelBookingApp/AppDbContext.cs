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
            modelBuilder.Entity<Room>().HasData(
                new Room { RoomId = 1, Type = "Single", ExtraBeds = 0, IsAvailable = false , PricePerNight = 1500, SizeInSquareMeters=20},
                new Room { RoomId = 2, Type = "Double", ExtraBeds = 1, IsAvailable = false, PricePerNight = 3500, SizeInSquareMeters = 80 },
                new Room { RoomId = 3, Type = "Double", ExtraBeds = 2, IsAvailable = true, PricePerNight = 3500, SizeInSquareMeters = 70 },
                new Room { RoomId = 4, Type = "Single", ExtraBeds = 0, IsAvailable = true,PricePerNight = 1500, SizeInSquareMeters = 215 }
            );

            modelBuilder.Entity<Guest>().HasData(
                new Guest { GuestId = 1, FirstName = "Person2", LastName = "lastname1", Email = "234p@example.com", PhoneNumber = "234" },
                new Guest { GuestId = 2, FirstName = "Person2", LastName = "lastname2", Email = "342p@example.com", PhoneNumber = "3453" },
                new Guest { GuestId = 3, FirstName = "Person3", LastName = "lastname3", Email = "p234@example.com", PhoneNumber = "3453" },
                new Guest { GuestId = 4, FirstName = "Person4", LastName = "lastname4", Email = "243p@example.com", PhoneNumber = "43" }
            );
            
            modelBuilder.Entity<Booking>().HasData(
                new Booking
                {
                    BookingId = 1,
                    GuestId = 1,
                    RoomId = 1,
                    IsCheckedIn = true,
                    IsCheckedOut = true,
                    BookingStatus = true, // Completed
                    CheckInDate = DateTime.Now.AddDays(-5),
                    CheckOutDate = DateTime.Now.AddDays(-1)
                },
                new Booking
                {
                    BookingId = 2,
                    GuestId = 2,
                    RoomId = 2,
                    IsCheckedIn = true,
                    IsCheckedOut = false,
                    BookingStatus = false, // Not completed
                    CheckInDate = DateTime.Now.AddDays(-2),
                    CheckOutDate = null
                }
            );

            modelBuilder.Entity<Invoice>().HasData(
                new Invoice
                {
                    InvoiceId = 1,
                    BookingId = 1,
                    TotalAmount = 500.00m,
                    IsPaid = true,
                    PaymentDeadline = DateTime.Now.AddDays(-3)
                },
                new Invoice
                {
                    InvoiceId = 2,
                    BookingId = 2,
                    TotalAmount = 300.00m,
                    IsPaid = false,
                    PaymentDeadline = DateTime.Now.AddDays(5)
                }
            );

            modelBuilder.Entity<Payment>().HasData(
                new Payment
                {
                    PaymentId = 1,
                    InvoiceId = 1,
                    PaymentDate = DateTime.Now.AddDays(-4),
                    AmountPaid = 500.00m
                }
            );
        }






    }
}
