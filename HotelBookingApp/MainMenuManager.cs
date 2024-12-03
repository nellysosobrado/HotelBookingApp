using System;

namespace HotelBookingApp
{
    public class MainMenuManager
    {
        private readonly BookingManager _bookingManager;
        private readonly Admin _admin;
        private readonly RoomService _roomService;

        public MainMenuManager(BookingManager bookingManager, Admin admin, RoomService roomService)
        {
            _bookingManager = bookingManager;
            _admin = admin;
            _roomService = roomService;
        }

        public void Run()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("HOTEL BOOKING APP");
                Console.WriteLine("Choose an option:");
                Console.WriteLine("1. Booking");
                Console.WriteLine("2. Room");
                Console.WriteLine("3. Exit");

                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        _bookingManager.Run();
                        break;

                    case "2":
                        _roomService.Menu();
                        break;

                    case "3":
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
    }
}
