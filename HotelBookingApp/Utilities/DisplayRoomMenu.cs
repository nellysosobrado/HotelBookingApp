using HotelBookingApp.Controllers;
using System;

namespace HotelBookingApp.Utilities
{
    public class DisplayRoomMenu
    {
        private readonly RoomController _roomController;

        public DisplayRoomMenu(RoomController roomController)
        {
            _roomController = roomController;
        }

        public void Menu()
        {
            string[] options = { "Register New Room", "Edit Room", "View All Rooms","Delete room", "Main Menu" };

            while (true)
            {
                Console.Clear();
                Console.WriteLine("Room Menu");

                for (int i = 0; i < options.Length; i++)
                    Console.WriteLine($"{i + 1}. {options[i]}");

                Console.Write("\nEnter your choice: ");
                var input = Console.ReadLine();

                switch (input)
                {
                    case "1":
                        _roomController.AddNewRoom();
                        break;
                    case "2":
                        _roomController.EditRoom();
                        break;
                    case "3":
                        _roomController.ViewAllRooms();
                        break;
                    case "4":
                        _roomController.DeleteRoom();
                        break;
                    case "5":
                        Console.WriteLine("Returning to Main Menu...");
                        return;
                    default:
                        Console.WriteLine("Invalid choice. Try again.");
                        break;
                }

                Console.WriteLine("Press any key to return to menu...");
                Console.ReadKey(true);
            }
        }
    }
}
