using Autofac;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;

namespace HotelBookingApp
{
    public class DependencyRegistrar : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            // Registrera DbContext
            builder.Register(context =>
            {
                var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
                optionsBuilder.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));

                var dbContext = new AppDbContext(optionsBuilder.Options);

                // Kontrollera och skapa databasen om den inte existerar
                try
                {
                    if (dbContext.Database.EnsureCreated())
                    {
                        Console.WriteLine("Database created successfully.");
                    }
                    else
                    {
                        Console.WriteLine("Database already exists.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred while ensuring the database: {ex.Message}");
                }

                return dbContext;
            }).As<AppDbContext>().InstancePerLifetimeScope();


            // Registrera tjänster och huvudklasser
            builder.RegisterType<MainMenuManager>().AsSelf();
            builder.RegisterType<HotelBookingApp>().AsSelf();
            builder.RegisterType<BookingService>().AsSelf();
            builder.RegisterType<RegisterNewBooking>().AsSelf();
            builder.RegisterType<Admin>().AsSelf();
            builder.RegisterType<RoomService>().AsSelf();
            builder.RegisterType<GuestServices>().AsSelf();

        }
    }
}
