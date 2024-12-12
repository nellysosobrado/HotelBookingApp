using System;

public class BookingController
{
    private readonly BookingRepository _bookingRepository;

    public BookingController(BookingRepository bookingRepository)
    {
        _bookingRepository = bookingRepository;
    }

    public void CheckIn()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("CHECK IN GUEST");

            Console.WriteLine("Enter 'Exit' to go back to main menu");
            Console.Write("Enter Booking ID to check in: ");

            string input = Console.ReadLine()?.Trim();

            if (input?.ToLower() == "exit")
            {
                break;
            }

            if (int.TryParse(input, out int checkInId))
            {
                var booking = _bookingRepository.GetBookingById(checkInId);
                if (booking == null)
                {
                    Console.WriteLine("Booking not found. Press any key to try again...");
                    Console.ReadKey();
                    continue;
                }

                if (booking.IsCheckedIn)
                {
                    Console.WriteLine("Guest is already checked in. Press any key to try again...");
                    Console.ReadKey();
                    continue;
                }

                booking.IsCheckedIn = true;
                booking.CheckInDate = DateTime.Now;
                _bookingRepository.UpdateBooking(booking);

                var guest = _bookingRepository.GetGuestById(booking.GuestId);
                var room = _bookingRepository.GetRoomById(booking.RoomId);

                Console.Clear();
                Console.WriteLine($"\nGuest {guest?.FirstName + " " + guest?.LastName} has been successfully checked in.");
                Console.WriteLine(new string('-', 60));
                Console.WriteLine($"{"Booking ID",-15}{"Guest",-20}{"Room ID",-10}{"Check-In Date",-15}");
                Console.WriteLine(new string('-', 60));
                Console.WriteLine($"{booking.BookingId,-15}{guest?.FirstName + " " + guest?.LastName,-20}{room?.RoomId,-10}{booking.CheckInDate,-15:yyyy-MM-dd HH:mm}");
                Console.WriteLine(new string('-', 60));

                Console.WriteLine("\nPress any key to continue...");
                Console.ReadKey();
            }
            else if (input == string.Empty)
            {
                Console.WriteLine("Input cannot be empty. Press any key to try again...");
                Console.ReadKey();
            }
            else
            {
                Console.WriteLine("Invalid Booking ID. Press any key to try again...");
                Console.ReadKey();
            }
        }
    }
}
