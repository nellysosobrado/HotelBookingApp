using Microsoft.EntityFrameworkCore;//Makes it possible to use the databse that i am using from SSMS
using Microsoft.Extensions.Configuration; //Makes it possible to load my configuration information that I have stored in my Json File
using Microsoft.Extensions.DependencyInjection; // Makes it possible to use the build in dependency injection
using System.Threading;
using System;

namespace HotelBookingApp
{
    internal class Program
    {
        private static ServiceProvider serviceProvider;
        static void Main(string[] args)
        {
            try 
            {
                Console.WriteLine("Loads the configuration from appsettings.json file...");
                Thread.Sleep(5000);

                //Loads, reads my appsettings.json file as a configuration. Sets up
                var configuration = new ConfigurationBuilder()
                    //Json file contains the connectionstring variable, where i stored the information the application needs to connect to the databvase
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .Build();//calls build
                Console.WriteLine("Configuration, has been completed. Trying to connect to the database...");
                Thread.Sleep(5000);


                //Creates service provider
                serviceProvider = new ServiceCollection()
                    .AddDbContext<AppDbContext>(options =>
                        options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")))
                    .BuildServiceProvider();

                //test database connetion
                using (var context = serviceProvider.GetService<AppDbContext>())
                {
                    if(context.Database.CanConnect())
                    {
                        Console.WriteLine("Connected to database succeded!");
                        Thread.Sleep(1000);
                    }
                    else
                    {
                        Console.WriteLine("Could not connect to the database");
                    }
                }
                RunMenu();
            }
            catch(Exception ex)
            {
                Console.WriteLine("Error:" + ex.Message);
            }
            finally
            {
                Console.WriteLine("Program is exiting.. press any key");
                Console.ReadKey();
            }
           


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
