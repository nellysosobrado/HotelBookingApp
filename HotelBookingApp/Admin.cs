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
                Console.WriteLine("1. Register a Room");
                Console.WriteLine("2. View All Rooms");
                Console.WriteLine("3. Edit Room");
                Console.WriteLine("4. Back to Main Menu");
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
                        EditRoom();
                        break;
                    case "4":
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

        public void EditRoom()
        {
            Console.Clear();
            Console.WriteLine("=== EDIT ROOM ===");

            Console.WriteLine("Enter the Room ID to edit:");
            if (!int.TryParse(Console.ReadLine(), out int roomId))
            {
                Console.WriteLine("Invalid Room ID.");
                return;
            }

            var room = _context.Rooms.FirstOrDefault(r => r.RoomId == roomId);
            if (room == null)
            {
                Console.WriteLine("Room not found.");
                return;
            }

            Console.WriteLine("Leave a field empty to keep the current value.");
            Console.WriteLine($"Current Type: {room.Type}");
            Console.WriteLine("Enter the new type of room (Single/Double):");
            var roomType = Console.ReadLine()?.Trim();
            if (!string.IsNullOrEmpty(roomType) && (roomType.ToLower() == "single" || roomType.ToLower() == "double"))
            {
                room.Type = char.ToUpper(roomType[0]) + roomType.Substring(1);
            }

            Console.WriteLine($"Current Price Per Night: {room.PricePerNight}");
            Console.WriteLine("Enter the new price per night:");
            var priceInput = Console.ReadLine()?.Trim();
            if (!string.IsNullOrEmpty(priceInput) && decimal.TryParse(priceInput, out decimal price) && price > 0)
            {
                room.PricePerNight = price;
            }

            Console.WriteLine($"Current Size (m²): {room.SizeInSquareMeters}");
            Console.WriteLine("Enter the new size in square meters:");
            var sizeInput = Console.ReadLine()?.Trim();
            if (!string.IsNullOrEmpty(sizeInput) && int.TryParse(sizeInput, out int size) && size > 0)
            {
                room.SizeInSquareMeters = size;
            }

            if (room.Type.Equals("Double", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine($"Current Extra Beds: {room.ExtraBeds}");
                Console.WriteLine("Enter the new number of extra beds (0-2):");
                var extraBedsInput = Console.ReadLine()?.Trim();
                if (!string.IsNullOrEmpty(extraBedsInput) && int.TryParse(extraBedsInput, out int extraBeds) && extraBeds >= 0 && extraBeds <= 2)
                {
                    room.ExtraBeds = extraBeds;
                }
            }

            _context.SaveChanges();
            Console.WriteLine($"Room with ID {room.RoomId} successfully updated.");
        }

        public void ViewAllRooms()
        {
            Console.Clear();
            Console.WriteLine("=== VIEW ALL ROOMS ===");

            var rooms = _context.Rooms
                .GroupJoin(
                    _context.Bookings,
                    room => room.RoomId,
                    booking => booking.RoomId,
                    (room, bookings) => new
                    {
                        Room = room,
                        Booking = bookings.FirstOrDefault() // Hämtar första bokningen för rummet (om någon)
                    })
                .ToList();

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
                Console.WriteLine(new string('-', 90));
                Console.WriteLine($"{"Room ID",-10}{"Type",-15}{"Price/Night",-15}{"Size m²",-10}{"Max People",-12}{"Booked By",-20}");
                Console.WriteLine(new string('-', 90));

                var roomsOnPage = rooms
                    .Skip(currentPage * pageSize)
                    .Take(pageSize)
                    .ToList();

                foreach (var entry in roomsOnPage)
                {
                    var room = entry.Room;
                    var booking = entry.Booking;

                    // Bestäm vem som har bokat rummet
                    var bookedBy = booking != null
                        ? $"{booking.Guest.FirstName} {booking.Guest.LastName}" // Gästens namn
                        : "Not Booked"; // Ingen bokning

                    // Bestäm max antal personer
                    var maxPeople = room.Type.Equals("Double", StringComparison.OrdinalIgnoreCase) ? 4 : 2;

                    Console.WriteLine($"{room.RoomId,-10}{room.Type,-15}{room.PricePerNight,-15:C}{room.SizeInSquareMeters + " m²",-10}{maxPeople,-12}{bookedBy,-20}");
                }

                Console.WriteLine(new string('-', 90));
                Console.WriteLine("\nOptions: [N] Next Page | [P] Previous Page | [Q] Quit");
                ConsoleKey input = Console.ReadKey(true).Key; 

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
                        return; 
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
