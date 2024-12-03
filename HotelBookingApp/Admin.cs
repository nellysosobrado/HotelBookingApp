using System;

namespace HotelBookingApp
{
    public class Admin
    {
        private readonly RoomService _roomService;

        public Admin(RoomService roomService)
        {
            _roomService = roomService;
        }

        public void Run()
        {
            while (true)
            {
                Console.Clear();
                DisplayMenu();

                var choice = Console.ReadLine()?.Trim();

                switch (choice)
                {
                    case "1":
                        HandleAddRoom();
                        break;
                    case "2":
                        _roomService.ViewAllRooms();
                        break;
                    case "3":
                        _roomService.EditRoom();
                        break;
                    case "4":
                        EditGuest(); // Gästhantering kvar i Admin
                        break;
                    case "5":
                        Console.WriteLine("Returning to main menu...");
                        return;
                    case "6":
                        _roomService.RegisterNewRoom();
                        break;
                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }

                PromptToContinue();
            }
        }

        private void DisplayMenu()
        {
            Console.WriteLine("=== ADMIN MENU ===");
            Console.WriteLine("Choose an option:");
            Console.WriteLine("1. Register a Room");
            Console.WriteLine("2. View All Rooms");
            Console.WriteLine("3. Edit Room");
            Console.WriteLine("4. Edit Guest");
            Console.WriteLine("5. Back to Main Menu");
            Console.WriteLine("6. Register New room");
            Console.WriteLine("===================");
        }

        private void HandleAddRoom()
        {
            Console.Clear();
            Console.WriteLine("=== ADD ROOM ===");

            Console.WriteLine("Enter the type of room (Single/Double):");
            var roomType = Console.ReadLine()?.Trim().ToLower();
            if (roomType != "single" && roomType != "double")
            {
                Console.WriteLine("Invalid room type. Room creation failed.");
                return;
            }

            Console.WriteLine("Enter the price per night:");
            if (!decimal.TryParse(Console.ReadLine(), out decimal price) || price <= 0)
            {
                Console.WriteLine("Invalid price. Room creation failed.");
                return;
            }

            Console.WriteLine("Enter the size of the room in square meters:");
            if (!int.TryParse(Console.ReadLine(), out int size) || size <= 0)
            {
                Console.WriteLine("Invalid size. Room creation failed.");
                return;
            }

            int extraBeds = 0;
            if (roomType == "double")
            {
                Console.WriteLine("Enter the number of extra beds (0-2):");
                if (!int.TryParse(Console.ReadLine(), out extraBeds) || extraBeds < 0 || extraBeds > 2)
                {
                    Console.WriteLine("Invalid number of extra beds. Room creation failed.");
                    return;
                }
            }

            _roomService.AddRoom(roomType, price, size, extraBeds);
        }

       
        private void PromptToContinue()
        {
            Console.WriteLine("\nPress any key to return to the menu...");
            Console.ReadKey();
        }

        private void EditGuest()
        {
            // Gästredigering kvar som tidigare
        }
    }
}
