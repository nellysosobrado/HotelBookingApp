using HotelBookingApp.Repositories;
using Microsoft.IdentityModel.Tokens;
using Spectre.Console;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBookingApp.Services.GuestServices
{
    public class DisplayRegisteredGuestsService
    {
        private readonly GuestRepository _guestRepository;
        private readonly BookingRepository _bookingRepository;

        public DisplayRegisteredGuestsService(GuestRepository guestRepository, BookingRepository bookingRepository)
        {
            _guestRepository = guestRepository;
            _bookingRepository = bookingRepository;
        }

        public void DisplayGuests()
        {
            while (true)
            {
                var action = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("[italic yellow]Display guest information[/]")
                        .AddChoices(new[]
                        {
                    "Display deleted guests",
                    "Display registered guests",
                    "Back"
                        })
                        .HighlightStyle(new Style(foreground: Color.Green))
                );

                switch (action)
                {
                    case "Display deleted guests":
                        DisplayDeletedGuests();
                        break;

                    case "Display registered guests":
                        DisplayAllRegisteredGuests();
                        break;

                    case "Back":
                        return; 

                    default:
                        AnsiConsole.Markup("[red]Invalid option. Try again.[/]\n");
                        break;
                }
            }
        }

        public void DisplayDeletedGuests()
        {
            bool keepRunning = true;

            while (keepRunning)
            {
                Console.Clear(); 
                AnsiConsole.MarkupLine("[yellow]Deleted Guests:[/]");

                var deletedGuests = _guestRepository.GetDeletedGuests();

                if (!deletedGuests.Any())
                {
                    AnsiConsole.MarkupLine("[gray]No deleted guests found.[/]");
                    Console.WriteLine("[B] Back");
                    var input = Console.ReadKey(true).Key;

                    if (input == ConsoleKey.B)
                        keepRunning = false;

                    continue;
                }

                var table = new Table()
                    .Border(TableBorder.Rounded)
                    .AddColumn("[bold]Guest ID[/]")
                    .AddColumn("[bold]Name[/]")
                    .AddColumn("[bold]Last Name[/]");

                foreach (var guest in deletedGuests)
                {
                    table.AddRow(
                        guest.GuestId.ToString(),
                        $"{guest.FirstName ?? "[gray]Unknown[/]"}",
                        $"{guest.LastName ?? "[gray]Unknown[/]"}"
                    );
                }

                AnsiConsole.Write(table);

                Console.WriteLine("\n[B] Back");
                var userInput = Console.ReadKey(true).Key;

                if (userInput == ConsoleKey.B) 
                    keepRunning = false;
            }
        }


        public void DisplayAllRegisteredGuests()
        {
            bool keepRunning = true;

            while (keepRunning)
            {
                Console.Clear(); 
                AnsiConsole.MarkupLine("[yellow]Currently Registered Guests:[/]");

                var guests = _guestRepository.GetAllGuests();

                if (!guests.Any())
                {
                    AnsiConsole.MarkupLine("[gray]No registered guests found.[/]");
                    Console.WriteLine("\n[B] Back");
                    var input = Console.ReadKey(true).Key;

                    if (input == ConsoleKey.B) 
                        keepRunning = false;

                    continue; 
                }

                var table = new Table()
                    .Border(TableBorder.Rounded)
                    .AddColumn("[bold]Guest ID[/]")
                    .AddColumn("[bold]Name[/]")
                    .AddColumn("[bold]Email[/]")
                    .AddColumn("[bold]Phone Number[/]")
                    .AddColumn("[bold]Number of Bookings[/]");

                foreach (var guest in guests)
                {
                    int bookingCount = _bookingRepository.GetBookingsByGuestId(guest.GuestId).Count();

                    table.AddRow(
                        guest.GuestId.ToString(),
                        $"{guest.FirstName ?? "[gray]Unknown[/]"} {guest.LastName ?? "[gray]Unknown[/]"}",
                        guest.Email ?? "[gray]N/A[/]",
                        guest.PhoneNumber ?? "[gray]N/A[/]",
                        bookingCount.ToString()
                    );
                }

                AnsiConsole.Write(table);

                Console.WriteLine("\n[B] Back");
                var userInput = Console.ReadKey(true).Key;

                if (userInput == ConsoleKey.B) 
                    keepRunning = false;

            }
        }


    }
}
