using Autofac;
using Autofac.Extensions.DependencyInjection;
using HotelBookingApp.Data;
using HotelBookingApp.DI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace HotelBookingApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            using (var scope = host.Services.CreateScope())
            {

                var dbContext = scope.ServiceProvider.GetService<AppDbContext>();

                try
                {
                    if (dbContext.Database.CanConnect())
                    {
                        Console.WriteLine("Connected to the database successfully.");
                        // Tilldela rum till befintliga gäster
                        ExistingGuests.AssignRoomsToExistingGuests(dbContext);
                    }
                    else
                    {
                        Console.WriteLine("Could not connect to the database.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred while connecting to the database: {ex.Message}");
                }

                Console.WriteLine("Press any key to continue...");
                Console.ReadKey(); // Vänta på att användaren trycker på en tangent

                var app = scope.ServiceProvider.GetService<HotelBookingApp>();
                app?.Run();
            }
        }

        static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                .ConfigureContainer<ContainerBuilder>(builder =>
                {
                    builder.RegisterModule(new DependencyRegistrar());
                });
    }
}
