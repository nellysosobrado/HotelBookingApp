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

            Console.Write("Enter Price Per Night: ");
            if (!decimal.TryParse(Console.ReadLine(), out var price))
            {
                Console.WriteLine("Invalid price.");
                return;
            }

            Console.Write("Enter Size in Square Meters: ");
            if (!int.TryParse(Console.ReadLine(), out var size))
            {
                Console.WriteLine("Invalid size.");
                return;
            }

            Console.Write("Enter Number of Extra Beds: ");
            if (!int.TryParse(Console.ReadLine(), out var extraBeds))
            {
                Console.WriteLine("Invalid extra beds.");
                return;
            }

            var newRoom = new Room
            {
                Type = char.ToUpper(roomType[0]) + roomType.Substring(1),
                PricePerNight = price,
                SizeInSquareMeters = size,
                ExtraBeds = extraBeds,
                IsAvailable = true
            };

            _roomRepository.AddRoom(newRoom);

            Console.WriteLine($"Room '{newRoom.Type}' successfully added with ID {newRoom.RoomId}.");
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
        }
    }
}
