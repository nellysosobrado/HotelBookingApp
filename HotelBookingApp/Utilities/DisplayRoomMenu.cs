//using HotelBookingApp.Controllers;
//using System;

//namespace HotelBookingApp.Utilities
//{
//    public class DisplayRoomMenu
//    {
//        private readonly RoomController _roomController;

//        public DisplayRoomMenu(RoomController roomController)
//        {
//            _roomController = roomController;
//        }

//        public void Menu()
//        {
//            string[] options = { "Register New Room", "Edit Room", "View All Rooms", "Delete Room", "Main Menu" };
//            int selectedIndex = 0;

//            while (true)
//            {
//                Console.Clear();

//                for (int i = 0; i < options.Length; i++)
//                {
//                    if (i == selectedIndex)
//                    {
//                        Console.ForegroundColor = ConsoleColor.Green;
//                        Console.WriteLine($"> {options[i]}");
//                        Console.ResetColor();
//                    }
//                    else
//                    {
//                        Console.WriteLine($"  {options[i]}");
//                    }
//                }


//                var key = Console.ReadKey(true).Key;

//                switch (key)
//                {
//                    case ConsoleKey.UpArrow:
//                        selectedIndex = (selectedIndex - 1 + options.Length) % options.Length; 
//                        break;
//                    case ConsoleKey.DownArrow:
//                        selectedIndex = (selectedIndex + 1) % options.Length; 
//                        break;
//                    case ConsoleKey.Enter:
//                        ExecuteOption(selectedIndex); 
//                        return;
//                    case ConsoleKey.Escape:
//                        Console.WriteLine("Returning to Main Menu...");
//                        return;
//                }
//            }
//        }

//        private void ExecuteOption(int selectedIndex)
//        {
//            Console.Clear();

//            switch (selectedIndex)
//            {
//                case 0:
//                    _roomController.RegisterANewRoom();
//                    break;
//                case 1:
//                    _roomController.EditRoom();
//                    break;
//                case 2:
//                    _roomController.ViewAllRooms();
//                    break;
//                case 3:
//                    _roomController.DeleteRoom();
//                    break;
//                case 4:
//                    Console.WriteLine("Returning to Main Menu...");
//                    return;
//                default:
//                    Console.WriteLine("Invalid choice. Try again.");
//                    break;
//            }

//            Console.WriteLine("\nPress any key to return to menu...");
//            Console.ReadKey(true);
//        }
//    }
//}
