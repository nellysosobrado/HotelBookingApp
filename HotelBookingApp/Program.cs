using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace HotelBookingApp
{
    internal class Program
    {
        private static ServiceProvider serviceProvider;
        static void Main(string[] args)
        {
            //Lods the configuration from appsettings.json
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            //Creates service provider
            serviceProvider = new ServiceCollection()
                .AddDbContext<AppDbContext>(options =>
                    options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")))
                .BuildServiceProvider();

            RunMenu();
        }

        static void RunMenu()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("HOTEL BOOKING APP");
                Console.WriteLine("Choose a option:");
                Console.WriteLine("1. Display all rooms");
                Console.WriteLine("2. Add new room");
                Console.WriteLine("3. Exit");

                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        ShowAllRooms();
                        break;
                    case "2":
                        AddRoom();
                        break;
                    case "3":
                        Console.WriteLine("Exiting..program..");
                        return;
                    default:
                        Console.WriteLine("Error");
                        break;
                }

                Console.WriteLine("Enter any key to continue");
                Console.ReadKey();
            }
        }

        static void ShowAllRooms()
        {
            using (var context = serviceProvider.GetService<AppDbContext>())
            {
                var rooms = context.Rooms.ToList();
                Console.WriteLine("List of all the rooms:");
                foreach (var room in rooms)
                {
                    Console.WriteLine($"Rum ID: {room.RoomId}, Typ: {room.Type}, Extrasängar: {room.ExtraBeds}");
                }
            }
        }

        static void AddRoom()
        {
            using (var context = serviceProvider.GetService<AppDbContext>())
            {
                Console.WriteLine("Enter type of room (Single/Double):");
                var type = Console.ReadLine();

                Console.WriteLine("How many extra beds would you like to add (1-2)?:");
                var extraBeds = int.Parse(Console.ReadLine());

                var newRoom = new Room { Type = type, ExtraBeds = extraBeds, IsAvailable = true };
                context.Rooms.Add(newRoom);
                context.SaveChanges();
                Console.WriteLine("Rum tillagt!");
            }
        }
    }

}
