using Autofac;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

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
                return new AppDbContext(optionsBuilder.Options);
            }).As<AppDbContext>().InstancePerLifetimeScope();

            // Registrera tjänster och huvudklasser
            builder.RegisterType<MainMenu>().AsSelf();
            builder.RegisterType<HotelBookingApp>().AsSelf();
            builder.RegisterType<BookingManager>().AsSelf();
        }
    }
}
