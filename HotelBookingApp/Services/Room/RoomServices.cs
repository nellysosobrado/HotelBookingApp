using System;
using System.Linq;
using HotelBookingApp.Data;
using HotelBookingApp.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HotelBookingApp
{
    public class RoomService : IMenu, IMenuNavigation
    {
        private readonly AppDbContext _context;

        public RoomService(AppDbContext context)
        {
            _context = context;
        }
        public void Menu()
        {
            string[] options = { "Register New Room", "Edit Room", "View All Rooms", "Main Menu" };

            while (true)
            {
                
                int selectedOption = NavigateMenu(options);

                Console.Clear();
                
                switch (selectedOption)
                {
                    case 0:
                        RegisterNewRoom();
                        break;
                    case 1:
                        EditRoom();
                        break;
                    case 2:
                        ViewAllRooms();
                        break;
                    case 3:
                        Console.WriteLine("Exiting menu...");
                        return; 
                }

                // Ge användaren tid att se resultatet innan menyn visas igen
                Console.WriteLine("\nPress any key to return to the menu...");
                Console.ReadKey(true);
            }
        }
        public int NavigateMenu(string[] options)
        {
            int selectedOption = 0;

            while (true)
            {
                Console.Clear();
                Console.WriteLine("RoomServices.cs");

                // Visa alternativen och markera det valda alternativet
                for (int i = 0; i < options.Length; i++)
                {
                    if (i == selectedOption)
                    {
                        Console.ForegroundColor = ConsoleColor.Green; // Markera det valda alternativet
                        Console.WriteLine($"> {options[i]}");
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.WriteLine($"  {options[i]}");
                    }
                }

                // Hantera knapptryckningar
                ConsoleKey key = Console.ReadKey(true).Key;
                switch (key)
                {
                    case ConsoleKey.UpArrow:
                        selectedOption = (selectedOption - 1 + options.Length) % options.Length; // Flytta upp
                        break;
                    case ConsoleKey.DownArrow:
                        selectedOption = (selectedOption + 1) % options.Length; // Flytta ner
                        break;
                    case ConsoleKey.Enter:
                        return selectedOption; // Returnera det valda alternativet
                }
            }
        }



        public void AddRoom(string roomType, decimal price, int size, int extraBeds)
        {
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
        public void RegisterNewRoom()
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

            // Hämta rummet från databasen
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
            if (!string.IsNullOrEmpty(roomType))
            {
                room.Type = char.ToUpper(roomType[0]) + roomType.Substring(1); // Formatera till "Single" eller "Double"
            }

            Console.WriteLine($"Current Price Per Night: {room.PricePerNight}");
            Console.WriteLine("Enter the new price per night:");
            var priceInput = Console.ReadLine()?.Trim();
            if (!string.IsNullOrEmpty(priceInput) && decimal.TryParse(priceInput, out var parsedPrice))
            {
                room.PricePerNight = parsedPrice;
            }

            Console.WriteLine($"Current Size (m²): {room.SizeInSquareMeters}");
            Console.WriteLine("Enter the new size in square meters:");
            var sizeInput = Console.ReadLine()?.Trim();
            if (!string.IsNullOrEmpty(sizeInput) && int.TryParse(sizeInput, out var parsedSize))
            {
                room.SizeInSquareMeters = parsedSize;
            }

            Console.WriteLine($"Current Extra Beds: {room.ExtraBeds}");
            Console.WriteLine("Enter the number of extra beds (if applicable):");
            var extraBedsInput = Console.ReadLine()?.Trim();
            if (!string.IsNullOrEmpty(extraBedsInput) && int.TryParse(extraBedsInput, out var parsedBeds))
            {
                room.ExtraBeds = parsedBeds;
            }

            // Spara ändringar
            _context.SaveChanges();

            Console.WriteLine($"Room with ID {room.RoomId} successfully updated.");
        }




        public void ViewAllRooms()
        {
            var rooms = _context.Rooms
                .GroupJoin(
                    _context.Bookings.Include(b => b.Guest),
                    room => room.RoomId,
                    booking => booking.RoomId,
                    (room, bookings) => new
                    {
                        Room = room,
                        Booking = bookings.FirstOrDefault()
                    })
                .ToList();

            if (!rooms.Any())
            {
                Console.WriteLine("No rooms available.");
                return;
            }

            const int pageSize = 5; // Antal rader per sida
            int currentPage = 0;
            int totalPages = (int)Math.Ceiling((double)rooms.Count / pageSize);

            while (true)
            {
                Console.Clear();
                Console.WriteLine($"=== VIEW ALL ROOMS (Page {currentPage + 1} of {totalPages}) ===");

                Console.WriteLine(new string('-', 100));
                Console.WriteLine($"{"Room ID",-10}{"Type",-15}{"Price/Night",-15}{"Size m²",-10}{"Availability",-15}{"Booked By",-20}");
                Console.WriteLine(new string('-', 100));

                var roomsOnPage = rooms.Skip(currentPage * pageSize).Take(pageSize);

                foreach (var entry in roomsOnPage)
                {
                    var room = entry.Room;
                    var bookedBy = entry.Booking?.Guest != null
                        ? $"{entry.Booking.Guest.FirstName} {entry.Booking.Guest.LastName}"
                        : "Not Booked";

                    Console.WriteLine($"{room.RoomId,-10}{room.Type,-15}{room.PricePerNight,-15:C}{room.SizeInSquareMeters,-10}{(room.IsAvailable ? "Available" : "Not Available"),-15}{bookedBy,-20}");
                }

                Console.WriteLine(new string('-', 100));
                Console.WriteLine("\nOptions: [N] Next Page | [P] Previous Page | [Q] Quit");

                var key = Console.ReadKey(true).Key;

                switch (key)
                {
                    case ConsoleKey.N:
                        if (currentPage < totalPages - 1)
                        {
                            currentPage++;
                        }
                        else
                        {
                            Console.WriteLine("You are on the last page.");
                            Console.WriteLine("Press any key to continue...");
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
                            Console.WriteLine("You are on the first page.");
                            Console.WriteLine("Press any key to continue...");
                            Console.ReadKey(true);
                        }
                        break;

                    case ConsoleKey.Q:
                        Console.WriteLine("Exiting room view...");
                        return;

                    default:
                        Console.WriteLine("Invalid input. Please use [N], [P], or [Q].");
                        Console.WriteLine("Press any key to continue...");
                        Console.ReadKey(true);
                        break;
                }
            }
        }

    }
}
