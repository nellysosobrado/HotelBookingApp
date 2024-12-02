using System;
using System.Linq;

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
            Console.Clear();
            Console.WriteLine("=== REGISTER NEW BOOKING ===");

            // Gästinformation
            Console.WriteLine("Enter Guest First Name:");
            var firstName = Console.ReadLine()?.Trim();

            Console.WriteLine("Enter Guest Last Name:");
            var lastName = Console.ReadLine()?.Trim();

            Console.WriteLine("Enter Guest Email:");
            var email = Console.ReadLine()?.Trim();

            Console.WriteLine("Enter Guest Phone Number:");
            var phoneNumber = Console.ReadLine()?.Trim();

            // Rumval
            Console.WriteLine("Enter Room ID to book:");
            if (!int.TryParse(Console.ReadLine(), out int roomId))
            {
                Console.WriteLine("Invalid Room ID.");
                return;
            }

            var room = _context.Rooms.FirstOrDefault(r => r.RoomId == roomId && r.IsAvailable);
            if (room == null)
            {
                Console.WriteLine("Room not found or is not currently available.");
                return;
            }

            // Datumvalidering
            DateTime checkInDate, checkOutDate;
            while (true)
            {
                Console.WriteLine("Enter Check-In Date (yyyy-MM-dd):");
                var checkInInput = Console.ReadLine();

                if (!DateTime.TryParse(checkInInput, out checkInDate) || checkInDate.Date < DateTime.Now.Date)
                {
                    Console.WriteLine("Invalid Check-In Date. The date cannot be in the past.");
                    continue;
                }

                Console.WriteLine("Enter Check-Out Date (yyyy-MM-dd):");
                var checkOutInput = Console.ReadLine();

                if (!DateTime.TryParse(checkOutInput, out checkOutDate) || checkOutDate <= checkInDate)
                {
                    Console.WriteLine("Invalid Check-Out Date. It must be after the Check-In Date.");
                    continue;
                }

                // Kontrollera om datumintervallen redan är bokade
                var isConflict = _context.Bookings.Any(b =>
                    b.RoomId == roomId &&
                    ((checkInDate >= b.CheckInDate && checkInDate < b.CheckOutDate) ||
                     (checkOutDate > b.CheckInDate && checkOutDate <= b.CheckOutDate)));

                if (isConflict)
                {
                    Console.WriteLine("The selected room is already booked for the chosen dates. Please try different dates.");
                    continue;
                }

                break;
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
                IsCheckedOut = false,
                BookingStatus = false
            };

            _context.Bookings.Add(booking);

            // Uppdatera rummet som ej tillgängligt
            room.IsAvailable = false;

            _context.SaveChanges();

            Console.WriteLine($"Booking registered successfully for Guest ID {guest.GuestId} in Room ID {room.RoomId}.");
        }
    }
}
