﻿// <auto-generated />
using System;
using HotelBookingApp.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace HotelBookingApp.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20241224124956_IsCanceledBooking")]
    partial class IsCanceledBooking
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("HotelBookingApp.Entities.Booking", b =>
                {
                    b.Property<int>("BookingId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("BookingId"));

                    b.Property<bool>("BookingCompleted")
                        .HasColumnType("bit");

                    b.Property<DateTime?>("CheckInDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("CheckOutDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("GuestId")
                        .HasColumnType("int");

                    b.Property<bool>("IsCanceled")
                        .HasColumnType("bit");

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
                            BookingStatus = false,
                            CheckInDate = new DateTime(2024, 12, 26, 0, 0, 0, 0, DateTimeKind.Local),
                            CheckOutDate = new DateTime(2024, 12, 28, 0, 0, 0, 0, DateTimeKind.Local),
                            GuestId = 1,
                            IsCanceled = false,
                            IsCheckedIn = false,
                            IsCheckedOut = false,
                            RoomId = 1
                        },
                        new
                        {
                            BookingId = 2,
                            BookingStatus = false,
                            CheckInDate = new DateTime(2024, 12, 28, 0, 0, 0, 0, DateTimeKind.Local),
                            CheckOutDate = new DateTime(2024, 12, 31, 0, 0, 0, 0, DateTimeKind.Local),
                            GuestId = 2,
                            IsCanceled = false,
                            IsCheckedIn = false,
                            IsCheckedOut = false,
                            RoomId = 2
                        },
                        new
                        {
                            BookingId = 3,
                            BookingStatus = false,
                            CheckInDate = new DateTime(2024, 12, 26, 0, 0, 0, 0, DateTimeKind.Local),
                            CheckOutDate = new DateTime(2024, 12, 30, 0, 0, 0, 0, DateTimeKind.Local),
                            GuestId = 3,
                            IsCanceled = false,
                            IsCheckedIn = false,
                            IsCheckedOut = false,
                            RoomId = 3
                        },
                        new
                        {
                            BookingId = 4,
                            BookingStatus = false,
                            CheckInDate = new DateTime(2024, 12, 26, 0, 0, 0, 0, DateTimeKind.Local),
                            CheckOutDate = new DateTime(2024, 12, 31, 0, 0, 0, 0, DateTimeKind.Local),
                            GuestId = 4,
                            IsCanceled = false,
                            IsCheckedIn = false,
                            IsCheckedOut = false,
                            RoomId = 4
                        });
                });

            modelBuilder.Entity("HotelBookingApp.Entities.Room", b =>
                {
                    b.Property<int>("RoomId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("RoomId"));

                    b.Property<decimal>("ExtraBedPrice")
                        .HasColumnType("decimal(18,2)");

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
                            ExtraBedPrice = 0m,
                            ExtraBeds = 0,
                            IsAvailable = true,
                            PricePerNight = 4096m,
                            SizeInSquareMeters = 51,
                            TotalPeople = 0m,
                            Type = "Double"
                        },
                        new
                        {
                            RoomId = 2,
                            ExtraBedPrice = 0m,
                            ExtraBeds = 0,
                            IsAvailable = true,
                            PricePerNight = 1487m,
                            SizeInSquareMeters = 84,
                            TotalPeople = 0m,
                            Type = "Double"
                        },
                        new
                        {
                            RoomId = 3,
                            ExtraBedPrice = 0m,
                            ExtraBeds = 0,
                            IsAvailable = true,
                            PricePerNight = 1607m,
                            SizeInSquareMeters = 48,
                            TotalPeople = 0m,
                            Type = "Single"
                        },
                        new
                        {
                            RoomId = 4,
                            ExtraBedPrice = 0m,
                            ExtraBeds = 2,
                            IsAvailable = true,
                            PricePerNight = 1634m,
                            SizeInSquareMeters = 36,
                            TotalPeople = 0m,
                            Type = "Single"
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
                            Email = "mafalda.gleichner0@example.com",
                            FirstName = "Mafalda",
                            LastName = "Gleichner",
                            PhoneNumber = "679-759-8552 x99711"
                        },
                        new
                        {
                            GuestId = 2,
                            Email = "katelyn.balistreri1@example.com",
                            FirstName = "Katelyn",
                            LastName = "Balistreri",
                            PhoneNumber = "559-837-9563"
                        },
                        new
                        {
                            GuestId = 3,
                            Email = "dedric.borer2@example.com",
                            FirstName = "Dedric",
                            LastName = "Borer",
                            PhoneNumber = "853-445-1967"
                        },
                        new
                        {
                            GuestId = 4,
                            Email = "davon.rodriguez3@example.com",
                            FirstName = "Davon",
                            LastName = "Rodriguez",
                            PhoneNumber = "780-854-4991 x346"
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
                            IsPaid = false,
                            PaymentDeadline = new DateTime(2025, 1, 4, 0, 0, 0, 0, DateTimeKind.Local),
                            TotalAmount = 5161m
                        },
                        new
                        {
                            InvoiceId = 2,
                            BookingId = 2,
                            IsPaid = false,
                            PaymentDeadline = new DateTime(2025, 1, 7, 0, 0, 0, 0, DateTimeKind.Local),
                            TotalAmount = 16124m
                        },
                        new
                        {
                            InvoiceId = 3,
                            BookingId = 3,
                            IsPaid = false,
                            PaymentDeadline = new DateTime(2025, 1, 6, 0, 0, 0, 0, DateTimeKind.Local),
                            TotalAmount = 11573m
                        },
                        new
                        {
                            InvoiceId = 4,
                            BookingId = 4,
                            IsPaid = false,
                            PaymentDeadline = new DateTime(2025, 1, 7, 0, 0, 0, 0, DateTimeKind.Local),
                            TotalAmount = 13557m
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
                            AmountPaid = 2580.5m,
                            InvoiceId = 1,
                            PaymentDate = new DateTime(2024, 12, 22, 0, 0, 0, 0, DateTimeKind.Local)
                        },
                        new
                        {
                            PaymentId = 2,
                            AmountPaid = 8062m,
                            InvoiceId = 2,
                            PaymentDate = new DateTime(2024, 12, 20, 0, 0, 0, 0, DateTimeKind.Local)
                        },
                        new
                        {
                            PaymentId = 3,
                            AmountPaid = 5786.5m,
                            InvoiceId = 3,
                            PaymentDate = new DateTime(2024, 12, 22, 0, 0, 0, 0, DateTimeKind.Local)
                        },
                        new
                        {
                            PaymentId = 4,
                            AmountPaid = 6778.5m,
                            InvoiceId = 4,
                            PaymentDate = new DateTime(2024, 12, 20, 0, 0, 0, 0, DateTimeKind.Local)
                        });
                });

            modelBuilder.Entity("HotelBookingApp.Entities.Booking", b =>
                {
                    b.HasOne("HotelBookingApp.Guest", "Guest")
                        .WithMany()
                        .HasForeignKey("GuestId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("HotelBookingApp.Entities.Room", "Room")
                        .WithMany("Bookings")
                        .HasForeignKey("RoomId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Guest");

                    b.Navigation("Room");
                });

            modelBuilder.Entity("HotelBookingApp.Invoice", b =>
                {
                    b.HasOne("HotelBookingApp.Entities.Booking", "Booking")
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

            modelBuilder.Entity("HotelBookingApp.Entities.Booking", b =>
                {
                    b.Navigation("Invoices");
                });

            modelBuilder.Entity("HotelBookingApp.Entities.Room", b =>
                {
                    b.Navigation("Bookings");
                });

            modelBuilder.Entity("HotelBookingApp.Invoice", b =>
                {
                    b.Navigation("Payments");
                });
#pragma warning restore 612, 618
        }
    }
}
