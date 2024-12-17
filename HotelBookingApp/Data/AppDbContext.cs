using HotelBookingApp.Entities;
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
            DateTime today = DateTime.Now.Date;
            DateTime futureCheckInDate = today.AddDays(1);
            DateTime futureCheckOutDate = futureCheckInDate.AddDays(3);

            modelBuilder.Entity<Room>().HasData(
                new Room { RoomId = 1, Type = "Single", ExtraBeds = 0, IsAvailable = false, PricePerNight = 1500, SizeInSquareMeters = 20 },
                new Room { RoomId = 2, Type = "Double", ExtraBeds = 1, IsAvailable = false, PricePerNight = 3500, SizeInSquareMeters = 80 },
                new Room { RoomId = 3, Type = "Single", ExtraBeds = 0, IsAvailable = true, PricePerNight = 2000, SizeInSquareMeters = 25 },
                new Room { RoomId = 4, Type = "Double", ExtraBeds = 2, IsAvailable = true, PricePerNight = 4000, SizeInSquareMeters = 90 }
            );

            modelBuilder.Entity<Guest>().HasData(
                new Guest { GuestId = 1, FirstName = "p1", LastName = "l1", Email = "gmail.com1", PhoneNumber = "11111" },
                new Guest { GuestId = 2, FirstName = "p2", LastName = "l2", Email = "gmail.com2", PhoneNumber = "22222" },
                new Guest { GuestId = 3, FirstName = "p3", LastName = "l3", Email = "gmail.com3", PhoneNumber = "33333" },
                new Guest { GuestId = 4, FirstName = "p4", LastName = "l4", Email = "gmail.com4", PhoneNumber = "44444" }
            );

            modelBuilder.Entity<Booking>().HasData(
                new Booking
                {
                    BookingId = 1,
                    GuestId = 1,
                    RoomId = 1,
                    IsCheckedIn = false,
                    IsCheckedOut = false,
                    BookingStatus = false,
                    CheckInDate = futureCheckInDate,
                    CheckOutDate = futureCheckOutDate
                },
                new Booking
                {
                    BookingId = 2,
                    GuestId = 2,
                    RoomId = 2,
                    IsCheckedIn = false,
                    IsCheckedOut = false,
                    BookingStatus = false,
                    CheckInDate = futureCheckInDate,
                    CheckOutDate = futureCheckOutDate
                },
                new Booking
                {
                    BookingId = 3,
                    GuestId = 3,
                    RoomId = 3,
                    IsCheckedIn = true,
                    IsCheckedOut = false,
                    BookingStatus = false,
                    CheckInDate = today.AddDays(-2),
                    CheckOutDate = today.AddDays(1)
                },
                new Booking
                {
                    BookingId = 4,
                    GuestId = 4,
                    RoomId = 4,
                    IsCheckedIn = false,
                    IsCheckedOut = true,
                    BookingStatus = true,
                    CheckInDate = today.AddDays(-5),
                    CheckOutDate = today.AddDays(-1)
                }
            );

            modelBuilder.Entity<Invoice>().HasData(
                new Invoice
                {
                    InvoiceId = 1,
                    BookingId = 1,
                    TotalAmount = 4500,
                    IsPaid = false,
                    PaymentDeadline = futureCheckOutDate.AddDays(7)
                },
                new Invoice
                {
                    InvoiceId = 2,
                    BookingId = 2,
                    TotalAmount = 7000,
                    IsPaid = false,
                    PaymentDeadline = futureCheckOutDate.AddDays(7)
                },
                new Invoice
                {
                    InvoiceId = 3,
                    BookingId = 3,
                    TotalAmount = 6000,
                    IsPaid = false,
                    PaymentDeadline = today.AddDays(7)
                },
                new Invoice
                {
                    InvoiceId = 4,
                    BookingId = 4,
                    TotalAmount = 8000,
                    IsPaid = true,
                    PaymentDeadline = today.AddDays(-2)
                }
            );

            modelBuilder.Entity<Payment>().HasData(
                new Payment
                {
                    PaymentId = 1,
                    InvoiceId = 4,
                    PaymentDate = today.AddDays(-2),
                    AmountPaid = 8000
                }
            );

            base.OnModelCreating(modelBuilder);
        }




    }
}
