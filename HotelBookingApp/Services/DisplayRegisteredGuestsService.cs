using HotelBookingApp.Repositories;

using Microsoft.IdentityModel.Tokens;
using Spectre.Console;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBookingApp.Services
{
    public class DisplayRegisteredGuestsService
    {
        private readonly GuestRepository _guestRepository;

        public DisplayRegisteredGuestsService(GuestRepository guestRepository)
        {
            _guestRepository = guestRepository;
        }

        public void DisplayAllRegisteredGuests()
        {
            AnsiConsole.MarkupLine("[yellow]Currently Registered Guests:[/]");

            var guests = _guestRepository.GetAllGuests();

            if (!guests.Any())
            {
                AnsiConsole.MarkupLine("[gray]No registered guests found.[/]");
                return;
            }

            var table = new Table()
                .Border(TableBorder.Rounded)
                .AddColumn("[bold]Guest ID[/]")
                .AddColumn("[bold]Name[/]")
                .AddColumn("[bold]Email[/]")
                .AddColumn("[bold]Phone Number[/]");

            foreach (var guest in guests)
            {
                table.AddRow(
                    guest.GuestId.ToString(),
                    $"{guest.FirstName ?? "[gray]Unknown[/]"} {guest.LastName ?? "[gray]Unknown[/]"}",
                    guest.Email ?? "[gray]N/A[/]",
                    guest.PhoneNumber ?? "[gray]N/A[/]"
                );
            }

            AnsiConsole.Write(table);
        }
    }
}
