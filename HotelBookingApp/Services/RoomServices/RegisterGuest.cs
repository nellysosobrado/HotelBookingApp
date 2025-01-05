using HotelBookingApp.Repositories;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBookingApp.Services.RoomServices
{
    public class RegisterGuest
    {
        private readonly GuestRepository _guestRepository;

        public RegisterGuest(GuestRepository guestRepository)
        {
            _guestRepository = guestRepository;
        }

        public void NewGuest()
        {
            Console.Clear();

            bool proceed = AnsiConsole.Confirm("[white]Do you want to register a new guest?[/]");
            if (!proceed)
            {
                AnsiConsole.MarkupLine("[red]Guest registration canceled. Returning to the main menu.[/]");
                return;
            }

            AnsiConsole.MarkupLine("[italic yellow]Register New Guest[/]\n");

            var guest = CollectGuestInformation();
            if (guest == null)
            {
                AnsiConsole.MarkupLine("[red]Registration has been canceled.[/]");
                return;
            }

            _guestRepository.AddGuest(guest);
            AnsiConsole.MarkupLine("[bold green]Guest has been successfully registered![/]");
            Console.ReadKey();
        }
        private Guest CollectGuestInformation()
        {
            string firstName = AnsiConsole.Prompt(
                new TextPrompt<string>("[white]Firstname:[/]")
                    .ValidationErrorMessage("[red]First name cannot be empty[/]")
                    .Validate(input => !string.IsNullOrWhiteSpace(input)));

            string lastName = AnsiConsole.Prompt(
                new TextPrompt<string>("[white]Lastname:[/]")
                    .ValidationErrorMessage("[red]Last name cannot be empty[/]")
                    .Validate(input => !string.IsNullOrWhiteSpace(input)));

            string email = AnsiConsole.Prompt(
                new TextPrompt<string>("[white]Email:[/]")
                    .ValidationErrorMessage("[red]Invalid email[/]")
                    .Validate(input => input.Contains("@")));

            string phone = AnsiConsole.Prompt(
                new TextPrompt<string>("[white]Phone number(must be Swedish, e.g., +46701234567 or 0701234567):[/]")
                    .ValidationErrorMessage("[red]Invalid phone number! Must be exactly 10 digits and start with +46 or 0.[/]")
                    .Validate(input => IsValidSwedishPhoneNumber(input)));

            return new Guest
            {
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                PhoneNumber = phone
            };
        }

        private bool IsValidSwedishPhoneNumber(string phoneNumber)
        {
            phoneNumber = phoneNumber.Replace(" ", "").Replace("-", "");

            if (phoneNumber.StartsWith("0"))
            {
                return phoneNumber.Length == 10 && long.TryParse(phoneNumber, out _);
            }
            else if (phoneNumber.StartsWith("+46"))
            {
                return phoneNumber.Length == 12 && long.TryParse(phoneNumber.Substring(1), out _); 
            }

            return false;
        }
    }
}
