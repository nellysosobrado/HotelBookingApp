using HotelBookingApp.Entities;
using HotelBookingApp.Repositories;
using System;
using System.Linq;

namespace HotelBookingApp.Controllers
{
    public class RoomController
    {
        private readonly RoomRepository _roomRepository;

        public RoomController(RoomRepository roomRepository)
        {
            _roomRepository = roomRepository;
        }

        public void RegisterNewRoom()
        {
            Console.Clear();
            Console.WriteLine("=== REGISTER NEW ROOM ===");

            Console.Write("Enter Room Type (Single/Double): ");
            var roomType = Console.ReadLine()?.Trim();

            if (string.IsNullOrWhiteSpace(roomType) ||
                (roomType.ToLower() != "single" && roomType.ToLower() != "double"))
            {
                Console.WriteLine("Invalid Room Type. Please enter 'Single' or 'Double'.");
                return;
            }

            Console.Write("Enter Price Per Night: ");
            if (!decimal.TryParse(Console.ReadLine(), out var price) || price <= 0)
            {
                Console.WriteLine("Invalid price.");
                return;
            }

            Console.Write("Enter Size in Square Meters: ");
            if (!int.TryParse(Console.ReadLine(), out var size) || size <= 0)
            {
                Console.WriteLine("Invalid size.");
                return;
            }

            int extraBeds = 0;
            int maxPeople = 0;

            if (roomType.ToLower() == "double")
            {
                Console.Write("Enter Number of Extra Beds (0, 1, or 2): ");
                if (!int.TryParse(Console.ReadLine(), out extraBeds) || extraBeds < 0 || extraBeds > 2)
                {
                    Console.WriteLine("Invalid number of extra beds. Allowed values for Double rooms are 0, 1, or 2.");
                    return;
                }
                maxPeople = 2 + extraBeds; 
            }
            else if (roomType.ToLower() == "single")
            {
                extraBeds = 0;
                maxPeople = 1; 
            }

            var newRoom = new Room
            {
                Type = char.ToUpper(roomType[0]) + roomType.Substring(1).ToLower(),
                PricePerNight = price,
                SizeInSquareMeters = size,
                ExtraBeds = extraBeds,
                IsAvailable = true,
                TotalPeople = maxPeople
            };

            _roomRepository.AddRoom(newRoom);

            Console.WriteLine($"\nRoom '{newRoom.Type}' successfully added with ID {newRoom.RoomId}.");
            Console.WriteLine($"Max People Allowed: {newRoom.TotalPeople}");
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }


        public void EditRoom()
        {
            Console.Clear();
            Console.WriteLine("=== EDIT ROOM ===");

            Console.Write("Enter Room ID: ");
            if (!int.TryParse(Console.ReadLine(), out var roomId))
            {
                Console.WriteLine("Invalid Room ID.");
                return;
            }

            var room = _roomRepository.GetRoomById(roomId);
            if (room == null)
            {
                Console.WriteLine("Room not found.");
                return;
            }

            Console.WriteLine("Leave blank to keep current value.");

            Console.Write($"Current Type: {room.Type}\nEnter new Type: ");
            var newType = Console.ReadLine()?.Trim();
            if (!string.IsNullOrWhiteSpace(newType))
                room.Type = newType;

            Console.Write($"Current Price: {room.PricePerNight}\nEnter new Price: ");
            if (decimal.TryParse(Console.ReadLine(), out var price))
                room.PricePerNight = price;

            Console.Write($"Current Size: {room.SizeInSquareMeters}\nEnter new Size: ");
            if (int.TryParse(Console.ReadLine(), out var size))
                room.SizeInSquareMeters = size;

            _roomRepository.UpdateRoom(room);
            Console.WriteLine("Room updated successfully.");
        }

        public void ViewAllRooms()
        {
            Console.Clear();
            var rooms = _roomRepository.GetRoomsWithBookings();

            if (!rooms.Any())
            {
                Console.WriteLine("No rooms found.");
                return;
            }

            foreach (var entry in rooms)
            {
                var room = entry.Room;
                var bookedBy = entry.Booking?.Guest != null
                    ? $"{entry.Booking.Guest.FirstName} {entry.Booking.Guest.LastName}"
                    : "Not Booked";

                Console.WriteLine($"ID: {room.RoomId} | Type: {room.Type} | Price: {room.PricePerNight:C} | Booked By: {bookedBy}");
            }
            Console.WriteLine("press any key to contionue");
            Console.ReadKey();
        }
        public void DeleteRoom()
        {
            Console.Clear();
            Console.WriteLine("DELETE ROOM");

            Console.Write("Enter Room ID to delete: ");
            if (!int.TryParse(Console.ReadLine(), out var roomId))
            {
                Console.WriteLine("Invalid Room ID.");
                return;
            }

            var room = _roomRepository.GetRoomById(roomId);
            if (room == null)
            {
                Console.WriteLine("Room not found.");
                return;
            }

            Console.WriteLine($"Are you sure you want to delete Room ID {room.RoomId} (Type: {room.Type})? (Y/N)");
            var confirmation = Console.ReadLine()?.Trim().ToUpper();

            if (confirmation == "Y")
            {
                _roomRepository.DeleteRoom(roomId);
                Console.WriteLine($"Room ID {roomId} has been successfully deleted.");
            }
            else
            {
                Console.WriteLine("Operation cancelled.");
            }
        }

    }
}
