﻿using Autofac;
using HotelBookingApp.Data;
using HotelBookingApp.Services.BookingService;
using HotelBookingApp.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
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


            builder.RegisterType<DisplayMainMenu>().AsSelf();
            builder.RegisterType<App>().AsSelf();
            builder.RegisterType<BookingService>().AsSelf();
            builder.RegisterType<RoomService>().AsSelf();
            builder.RegisterType<GuestServices>().AsSelf();
            builder.RegisterType<DisplayBookingMenu>().AsSelf();
            builder.RegisterType<BookingController>().AsSelf();
            builder.RegisterType<BookingRepository>().AsSelf();

        }
    }
}
