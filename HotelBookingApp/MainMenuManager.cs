using System;

namespace HotelBookingApp
{
    public class MainMenuManager
    {
        private readonly BookingService _bookingService;
        private readonly Admin _admin;
        private readonly RoomService _roomService;
        private readonly GuestServices _guestServices;

        public MainMenuManager(BookingService bookingManager, Admin admin, RoomService roomService,GuestServices guestServices )
        {
            _bookingService = bookingManager;
            _admin = admin;
            _roomService = roomService;
            _guestServices = guestServices;
            
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
                Console.WriteLine("3. Guest");
                Console.WriteLine("4. Exit");

                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        _bookingService.Menu();
                        break;

                    case "2":
                        _roomService.Menu();
                        break;

                    case "3":
                        _guestServices.Menu();
                        return;
                        case "4":
                        Console.WriteLine("Exiting Program");
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
