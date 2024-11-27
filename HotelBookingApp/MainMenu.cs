using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System;
using Microsoft.EntityFrameworkCore;

namespace HotelBookingApp
{
    public class MainMenu
    {
        private readonly BookingManager _bookingManager;
        private readonly RegisterNewBooking _registerNewBooking;
        private readonly AppDbContext _context;
        private readonly Admin _admin; 


        public MainMenu(BookingManager bookingManager, RegisterNewBooking registerNewBooking, AppDbContext context, Admin admin)
        {
            _bookingManager = bookingManager;
            _registerNewBooking = registerNewBooking;
            _context = context;
            _admin = admin;
        }
        public void Run()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("HOTEL BOOKING APP");
                Console.WriteLine("Choose an option:");
                Console.WriteLine("1. Check In Guest");
                Console.WriteLine("2. Check Out Guest");
                Console.WriteLine("3. View Booking Details");
                Console.WriteLine("4. Register New Booking");
                Console.WriteLine("5. View All Guests");
                Console.WriteLine("6. Add New Room"); // Nytt alternativ
                Console.WriteLine("7. Exit");

                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        Console.WriteLine("Enter Booking ID to check in:");
                        if (int.TryParse(Console.ReadLine(), out int checkInId))
                        {
                            _bookingManager.CheckInGuest(checkInId);
                        }
                        else
                        {
                            Console.WriteLine("Invalid Booking ID.");
                        }
                        break;

                    case "2":
                        Console.WriteLine("Enter Booking ID to check out:");
                        if (int.TryParse(Console.ReadLine(), out int checkOutId))
                        {
                            _bookingManager.CheckOutGuest(checkOutId);
                        }
                        else
                        {
                            Console.WriteLine("Invalid Booking ID.");
                        }
                        break;

                    case "3":
                        Console.WriteLine("Enter Booking ID to view details:");
                        if (int.TryParse(Console.ReadLine(), out int viewId))
                        {
                            _bookingManager.ViewBookingDetails(viewId);
                        }
                        else
                        {
                            Console.WriteLine("Invalid Booking ID.");
                        }
                        break;

                    case "4":
                        _registerNewBooking.Execute();
                        break;

                    case "5":
                        ViewAllGuests();
                        break;

                    case "6": // Ny funktionalitet
                        _admin.AddRoom();
                        break;

                    case "7":
                        Console.WriteLine("Exiting program...");
                        return;

                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }

                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
            }
        }




        private void ViewAllGuests()
        {
            var guests = _context.Guests
                .GroupJoin(
                    _context.Bookings,
                    g => g.GuestId,
                    b => b.GuestId,
                    (guest, bookings) => new
                    {
                        Guest = guest,
                        Bookings = bookings.ToList()
                    })
                .ToList();

            if (!guests.Any())
            {
                Console.WriteLine("No guests found.");
                return;
            }

            Console.WriteLine("---- Guests and Bookings ----");
            foreach (var entry in guests)
            {
                var guest = entry.Guest;
                Console.WriteLine($"Guest: {guest.FirstName} {guest.LastName} (ID: {guest.GuestId})");
                Console.WriteLine($"Email: {guest.Email}");
                Console.WriteLine($"Phone: {guest.PhoneNumber}");

                if (!entry.Bookings.Any())
                {
                    Console.WriteLine("No bookings for this guest.");
                }
                else
                {
                    foreach (var booking in entry.Bookings)
                    {
                        Console.WriteLine($"- Booking ID: {booking.BookingId}");
                        Console.WriteLine($"  Room ID: {booking.RoomId}");
                        Console.WriteLine($"  Check-In Date: {booking.CheckInDate}");
                        Console.WriteLine($"  Check-Out Date: {booking.CheckOutDate}");
                        Console.WriteLine($"  Checked In: {(booking.IsCheckedIn ? "Yes" : "No")}");
                        Console.WriteLine($"  Checked Out: {(booking.IsCheckedOut ? "Yes" : "No")}");
                    }
                }

                Console.WriteLine("----------------------------");
            }
        }


    }
}
