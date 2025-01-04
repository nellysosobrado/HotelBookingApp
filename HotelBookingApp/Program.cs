using Autofac;
using Autofac.Extensions.DependencyInjection;
using HotelBookingApp.DI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace HotelBookingApp
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            using (var scope = host.Services.CreateScope())
            {
                // Hämtar App-instansen via DI
                var app = scope.ServiceProvider.GetService<App>();
                app?.Run();
            }
        }

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                .ConfigureContainer<ContainerBuilder>(builder =>
                {
                    // Registrerar modulen som hanterar beroenden
                    builder.RegisterModule(new DependencyRegistrar());
                });
    }
}
