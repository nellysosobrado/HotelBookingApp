﻿using System;
using System.Linq;

namespace HotelBookingApp
{
    public class Admin
    {
        private readonly AppDbContext _context;

        public Admin(AppDbContext context)
        {
            _context = context;
        }

        public void Run()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== ADMIN MENU ===");
                Console.WriteLine("Choose an option:");
                Console.WriteLine("1. Add Room");
                Console.WriteLine("2. View All Rooms");
                Console.WriteLine("3. Back to Main Menu");
                Console.WriteLine("===================");

                var choice = Console.ReadLine()?.Trim();

                switch (choice)
                {
                    case "1":
                        AddRoom();
                        break;
                    case "2":
                        ViewAllRooms();
                        break;
                    case "3":
                        Console.WriteLine("Returning to main menu...");
                        return; // Avslutar metoden och återgår till huvudmenyn
                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }

                PromptToContinue();
            }
        }

        public void AddRoom()
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

            var newRoom = new Room
            {
                Type = char.ToUpper(roomType[0]) + roomType.Substring(1), // Formatera till "Single" eller "Double"
                PricePerNight = price,
                SizeInSquareMeters = size,
                ExtraBeds = extraBeds,
                IsAvailable = true
            };

            _context.Rooms.Add(newRoom);
            _context.SaveChanges();

            Console.WriteLine($"Room of type '{newRoom.Type}' successfully added with ID {newRoom.RoomId}.");
        }


        public void ViewAllRooms()
        {
            Console.Clear();
            Console.WriteLine("=== VIEW ALL ROOMS ===");

            var rooms = _context.Rooms.ToList();
            if (!rooms.Any())
            {
                Console.WriteLine("No rooms available.");
                return;
            }

            const int pageSize = 5; // Antal rum per sida
            int currentPage = 0;
            int totalPages = (int)Math.Ceiling((double)rooms.Count / pageSize);

            while (true)
            {
                Console.Clear();
                Console.WriteLine($"=== VIEW ALL ROOMS (Page {currentPage + 1}/{totalPages}) ===");

                var roomsOnPage = rooms
                    .Skip(currentPage * pageSize)
                    .Take(pageSize)
                    .ToList();

                foreach (var room in roomsOnPage)
                {
                    Console.WriteLine($"Room ID: {room.RoomId}");
                    Console.WriteLine($"Type: {room.Type}");
                    Console.WriteLine($"Price Per Night: {room.PricePerNight:C}");
                    Console.WriteLine($"Size: {room.SizeInSquareMeters} sqm");
                    Console.WriteLine($"Extra Beds: {room.ExtraBeds}");
                    Console.WriteLine($"Available: {(room.IsAvailable ? "Yes" : "No")}");
                    Console.WriteLine("-------------------");
                }

                Console.WriteLine("\nOptions: [N] Next Page | [P] Previous Page | [Q] Quit");
                ConsoleKey input = Console.ReadKey(true).Key; // Läs tangenttryckning utan att visa på skärmen

                switch (input)
                {
                    case ConsoleKey.N:
                        if (currentPage < totalPages - 1)
                        {
                            currentPage++;
                        }
                        else
                        {
                            Console.WriteLine("You are on the last page. Press any key to continue...");
                            Console.ReadKey(true);
                        }
                        break;
                    case ConsoleKey.P:
                        if (currentPage > 0)
                        {
                            currentPage--;
                        }
                        else
                        {
                            Console.WriteLine("You are on the first page. Press any key to continue...");
                            Console.ReadKey(true);
                        }
                        break;
                    case ConsoleKey.Q:
                        Console.WriteLine("Exiting room view...");
                        return; // Gå tillbaka till huvudmenyn
                    default:
                        Console.WriteLine("Invalid choice. Please use [N], [P], or [Q]. Press any key to continue...");
                        Console.ReadKey(true);
                        break;
                }
            }
        }




        private void PromptToContinue()
        {
            Console.WriteLine("\nPress any key to return to the menu...");
            Console.ReadKey();
        }
    }
}
