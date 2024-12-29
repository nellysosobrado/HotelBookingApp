using HotelBookingApp.Repositories;
using Spectre.Console;
using System;
using System.Linq;

namespace HotelBookingApp.Services
{
    public class UnbookBooking
    {
        private readonly BookingRepository _bookingRepository;

        public UnbookBooking(BookingRepository bookingRepository)
        {
            _bookingRepository = bookingRepository;
        }

        public void CancelBooking()
        {
            while (true)
            {
                Console.Clear();

                // Hämta bokningar som inte är avbokade, incheckade eller utcheckade
                var notCheckedInBookings = _bookingRepository.GetAllBookings()
                    .Where(b => !b.IsCanceled && !b.IsCheckedIn && !b.IsCheckedOut) // Endast bokningar som inte är incheckade eller utcheckade
                    .ToList();

                if (!notCheckedInBookings.Any())
                {
                    AnsiConsole.Markup("[red]No bookings available to cancel.[/]\n");
                    Console.WriteLine("\nPress any key to return...");
                    Console.ReadKey();
                    return;
                }

                // Skapa valalternativ för bokningar
                var bookingChoices = notCheckedInBookings
                    .Select(b => $"{b.BookingId}: {b.Guest.FirstName} {b.Guest.LastName} (Room ID: {b.RoomId}, Check-In: {b.CheckInDate:yyyy-MM-dd})")
                    .ToList();
                bookingChoices.Add("Back");

                // Låt användaren välja en bokning
                var selectedBooking = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("[bold yellow]Select a booking to cancel:[/]")
                        .AddChoices(bookingChoices)
                        .HighlightStyle(new Style(foreground: Color.Green))
                );

                // Om användaren väljer "Back"
                if (selectedBooking == "Back")
                    break;

                // Extrahera Booking ID från det valda alternativet
                if (!int.TryParse(selectedBooking.Split(':')[0], out int bookingId))
                {
                    AnsiConsole.MarkupLine("[red]Invalid booking selection. Please try again.[/]");
                    Console.ReadKey();
                    continue;
                }

                var booking = notCheckedInBookings.FirstOrDefault(b => b.BookingId == bookingId);
                if (booking == null)
                {
                    AnsiConsole.MarkupLine("[red]Booking not found. Please try again.[/]");
                    Console.ReadKey();
                    continue;
                }

                // Bekräfta avbokningen
                var confirm = AnsiConsole.Confirm($"Are you sure you want to cancel booking [yellow]{bookingId}[/] for [green]{booking.Guest.FirstName} {booking.Guest.LastName}[/]?");
                if (!confirm)
                    continue;

                // Markera bokningen som avbokad
                booking.IsCanceled = true;
                _bookingRepository.UpdateBooking(booking);

                AnsiConsole.MarkupLine($"[green]Booking {bookingId} has been successfully canceled.[/]");

                Console.WriteLine("\nPress any key to return...");
                Console.ReadKey();
                break;
            }
        }
    }
}
