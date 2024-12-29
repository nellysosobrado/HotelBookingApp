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
        private readonly TableDisplayService _tableDisplayService;

        public GuestRemovalService(
            GuestRepository guestRepository,
            BookingRepository bookingRepository,
            TableDisplayService tableDisplayService)
        {
            _guestRepository = guestRepository;
            _bookingRepository = bookingRepository;
            _tableDisplayService = tableDisplayService;
        }

        public void Execute()
        {
            while (true)
            {
                Console.Clear();
                AnsiConsole.MarkupLine("[bold yellow]Remove a Guest[/]");

                var allGuests = _guestRepository.GetAllGuests().Where(g => !g.IsDeleted).ToList();
                if (!allGuests.Any())
                {
                    AnsiConsole.MarkupLine("[red]No registered guests found.[/]");
                    Console.ReadKey();
                    return;
                }

                _tableDisplayService.DisplayGuestTable(allGuests);

                var guestOptions = allGuests.Select(g => g.GuestId.ToString()).ToList();
                guestOptions.Add("Go Back");

                var selectedOption = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("[yellow]Select a Guest ID to remove or choose Go Back:[/]")
                        .AddChoices(guestOptions)
                        .HighlightStyle(new Style(foreground: Color.Green))
                );

                if (selectedOption == "Go Back")
                {
                    return; 
                }

                int guestId = int.Parse(selectedOption);
                RemoveGuestById(guestId);
            }
        }

        private void RemoveGuestById(int guestId)
        {
            var selectedGuest = _guestRepository.GetGuestById(guestId);

            if (selectedGuest == null)
            {
                AnsiConsole.MarkupLine("[red]Guest not found.[/]");
                Console.ReadKey();
                return;
            }

            var guestBookings = _bookingRepository.GetBookingsByGuestId(guestId);
            if (guestBookings.Any(b => !b.IsCanceled && !b.IsCheckedOut))
            {
                AnsiConsole.MarkupLine("[red]This guest has active bookings and cannot be removed.[/]");
                Console.ReadKey();
                return;
            }

            selectedGuest.IsDeleted = true;
            _guestRepository.UpdateGuest(selectedGuest);

            AnsiConsole.MarkupLine($"[green]Guest {selectedGuest.FirstName} (ID: {selectedGuest.GuestId}) has been successfully removed.[/]");
            Console.ReadKey();
        }
    }
}
