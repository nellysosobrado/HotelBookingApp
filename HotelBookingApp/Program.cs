using Autofac;
using Autofac.Extensions.DependencyInjection;
using HotelBookingApp.Data;
using HotelBookingApp.DI;
using HotelBookingApp.Services.BookingServices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Spectre.Console;

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
                        AnsiConsole.MarkupLine("[bold green]Connected to the database successfully.![/]");
                        SeededBookingAssignerService.AssignRoomsToExistingGuests(dbContext);
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
                AnsiConsole.MarkupLine("[italic purple]Setup complete, Press any key to continue...![/]");
                Console.ReadKey(); 

                var app = scope.ServiceProvider.GetService<App>();
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
