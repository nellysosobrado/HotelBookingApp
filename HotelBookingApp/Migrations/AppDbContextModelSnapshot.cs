﻿// <auto-generated />
using System;
using HotelBookingApp;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace HotelBookingApp.Migrations
{
    [DbContext(typeof(AppDbContext))]
    partial class AppDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("HotelBookingApp.Booking", b =>
                {
                    b.Property<int>("BookingId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("BookingId"));

                    b.Property<bool>("BookingStatus")
                        .HasColumnType("bit");

                    b.Property<DateTime?>("CheckInDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("CheckOutDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("GuestId")
                        .HasColumnType("int");

                    b.Property<bool>("IsCheckedIn")
                        .HasColumnType("bit");

                    b.Property<bool>("IsCheckedOut")
                        .HasColumnType("bit");

                    b.Property<int>("RoomId")
                        .HasColumnType("int");

                    b.HasKey("BookingId");

                    b.HasIndex("GuestId");

                    b.HasIndex("RoomId");

                    b.ToTable("Bookings");

                    b.HasData(
                        new
                        {
                            BookingId = 1,
                            BookingStatus = true,
                            CheckInDate = new DateTime(2024, 11, 29, 15, 38, 28, 594, DateTimeKind.Local).AddTicks(2563),
                            CheckOutDate = new DateTime(2024, 12, 3, 15, 38, 28, 595, DateTimeKind.Local).AddTicks(5171),
                            GuestId = 1,
                            IsCheckedIn = true,
                            IsCheckedOut = true,
                            RoomId = 1
                        },
                        new
                        {
                            BookingId = 2,
                            BookingStatus = false,
                            CheckInDate = new DateTime(2024, 12, 2, 15, 38, 28, 595, DateTimeKind.Local).AddTicks(5379),
                            GuestId = 2,
                            IsCheckedIn = true,
                            IsCheckedOut = false,
                            RoomId = 2
                        });
                });

            modelBuilder.Entity("HotelBookingApp.Guest", b =>
                {
                    b.Property<int>("GuestId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("GuestId"));

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhoneNumber")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("GuestId");

                    b.ToTable("Guests");

                    b.HasData(
                        new
                        {
                            GuestId = 1,
                            Email = "234p@example.com",
                            FirstName = "Person2",
                            LastName = "lastname1",
                            PhoneNumber = "234"
                        },
                        new
                        {
                            GuestId = 2,
                            Email = "342p@example.com",
                            FirstName = "Person2",
                            LastName = "lastname2",
                            PhoneNumber = "3453"
                        },
                        new
                        {
                            GuestId = 3,
                            Email = "p234@example.com",
                            FirstName = "Person3",
                            LastName = "lastname3",
                            PhoneNumber = "3453"
                        },
                        new
                        {
                            GuestId = 4,
                            Email = "243p@example.com",
                            FirstName = "Person4",
                            LastName = "lastname4",
                            PhoneNumber = "43"
                        });
                });

            modelBuilder.Entity("HotelBookingApp.Invoice", b =>
                {
                    b.Property<int>("InvoiceId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("InvoiceId"));

                    b.Property<int>("BookingId")
                        .HasColumnType("int");

                    b.Property<bool>("IsPaid")
                        .HasColumnType("bit");

                    b.Property<DateTime>("PaymentDeadline")
                        .HasColumnType("datetime2");

                    b.Property<decimal>("TotalAmount")
                        .HasColumnType("decimal(18,2)");

                    b.HasKey("InvoiceId");

                    b.HasIndex("BookingId");

                    b.ToTable("Invoices");

                    b.HasData(
                        new
                        {
                            InvoiceId = 1,
                            BookingId = 1,
                            IsPaid = true,
                            PaymentDeadline = new DateTime(2024, 12, 1, 15, 38, 28, 595, DateTimeKind.Local).AddTicks(6270),
                            TotalAmount = 500.00m
                        },
                        new
                        {
                            InvoiceId = 2,
                            BookingId = 2,
                            IsPaid = false,
                            PaymentDeadline = new DateTime(2024, 12, 9, 15, 38, 28, 595, DateTimeKind.Local).AddTicks(6408),
                            TotalAmount = 300.00m
                        });
                });

            modelBuilder.Entity("HotelBookingApp.Payment", b =>
                {
                    b.Property<int>("PaymentId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("PaymentId"));

                    b.Property<decimal>("AmountPaid")
                        .HasColumnType("decimal(18,2)");

                    b.Property<int>("InvoiceId")
                        .HasColumnType("int");

                    b.Property<DateTime>("PaymentDate")
                        .HasColumnType("datetime2");

                    b.HasKey("PaymentId");

                    b.HasIndex("InvoiceId");

                    b.ToTable("Payments");

                    b.HasData(
                        new
                        {
                            PaymentId = 1,
                            AmountPaid = 500.00m,
                            InvoiceId = 1,
                            PaymentDate = new DateTime(2024, 11, 30, 15, 38, 28, 595, DateTimeKind.Local).AddTicks(6921)
                        });
                });

            modelBuilder.Entity("HotelBookingApp.Room", b =>
                {
                    b.Property<int>("RoomId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("RoomId"));

                    b.Property<int>("ExtraBeds")
                        .HasColumnType("int");

                    b.Property<bool>("IsAvailable")
                        .HasColumnType("bit");

                    b.Property<decimal>("PricePerNight")
                        .HasColumnType("decimal(18,2)");

                    b.Property<int>("SizeInSquareMeters")
                        .HasColumnType("int");

                    b.Property<decimal>("TotalPeople")
                        .HasColumnType("decimal(18,2)");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("RoomId");

                    b.ToTable("Rooms");

                    b.HasData(
                        new
                        {
                            RoomId = 1,
                            ExtraBeds = 0,
                            IsAvailable = false,
                            PricePerNight = 1500m,
                            SizeInSquareMeters = 20,
                            TotalPeople = 0m,
                            Type = "Single"
                        },
                        new
                        {
                            RoomId = 2,
                            ExtraBeds = 1,
                            IsAvailable = false,
                            PricePerNight = 3500m,
                            SizeInSquareMeters = 80,
                            TotalPeople = 0m,
                            Type = "Double"
                        },
                        new
                        {
                            RoomId = 3,
                            ExtraBeds = 2,
                            IsAvailable = true,
                            PricePerNight = 3500m,
                            SizeInSquareMeters = 70,
                            TotalPeople = 0m,
                            Type = "Double"
                        },
                        new
                        {
                            RoomId = 4,
                            ExtraBeds = 0,
                            IsAvailable = true,
                            PricePerNight = 1500m,
                            SizeInSquareMeters = 215,
                            TotalPeople = 0m,
                            Type = "Single"
                        });
                });

            modelBuilder.Entity("HotelBookingApp.Booking", b =>
                {
                    b.HasOne("HotelBookingApp.Guest", "Guest")
                        .WithMany()
                        .HasForeignKey("GuestId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("HotelBookingApp.Room", "Room")
                        .WithMany()
                        .HasForeignKey("RoomId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Guest");

                    b.Navigation("Room");
                });

            modelBuilder.Entity("HotelBookingApp.Invoice", b =>
                {
                    b.HasOne("HotelBookingApp.Booking", "Booking")
                        .WithMany("Invoices")
                        .HasForeignKey("BookingId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Booking");
                });

            modelBuilder.Entity("HotelBookingApp.Payment", b =>
                {
                    b.HasOne("HotelBookingApp.Invoice", "Invoice")
                        .WithMany("Payments")
                        .HasForeignKey("InvoiceId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Invoice");
                });

            modelBuilder.Entity("HotelBookingApp.Booking", b =>
                {
                    b.Navigation("Invoices");
                });

            modelBuilder.Entity("HotelBookingApp.Invoice", b =>
                {
                    b.Navigation("Payments");
                });
#pragma warning restore 612, 618
        }
    }
}
