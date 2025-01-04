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

        public void DisplayAllRegisteredGuests()
        {
            AnsiConsole.MarkupLine("[yellow]Currently Registered Guests:[/]");

            var guests = _guestRepository.GetAllGuests();

            if (!guests.Any())
            {
                AnsiConsole.MarkupLine("[gray]No registered guests found.[/]");
                Console.ReadKey();
                return;
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
            Console.WriteLine("Press any key to continue..");
            Console.ReadKey();
        }

    }
}
