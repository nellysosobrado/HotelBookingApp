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
    [Migration("20250103083811_InitialCreate")]
    partial class InitialCreate
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

                    b.Property<DateTime?>("CanceledDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("CheckInDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("CheckOutDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("ExtraBeds")
                        .HasColumnType("int");

                    b.Property<int>("GuestId")
                        .HasColumnType("int");

                    b.Property<bool>("IsCanceled")
                        .HasColumnType("bit");

                    b.Property<bool>("IsCheckedIn")
                        .HasColumnType("bit");

                    b.Property<bool>("IsCheckedOut")
                        .HasColumnType("bit");

                    b.Property<DateTime>("RegistrationDate")
                        .HasColumnType("datetime2");

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
                            BookingCompleted = false,
                            CheckInDate = new DateTime(2025, 1, 6, 0, 0, 0, 0, DateTimeKind.Local),
                            CheckOutDate = new DateTime(2025, 1, 10, 0, 0, 0, 0, DateTimeKind.Local),
                            CreatedDate = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            ExtraBeds = 0,
                            GuestId = 1,
                            IsCanceled = false,
                            IsCheckedIn = false,
                            IsCheckedOut = false,
                            RegistrationDate = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            RoomId = 1
                        },
                        new
                        {
                            BookingId = 2,
                            BookingCompleted = false,
                            CheckInDate = new DateTime(2025, 1, 6, 0, 0, 0, 0, DateTimeKind.Local),
                            CheckOutDate = new DateTime(2025, 1, 11, 0, 0, 0, 0, DateTimeKind.Local),
                            CreatedDate = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            ExtraBeds = 0,
                            GuestId = 2,
                            IsCanceled = false,
                            IsCheckedIn = false,
                            IsCheckedOut = false,
                            RegistrationDate = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            RoomId = 2
                        },
                        new
                        {
                            BookingId = 3,
                            BookingCompleted = false,
                            CheckInDate = new DateTime(2025, 1, 6, 0, 0, 0, 0, DateTimeKind.Local),
                            CheckOutDate = new DateTime(2025, 1, 10, 0, 0, 0, 0, DateTimeKind.Local),
                            CreatedDate = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            ExtraBeds = 0,
                            GuestId = 3,
                            IsCanceled = false,
                            IsCheckedIn = false,
                            IsCheckedOut = false,
                            RegistrationDate = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            RoomId = 3
                        },
                        new
                        {
                            BookingId = 4,
                            BookingCompleted = false,
                            CheckInDate = new DateTime(2025, 1, 4, 0, 0, 0, 0, DateTimeKind.Local),
                            CheckOutDate = new DateTime(2025, 1, 8, 0, 0, 0, 0, DateTimeKind.Local),
                            CreatedDate = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            ExtraBeds = 0,
                            GuestId = 4,
                            IsCanceled = false,
                            IsCheckedIn = false,
                            IsCheckedOut = false,
                            RegistrationDate = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            RoomId = 4
                        });
                });

            modelBuilder.Entity("HotelBookingApp.Entities.CanceledBookingHistory", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("BookingId")
                        .HasColumnType("int");

                    b.Property<DateTime>("CanceledDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("GuestId")
                        .HasColumnType("int");

                    b.Property<string>("GuestName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsCanceled")
                        .HasColumnType("bit");

                    b.Property<string>("Reason")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("RoomId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("BookingId");

                    b.HasIndex("RoomId");

                    b.ToTable("CanceledBookingsHistory");
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

                    b.Property<bool>("IsDeleted")
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
                            IsDeleted = false,
                            PricePerNight = 3463m,
                            SizeInSquareMeters = 65,
                            TotalPeople = 1m,
                            Type = "Single"
                        },
                        new
                        {
                            RoomId = 2,
                            ExtraBedPrice = 0m,
                            ExtraBeds = 1,
                            IsAvailable = true,
                            IsDeleted = false,
                            PricePerNight = 2369m,
                            SizeInSquareMeters = 21,
                            TotalPeople = 3m,
                            Type = "Double"
                        },
                        new
                        {
                            RoomId = 3,
                            ExtraBedPrice = 0m,
                            ExtraBeds = 2,
                            IsAvailable = true,
                            IsDeleted = false,
                            PricePerNight = 3922m,
                            SizeInSquareMeters = 22,
                            TotalPeople = 4m,
                            Type = "Double"
                        },
                        new
                        {
                            RoomId = 4,
                            ExtraBedPrice = 0m,
                            ExtraBeds = 0,
                            IsAvailable = true,
                            IsDeleted = false,
                            PricePerNight = 4857m,
                            SizeInSquareMeters = 32,
                            TotalPeople = 2m,
                            Type = "Double"
                        });
                });

            modelBuilder.Entity("HotelBookingApp.Guest", b =>
                {
                    b.Property<int>("GuestId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("GuestId"));

                    b.Property<DateTime?>("DeletedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhoneNumber")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RemovalReason")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("GuestId");

                    b.ToTable("Guests");

                    b.HasData(
                        new
                        {
                            GuestId = 1,
                            Email = "margarita.d'amore0@example.com",
                            FirstName = "Margarita",
                            IsDeleted = false,
                            LastName = "D'Amore",
                            PhoneNumber = "658.247.8219",
                            RemovalReason = "Not applicable"
                        },
                        new
                        {
                            GuestId = 2,
                            Email = "jaylin.beahan1@example.com",
                            FirstName = "Jaylin",
                            IsDeleted = false,
                            LastName = "Beahan",
                            PhoneNumber = "204-791-5675 x468",
                            RemovalReason = "Not applicable"
                        },
                        new
                        {
                            GuestId = 3,
                            Email = "kiana.schulist2@example.com",
                            FirstName = "Kiana",
                            IsDeleted = false,
                            LastName = "Schulist",
                            PhoneNumber = "834-645-7666 x367",
                            RemovalReason = "Not applicable"
                        },
                        new
                        {
                            GuestId = 4,
                            Email = "oral.gleason3@example.com",
                            FirstName = "Oral",
                            IsDeleted = false,
                            LastName = "Gleason",
                            PhoneNumber = "519.765.7953",
                            RemovalReason = "Not applicable"
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

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

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
                            CreatedDate = new DateTime(2025, 1, 3, 0, 0, 0, 0, DateTimeKind.Local),
                            IsPaid = false,
                            PaymentDeadline = new DateTime(2025, 1, 13, 0, 0, 0, 0, DateTimeKind.Local),
                            TotalAmount = 17241m
                        },
                        new
                        {
                            InvoiceId = 2,
                            BookingId = 2,
                            CreatedDate = new DateTime(2025, 1, 3, 0, 0, 0, 0, DateTimeKind.Local),
                            IsPaid = false,
                            PaymentDeadline = new DateTime(2025, 1, 13, 0, 0, 0, 0, DateTimeKind.Local),
                            TotalAmount = 6144m
                        },
                        new
                        {
                            InvoiceId = 3,
                            BookingId = 3,
                            CreatedDate = new DateTime(2025, 1, 3, 0, 0, 0, 0, DateTimeKind.Local),
                            IsPaid = true,
                            PaymentDeadline = new DateTime(2025, 1, 13, 0, 0, 0, 0, DateTimeKind.Local),
                            TotalAmount = 5214m
                        },
                        new
                        {
                            InvoiceId = 4,
                            BookingId = 4,
                            CreatedDate = new DateTime(2025, 1, 3, 0, 0, 0, 0, DateTimeKind.Local),
                            IsPaid = false,
                            PaymentDeadline = new DateTime(2024, 12, 23, 0, 0, 0, 0, DateTimeKind.Local),
                            TotalAmount = 10806m
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
                            AmountPaid = 5214m,
                            InvoiceId = 3,
                            PaymentDate = new DateTime(2024, 12, 30, 0, 0, 0, 0, DateTimeKind.Local)
                        });
                });

            modelBuilder.Entity("HotelBookingApp.Entities.Booking", b =>
                {
                    b.HasOne("HotelBookingApp.Guest", "Guest")
                        .WithMany("Bookings")
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

            modelBuilder.Entity("HotelBookingApp.Entities.CanceledBookingHistory", b =>
                {
                    b.HasOne("HotelBookingApp.Entities.Booking", "Booking")
                        .WithMany()
                        .HasForeignKey("BookingId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("HotelBookingApp.Entities.Room", "Room")
                        .WithMany()
                        .HasForeignKey("RoomId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Booking");

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

            modelBuilder.Entity("HotelBookingApp.Guest", b =>
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