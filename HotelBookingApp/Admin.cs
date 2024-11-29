using System;

namespace HotelBookingApp
{
    public class Admin
    {
        private readonly AppDbContext _context;

        public Admin(AppDbContext context)
        {
            _context = context;
        }
        public void Run() { }
        public void AddRoom()
        {
            Console.WriteLine("Enter the type of room (Single/Double):");
            var roomType = Console.ReadLine();

            Console.WriteLine("Enter the price per night:");
            if (!decimal.TryParse(Console.ReadLine(), out decimal price))
            {
                Console.WriteLine("Invalid price. Room creation failed.");
                return;
            }

            Console.WriteLine("Enter the size of the room in square meters:");
            if (!int.TryParse(Console.ReadLine(), out int size))
            {
                Console.WriteLine("Invalid size. Room creation failed.");
                return;
            }

            Console.WriteLine("Enter the number of extra beds (0-2):");
            if (!int.TryParse(Console.ReadLine(), out int extraBeds) || extraBeds < 0 || extraBeds > 2)
            {
                Console.WriteLine("Invalid number of extra beds. Room creation failed.");
                return;
            }

           
            var newRoom = new Room
            {
                Type = roomType,
                PricePerNight = price,
                SizeInSquareMeters = size,
                ExtraBeds = extraBeds,
                IsAvailable = true 
            };

            _context.Rooms.Add(newRoom);
            _context.SaveChanges();

            Console.WriteLine($"Room of type '{roomType}' successfully added with ID {newRoom.RoomId}.");
        }
    }
}
