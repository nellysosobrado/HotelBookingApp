using HotelBookingApp.Repositories;
using HotelBookingApp.Services.DisplayServices;
using Spectre.Console;
using System.Linq;

namespace HotelBookingApp.Services
{
    public class GuestRemovalService
    {
        private readonly GuestRepository _guestRepository;
        private readonly BookingRepository _bookingRepository;

        public GuestRemovalService(GuestRepository guestRepository, BookingRepository bookingRepository)
        {
            _guestRepository = guestRepository;
            _bookingRepository = bookingRepository;
        }

        public void Execute()
        {
            Console.Clear();
            Console.WriteLine("REMOVE GUEST");
            Console.WriteLine(new string('-', 60));

            var guests = _guestRepository.GetAllGuests();

            if (!guests.Any())
            {
                AnsiConsole.MarkupLine("[red]No registered guests found to remove.[/]");
                Console.ReadKey();
                return;
            }

            var guestId = AnsiConsole.Prompt(
                new SelectionPrompt<int>()
                    .Title("[green]Select a guest to remove (by Guest ID):[/]")
                    .AddChoices(guests.Select(g => g.GuestId))
            );

            var guest = guests.FirstOrDefault(g => g.GuestId == guestId);

            if (guest == null)
            {
                AnsiConsole.MarkupLine("[red]Guest not found.[/]");
                Console.ReadKey();
                return;
            }

            // Kontrollera om gästen har aktiva bokningar
            var activeBookings = _bookingRepository.GetBookingsByGuestId(guestId)
                .Where(b => !b.IsCanceled && !b.IsCheckedOut);

            if (activeBookings.Any())
            {
                AnsiConsole.MarkupLine("[red]Cannot remove guest with active bookings.[/]");
                Console.ReadKey();
                return;
            }

            // Ta bort gästen
            _guestRepository.RemoveGuest(guest);

            AnsiConsole.MarkupLine($"[green]Guest {guest.FirstName} {guest.LastName} has been removed successfully.[/]");
            Console.ReadKey();
        }
    }

}

