﻿using HotelBookingApp.Repositories;
using Spectre.Console;
using System;
using System.Linq;

namespace HotelBookingApp.Services.BookingServices
{
    public class UnbookBooking
    {
        private readonly BookingRepository _bookingRepository;

        public UnbookBooking(BookingRepository bookingRepository)
        {
            _bookingRepository = bookingRepository;
        }
        public void UnbookBookings()
        {
            while (true)
            {
                Console.Clear();

                var bookings = _bookingRepository.GetAllBookings()
                    .Where(b => !_bookingRepository.GetCanceledBookingsHistory().Any(cb => cb.BookingId == b.BookingId))
                    .ToList();

                if (!bookings.Any())
                {
                    AnsiConsole.Markup("[red]No bookings available to cancel.[/]\n");
                    Console.WriteLine("\nPress any key to return...");
                    Console.ReadKey();
                    return;
                }

                var bookingChoices = bookings
                    .Select(b => $"{b.BookingId}: {b.Guest.FirstName} {b.Guest.LastName} (Room ID: {b.RoomId}, Check-In: {b.CheckInDate:yyyy-MM-dd})")
                    .ToList();
                bookingChoices.Add("Back");

                var selectedBooking = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("[bold yellow]Select a booking to cancel:[/]")
                        .AddChoices(bookingChoices)
                        .HighlightStyle(new Style(foreground: Color.Green))
                );

                if (selectedBooking == "Back")
                    break;

                if (!int.TryParse(selectedBooking.Split(':')[0], out int bookingId))
                {
                    AnsiConsole.MarkupLine("[red]Invalid booking selection. Please try again.[/]");
                    Console.ReadKey();
                    continue;
                }

                var booking = bookings.FirstOrDefault(b => b.BookingId == bookingId);
                if (booking == null)
                {
                    AnsiConsole.MarkupLine("[red]Booking not found. Please try again.[/]");
                    Console.ReadKey();
                    continue;
                }

                var confirm = AnsiConsole.Confirm($"Are you sure you want to cancel booking [yellow]{bookingId}[/] for [green]{booking.Guest.FirstName} {booking.Guest.LastName}[/]?");
                if (!confirm)
                    continue;

                _bookingRepository.CancelBooking(booking, "Unbooked booking by guest");

                AnsiConsole.MarkupLine("[green]Booking has been canceled successfully.[/]");
                AnsiConsole.MarkupLine("[bold yellow]Payment will be on their way to your bank account within 12 days.[/]");

                Console.WriteLine("\nPress any key to return...");
                Console.ReadKey();
                break;
            }
        }


    }

}
