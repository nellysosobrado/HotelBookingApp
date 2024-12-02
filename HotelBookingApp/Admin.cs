using Microsoft.EntityFrameworkCore;
using System;
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
                DisplayMenu();

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
                        EditGuest(); // Ny funktion för att redigera gäster
                        break;
                    case "5":
                        Console.WriteLine("Returning to main menu...");
                        return;
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
            Console.WriteLine("4. Edit Guest"); // Nytt alternativ
            Console.WriteLine("5. Back to Main Menu");
            Console.WriteLine("===================");
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
                Console.WriteLine("Do you want to change the number of extra beds? (yes/no):");
                var changeExtraBeds = Console.ReadLine()?.Trim().ToLower();

                if (changeExtraBeds == "yes")
                {
                    Console.WriteLine("Enter the new number of extra beds (0-2):");
                    var extraBedsInput = Console.ReadLine()?.Trim();
                    if (!string.IsNullOrEmpty(extraBedsInput) && int.TryParse(extraBedsInput, out int extraBeds))
                    {
                        if (room.SizeInSquareMeters < 40 && extraBeds > 1)
                        {
                            Console.WriteLine("Rooms smaller than 40 m² can only have 1 extra bed. No changes made to extra beds.");
                        }
                        else if (extraBeds >= 0 && extraBeds <= 2)
                        {
                            room.ExtraBeds = extraBeds;
                        }
                        else
                        {
                            Console.WriteLine("Invalid number of extra beds. No changes made to extra beds.");
                        }
                    }
                }
            }
            else
            {
                Console.WriteLine("Extra beds can only be added to Double rooms.");
                room.ExtraBeds = 0; // För säkerhet, nollställ extra sängar om rummet är Single
            }

            _context.SaveChanges();
            Console.WriteLine($"Room with ID {room.RoomId} successfully updated.");
        }



        public void EditGuest()
        {
            Console.Clear();
            Console.WriteLine("=== EDIT GUEST ===");

            Console.WriteLine("Enter the Guest ID to edit:");
            if (!int.TryParse(Console.ReadLine(), out int guestId))
            {
                Console.WriteLine("Invalid Guest ID.");
                return;
            }

            var guest = _context.Guests.FirstOrDefault(g => g.GuestId == guestId);
            if (guest == null)
            {
                Console.WriteLine("Guest not found.");
                return;
            }

            Console.WriteLine("Leave a field empty to keep the current value.");

            // Uppdatera förnamn
            Console.WriteLine($"Current First Name: {guest.FirstName}");
            Console.WriteLine("Enter the new first name:");
            var firstName = Console.ReadLine()?.Trim();
            if (!string.IsNullOrEmpty(firstName))
            {
                guest.FirstName = firstName;
            }

            // Uppdatera efternamn
            Console.WriteLine($"Current Last Name: {guest.LastName}");
            Console.WriteLine("Enter the new last name:");
            var lastName = Console.ReadLine()?.Trim();
            if (!string.IsNullOrEmpty(lastName))
            {
                guest.LastName = lastName;
            }

            // Uppdatera e-postadress
            Console.WriteLine($"Current Email: {guest.Email}");
            Console.WriteLine("Enter the new email:");
            var email = Console.ReadLine()?.Trim();
            if (!string.IsNullOrEmpty(email))
            {
                guest.Email = email;
            }

            // Uppdatera telefonnummer
            Console.WriteLine($"Current Phone Number: {guest.PhoneNumber}");
            Console.WriteLine("Enter the new phone number:");
            var phoneNumber = Console.ReadLine()?.Trim();
            if (!string.IsNullOrEmpty(phoneNumber))
            {
                guest.PhoneNumber = phoneNumber;
            }

            _context.SaveChanges();
            Console.WriteLine($"Guest with ID {guest.GuestId} successfully updated.");
        }

        public void ViewAllRooms()
        {
            Console.Clear();
            Console.WriteLine("=== VIEW ALL ROOMS ===");

            var rooms = _context.Rooms
                .GroupJoin(
                    _context.Bookings.Include(b => b.Guest), // Inkludera gästinformation
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

            Console.WriteLine(new string('-', 100));
            Console.WriteLine($"{"Room ID",-10}{"Type",-15}{"Price/Night",-15}{"Size m²",-10}{"Availability",-15}{"Booked By",-20}");
            Console.WriteLine(new string('-', 100));

            foreach (var entry in rooms)
            {
                var room = entry.Room;
                var bookedBy = entry.Booking?.Guest != null
                    ? $"{entry.Booking.Guest.FirstName} {entry.Booking.Guest.LastName}"
                    : "Not Booked";

                Console.WriteLine($"{room.RoomId,-10}{room.Type,-15}{room.PricePerNight,-15:C}{room.SizeInSquareMeters,-10}{(room.IsAvailable ? "Available" : "Not Available"),-15}{bookedBy,-20}");
            }

            Console.WriteLine(new string('-', 100));
        }

        private void PromptToContinue()
        {
            Console.WriteLine("\nPress any key to return to the menu...");
            Console.ReadKey();
        }
    }
}
