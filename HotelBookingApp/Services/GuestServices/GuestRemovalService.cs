using HotelBookingApp.Repositories;
using HotelBookingApp.Services.DisplayServices;
using Spectre.Console;
using System.Linq;

namespace HotelBookingApp.Services.GuestServices
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

            var guests = _guestRepository.GetAllGuests()
                .Where(g => !g.IsDeleted)
                .ToList();

            if (!guests.Any())
            {
                AnsiConsole.MarkupLine("[red]No registered guests found to remove.[/]");
                Console.ReadKey();
                return;
            }

            var guestChoices = guests
                .Select(g => new { g.GuestId, Display = $"{g.GuestId}: {g.FirstName} {g.LastName}" })
                .ToList();

            var selectedGuestDisplay = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[green]Select a guest to remove (ID: Name):[/]")
                    .AddChoices(guestChoices.Select(g => g.Display))
            );

            var selectedGuest = guestChoices
                .FirstOrDefault(g => g.Display == selectedGuestDisplay);

            if (selectedGuest == null)
            {
                AnsiConsole.MarkupLine("[red]Guest not found.[/]");
                Console.ReadKey();
                return;
            }

            var guest = guests.FirstOrDefault(g => g.GuestId == selectedGuest.GuestId);

            var activeBookings = _bookingRepository.GetBookingsByGuestId(guest.GuestId)
                .Where(b => !b.IsCheckedOut && !_bookingRepository.GetCanceledBookingsHistory().Any(cb => cb.BookingId == b.BookingId))
                .ToList();

            if (activeBookings.Any())
            {
                AnsiConsole.MarkupLine("[red]Cannot remove guest with active bookings. Unbook the booking of the guest first! Then remove guest![/]");
                Console.ReadKey();
                return;
            }
            guest.IsDeleted = true;
            guest.DeletedDate = DateTime.Now;
            guest.RemovalReason = "Removed by receptionist"; 

            _guestRepository.UpdateGuest(guest);

            AnsiConsole.MarkupLine($"[green]Guest {guest.FirstName} {guest.LastName} has been removed successfully.[/]");
            Console.ReadKey();
        }


    }

}

