﻿using Bogus;
using HotelBookingApp.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

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
            var today = DateTime.Now.Date;

            var guestFaker = new Faker<Guest>()
                .RuleFor(g => g.GuestId, f => f.IndexFaker + 1)
                .RuleFor(g => g.FirstName, f => f.Name.FirstName())
                .RuleFor(g => g.LastName, f => f.Name.LastName())
                .RuleFor(g => g.Email, (f, g) => $"{g.FirstName.ToLower()}.{g.LastName.ToLower()}{f.UniqueIndex}@example.com")
                .RuleFor(g => g.PhoneNumber, f => f.Phone.PhoneNumber());
            var guests = guestFaker.Generate(4);

            var roomFaker = new Faker<Room>()
                .RuleFor(r => r.RoomId, f => f.IndexFaker + 1)
                .RuleFor(r => r.Type, f => f.PickRandom(new[] { "Single", "Double" }))
                .RuleFor(r => r.ExtraBeds, f => f.Random.Int(0, 2))
                .RuleFor(r => r.IsAvailable, f => true)
                .RuleFor(r => r.PricePerNight, f => f.Random.Int(1000, 5000))
                .RuleFor(r => r.SizeInSquareMeters, f => f.Random.Int(20, 100));
            var rooms = roomFaker.Generate(4);

            var bookingFaker = new Faker<Booking>()
                .RuleFor(b => b.BookingId, f => f.IndexFaker + 1)
                .RuleFor(b => b.GuestId, (f, b) => guests[b.BookingId - 1].GuestId)
                .RuleFor(b => b.RoomId, (f, b) => rooms[b.BookingId - 1].RoomId)
                .RuleFor(b => b.CheckInDate, f => today.AddDays(f.Random.Int(1, 5)))
                .RuleFor(b => b.CheckOutDate, (f, b) => b.CheckInDate.HasValue
                    ? b.CheckInDate.Value.AddDays(f.Random.Int(1, 5))
                    : today.AddDays(f.Random.Int(1, 5)))
                .RuleFor(b => b.IsCheckedIn, f => false)
                .RuleFor(b => b.IsCheckedOut, f => false)
                .RuleFor(b => b.BookingStatus, f => false) // Not canceled by default
                .RuleFor(b => b.IsCanceled, f => false); // Add IsCanceled property

            var bookings = bookingFaker.Generate(4);

            var invoices = bookings.Select((booking, index) => new Invoice
            {
                InvoiceId = index + 1,
                BookingId = booking.BookingId,
                TotalAmount = new Random().Next(5000, 20000),
                IsPaid = false,
                PaymentDeadline = booking.CheckOutDate.HasValue
                    ? booking.CheckOutDate.Value.AddDays(7)
                    : today.AddDays(7)
            }).ToList();

            var payments = invoices.Select((invoice, index) => new Payment
            {
                PaymentId = index + 1,
                InvoiceId = invoice.InvoiceId,
                PaymentDate = today.AddDays(-new Random().Next(1, 5)),
                AmountPaid = invoice.TotalAmount / 2
            }).ToList();

            modelBuilder.Entity<Guest>().HasData(guests);
            modelBuilder.Entity<Room>().HasData(rooms);
            modelBuilder.Entity<Booking>().HasData(bookings);
            modelBuilder.Entity<Invoice>().HasData(invoices);
            modelBuilder.Entity<Payment>().HasData(payments);

            base.OnModelCreating(modelBuilder);
        }



    }
}