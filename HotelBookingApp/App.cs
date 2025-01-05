using Spectre.Console;
using HotelBookingApp.Data;
using HotelBookingApp.Services.BookingServices;
using HotelBookingApp.Utilities;

namespace HotelBookingApp
{
    public class App
    {
        private readonly AppDbContext _dbContext;
        private readonly MainMenu _displayMainMenu;

        public App(AppDbContext dbContext, MainMenu mainMenu)
        {
            _dbContext = dbContext;
            _displayMainMenu = mainMenu;
        }

        public void Run()
        {
            CheckDatabaseConnection();

            _displayMainMenu.MenuOptions();
        }

        private void CheckDatabaseConnection()
        {
            try
            {
                if (_dbContext.Database.CanConnect())
                {
                    AnsiConsole.MarkupLine("[bold green]Connected to the database successfully![/]");
                    Console.WriteLine("Notification: 4 Seeded guests, 4 Seeded rooms has been created as test subjects.");
                }
                else
                {
                    AnsiConsole.MarkupLine("[bold red]Could not connect to the database.[/]");
                }
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[bold red]An error occurred: {ex.Message}[/]");
            }

            AnsiConsole.MarkupLine("[italic purple]Setup complete, Press any key to continue...[/]");
            Console.ReadKey();
        }
    }

}
