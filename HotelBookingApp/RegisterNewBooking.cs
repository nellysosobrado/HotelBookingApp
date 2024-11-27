using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;

namespace HotelBookingApp
{
    public class RegisterNewBooking
    {
        private readonly AppDbContext _context;

        public RegisterNewBooking(AppDbContext context)
        {
            _context = context;
        }

        public void Execute()
        {
            Console.WriteLine("Enter Guest First Name:");
            var firstName = Console.ReadLine();

            Console.WriteLine("Enter Guest Last Name:");
            var lastName = Console.ReadLine();

            Console.WriteLine("Enter Guest Email:");
            var email = Console.ReadLine();

            Console.WriteLine("Enter Guest Phone Number:");
            var phoneNumber = Console.ReadLine();

            Console.WriteLine("Enter Room ID to book:");
            if (!int.TryParse(Console.ReadLine(), out int roomId))
            {
                Console.WriteLine("Invalid Room ID.");
                return;
            }

            var room = _context.Rooms.FirstOrDefault(r => r.RoomId == roomId && r.IsAvailable);
            if (room == null)
            {
                Console.WriteLine("Room not found or not available.");
                return;
            }

            Console.WriteLine("Enter Check-In Date (yyyy-MM-dd):");
            if (!DateTime.TryParse(Console.ReadLine(), out DateTime checkInDate))
            {
                Console.WriteLine("Invalid Check-In Date.");
                return;
            }

            Console.WriteLine("Enter Check-Out Date (yyyy-MM-dd):");
            if (!DateTime.TryParse(Console.ReadLine(), out DateTime checkOutDate) || checkOutDate <= checkInDate)
            {
                Console.WriteLine("Invalid Check-Out Date.");
                return;
            }

            // Skapa ny gäst
            var guest = new Guest
            {
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                PhoneNumber = phoneNumber
            };

            _context.Guests.Add(guest);
            _context.SaveChanges();

            // Skapa ny bokning
            var booking = new Booking
            {
                GuestId = guest.GuestId,
                RoomId = room.RoomId,
                CheckInDate = checkInDate,
                CheckOutDate = checkOutDate,
                IsCheckedIn = false,
                IsCheckedOut = false
            };

            _context.Bookings.Add(booking);

            // Markera rummet som inte längre tillgängligt
            room.IsAvailable = false;

            _context.SaveChanges();

            Console.WriteLine("Booking registered successfully!");
        }
    }
}
