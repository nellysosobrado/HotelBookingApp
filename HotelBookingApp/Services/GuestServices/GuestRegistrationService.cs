using HotelBookingApp.Entities;
using HotelBookingApp.Repositories;
using HotelBookingApp.Services.DisplayServices;
using Spectre.Console;
using System;

namespace HotelBookingApp.Services.GuestServices
{
    public class GuestRegistrationService
    {
        private readonly GuestRepository _guestRepository;

        public GuestRegistrationService(GuestRepository guestRepository)
        {
            _guestRepository = guestRepository;

        }

        public void RegisterNewGuest()
        {
            Console.Clear();

            bool proceed = AnsiConsole.Confirm("[bold yellow]Do you want to register a new guest?[/]");
            if (!proceed)
            {
                AnsiConsole.MarkupLine("[red]Guest registration canceled. Returning to the main menu.[/]");
                return;
            }

            AnsiConsole.MarkupLine("[bold green]Register New Guest[/]\n");

            var guest = CollectGuestInformation();
            if (guest == null)
            {
                AnsiConsole.MarkupLine("[red]Registration has been canceled.[/]");
                return;
            }

            bool createBooking = AnsiConsole.Confirm("Would you like to continue to create a booking for this guest or cancel registration?");
            if (createBooking)
            {
                CreateBookingForGuest(guest);
            }
            else
            {
                _guestRepository.AddGuest(guest);
                AnsiConsole.MarkupLine("[bold green]Guest has been successfully registered![/]");
            }

            Console.ReadKey();
        }

        private Guest CollectGuestInformation()
        {
            try
            {
                string firstName = AnsiConsole.Ask<string>("Enter first name:");
                string lastName = AnsiConsole.Ask<string>("Enter last name:");
                string email = AnsiConsole.Ask<string>("Enter email address:");
                string phoneNumber = AnsiConsole.Ask<string>("Enter phone number:");

                return new Guest
                {
                    FirstName = firstName,
                    LastName = lastName,
                    Email = email,
                    PhoneNumber = phoneNumber
                };
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]Error collecting guest information: {ex.Message}[/]");
                return null;
            }
        }

        private void CreateBookingForGuest(Guest guest)
        {
            Booking booking = null;

            while (booking == null)
            {
                booking = CollectBookingDetails(guest);
                if (booking == null)
                {
                    bool tryAgain = AnsiConsole.Confirm("No rooms available for the selected dates and room type. Would you like to try again?");
                    if (!tryAgain)
                    {
                        AnsiConsole.MarkupLine("[red]Booking has been canceled.[/]");
                        return;
                    }
                }
            }

            decimal totalAmount = _guestRepository.CalculateTotalAmount(booking);

            var invoice = new Invoice
            {
                BookingId = booking.BookingId,
                TotalAmount = totalAmount,
                IsPaid = false,
                PaymentDeadline = booking.CheckOutDate?.AddDays(7) ?? DateTime.Now.AddDays(7)
            };

            _guestRepository.RegisterNewGuestWithBookingAndInvoice(guest, booking, invoice);

            AnsiConsole.MarkupLine("[bold green]Guest has been successfully registered and booked![/]");
            AnsiConsole.MarkupLine($"[yellow]Invoice created:[/] Total Amount: {totalAmount:C}, Payment Deadline: {invoice.PaymentDeadline:yyyy-MM-dd}");
        }

        private Booking CollectBookingDetails(Guest guest)
        {
            AnsiConsole.MarkupLine("[bold yellow]Please enter booking details:[/]");

            var checkInDate = AnsiConsole.Prompt(
                new TextPrompt<DateTime>("Enter Check-In Date (yyyy-MM-dd):")
                    .Validate(date => date > DateTime.Now ? ValidationResult.Success() : ValidationResult.Error("[red]Date must be in the future.[/]"))
            );

            var checkOutDate = AnsiConsole.Prompt(
                new TextPrompt<DateTime>("Enter Check-Out Date (yyyy-MM-dd):")
                    .Validate(date => date > checkInDate ? ValidationResult.Success() : ValidationResult.Error("[red]Check-Out Date must be after Check-In Date.[/]"))
            );

            var roomType = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Select Room Type:")
                    .AddChoices("Single", "Double", "Suite")
            );

            var availableRooms = _guestRepository.GetAvailableRooms(checkInDate, checkOutDate, roomType);
            if (!availableRooms.Any())
            {
                AnsiConsole.MarkupLine("[red]No available rooms found for the selected dates and room type.[/]");
                return null;
            }

            var selectedRoomId = AnsiConsole.Prompt(
                new SelectionPrompt<int>()
                    .Title("Select a room:")
                    .AddChoices(availableRooms.Select(r => r.RoomId))
            );

            var booking = new Booking
            {
                Guest = guest,
                RoomId = selectedRoomId,
                CheckInDate = checkInDate,
                CheckOutDate = checkOutDate,
                BookingCompleted = false 
            };

            return booking;
        }

    }
}
