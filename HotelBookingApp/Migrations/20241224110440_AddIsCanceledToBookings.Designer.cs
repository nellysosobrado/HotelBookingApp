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
    [Migration("20241224110440_AddIsCanceledToBookings")]
    partial class AddIsCanceledToBookings
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
                            CheckInDate = new DateTime(2024, 12, 28, 0, 0, 0, 0, DateTimeKind.Local),
                            CheckOutDate = new DateTime(2024, 12, 29, 0, 0, 0, 0, DateTimeKind.Local),
                            GuestId = 1,
                            IsCanceled = true,
                            IsCheckedIn = false,
                            IsCheckedOut = false,
                            RoomId = 1
                        },
                        new
                        {
                            BookingId = 2,
                            BookingStatus = false,
                            CheckInDate = new DateTime(2024, 12, 27, 0, 0, 0, 0, DateTimeKind.Local),
                            CheckOutDate = new DateTime(2024, 12, 29, 0, 0, 0, 0, DateTimeKind.Local),
                            GuestId = 2,
                            IsCanceled = true,
                            IsCheckedIn = false,
                            IsCheckedOut = false,
                            RoomId = 2
                        },
                        new
                        {
                            BookingId = 3,
                            BookingStatus = false,
                            CheckInDate = new DateTime(2024, 12, 28, 0, 0, 0, 0, DateTimeKind.Local),
                            CheckOutDate = new DateTime(2024, 12, 29, 0, 0, 0, 0, DateTimeKind.Local),
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
                            CheckInDate = new DateTime(2024, 12, 27, 0, 0, 0, 0, DateTimeKind.Local),
                            CheckOutDate = new DateTime(2024, 12, 28, 0, 0, 0, 0, DateTimeKind.Local),
                            GuestId = 4,
                            IsCanceled = true,
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
                            ExtraBeds = 1,
                            IsAvailable = true,
                            PricePerNight = 3438m,
                            SizeInSquareMeters = 96,
                            TotalPeople = 0m,
                            Type = "Single"
                        },
                        new
                        {
                            RoomId = 2,
                            ExtraBedPrice = 0m,
                            ExtraBeds = 1,
                            IsAvailable = true,
                            PricePerNight = 1575m,
                            SizeInSquareMeters = 93,
                            TotalPeople = 0m,
                            Type = "Single"
                        },
                        new
                        {
                            RoomId = 3,
                            ExtraBedPrice = 0m,
                            ExtraBeds = 1,
                            IsAvailable = true,
                            PricePerNight = 1393m,
                            SizeInSquareMeters = 67,
                            TotalPeople = 0m,
                            Type = "Single"
                        },
                        new
                        {
                            RoomId = 4,
                            ExtraBedPrice = 0m,
                            ExtraBeds = 1,
                            IsAvailable = true,
                            PricePerNight = 1872m,
                            SizeInSquareMeters = 63,
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
                            Email = "ryan.rodriguez0@example.com",
                            FirstName = "Ryan",
                            LastName = "Rodriguez",
                            PhoneNumber = "1-429-497-9296"
                        },
                        new
                        {
                            GuestId = 2,
                            Email = "christina.dietrich1@example.com",
                            FirstName = "Christina",
                            LastName = "Dietrich",
                            PhoneNumber = "1-244-353-1104 x21370"
                        },
                        new
                        {
                            GuestId = 3,
                            Email = "amparo.schulist2@example.com",
                            FirstName = "Amparo",
                            LastName = "Schulist",
                            PhoneNumber = "(396) 279-8814"
                        },
                        new
                        {
                            GuestId = 4,
                            Email = "austin.yost3@example.com",
                            FirstName = "Austin",
                            LastName = "Yost",
                            PhoneNumber = "(516) 724-7554 x06613"
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
                            PaymentDeadline = new DateTime(2025, 1, 5, 0, 0, 0, 0, DateTimeKind.Local),
                            TotalAmount = 13996m
                        },
                        new
                        {
                            InvoiceId = 2,
                            BookingId = 2,
                            IsPaid = false,
                            PaymentDeadline = new DateTime(2025, 1, 5, 0, 0, 0, 0, DateTimeKind.Local),
                            TotalAmount = 11759m
                        },
                        new
                        {
                            InvoiceId = 3,
                            BookingId = 3,
                            IsPaid = false,
                            PaymentDeadline = new DateTime(2025, 1, 5, 0, 0, 0, 0, DateTimeKind.Local),
                            TotalAmount = 19496m
                        },
                        new
                        {
                            InvoiceId = 4,
                            BookingId = 4,
                            IsPaid = false,
                            PaymentDeadline = new DateTime(2025, 1, 4, 0, 0, 0, 0, DateTimeKind.Local),
                            TotalAmount = 17602m
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
                            AmountPaid = 6998m,
                            InvoiceId = 1,
                            PaymentDate = new DateTime(2024, 12, 22, 0, 0, 0, 0, DateTimeKind.Local)
                        },
                        new
                        {
                            PaymentId = 2,
                            AmountPaid = 5879.5m,
                            InvoiceId = 2,
                            PaymentDate = new DateTime(2024, 12, 21, 0, 0, 0, 0, DateTimeKind.Local)
                        },
                        new
                        {
                            PaymentId = 3,
                            AmountPaid = 9748m,
                            InvoiceId = 3,
                            PaymentDate = new DateTime(2024, 12, 20, 0, 0, 0, 0, DateTimeKind.Local)
                        },
                        new
                        {
                            PaymentId = 4,
                            AmountPaid = 8801m,
                            InvoiceId = 4,
                            PaymentDate = new DateTime(2024, 12, 22, 0, 0, 0, 0, DateTimeKind.Local)
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
