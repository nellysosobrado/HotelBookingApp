using Autofac;
using HotelBookingApp.Controllers;
using HotelBookingApp.Data;
using HotelBookingApp.Entities;
using HotelBookingApp.Repositories;
using HotelBookingApp.Services.BookingServices;
using HotelBookingApp.Services.DisplayServices;
using HotelBookingApp.Services.GuestServices;
using HotelBookingApp.Services.RoomServices;
using HotelBookingApp.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Spectre.Console;
using System;

namespace HotelBookingApp.DI
{
    public class DependencyRegistrar : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("Data/appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            builder.Register(context =>
            {
                var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
                optionsBuilder.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));

                var dbContext = new AppDbContext(optionsBuilder.Options);

                try
                {
                    if (dbContext.Database.EnsureCreated())
                    {
                        AnsiConsole.MarkupLine("[bold green]Database created successfully.![/]");
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred while ensuring the database: {ex.Message}");
                }

                return dbContext;
            }).As<AppDbContext>().InstancePerLifetimeScope();


            builder.RegisterType<DisplayMainMenu>().AsSelf();
            builder.RegisterType<App>().AsSelf();
            builder.RegisterType<BookingController>().AsSelf();
            builder.RegisterType<BookingRepository>().AsSelf();

            builder.RegisterType<GuestRepository>().AsSelf();
            builder.RegisterType<GuestController>().AsSelf();
            builder.RegisterType<RoomController>().AsSelf();
            builder.RegisterType<RoomRepository>().AsSelf();


            builder.RegisterType<FindRoomByDate>().AsSelf();
            builder.RegisterType<FindRoomByTotalPeople>().AsSelf();
            builder.RegisterType<DeleteRoomService>().AsSelf();
            builder.RegisterType<EditRoomService>().AsSelf();
            builder.RegisterType<RegisterRoomService>().AsSelf();
            builder.RegisterType<TableDisplayService>().AsSelf();

            builder.RegisterType<CheckInOutService>().AsSelf();
            builder.RegisterType<GuestRegistrationService>().AsSelf();
            builder.RegisterType<BookingEditService>().AsSelf();
            builder.RegisterType<UnbookBooking>().AsSelf();
            builder.RegisterType<PaymentService>().AsSelf();
            builder.RegisterType<GuestRemovalService>().AsSelf();
            builder.RegisterType<DisplayRegisteredGuestsService>().AsSelf();

            builder.RegisterType<UnpaidBookingService>().AsSelf();

            builder.Register(ctx => new List<Booking>()).As<List<Booking>>();




        }
    }
}
