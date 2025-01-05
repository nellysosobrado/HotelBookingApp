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
                new TextPrompt<string>("[white]Please enter your first name:[/]")
                    .ValidationErrorMessage("[red]First name cannot be empty[/]")
                    .Validate(input => !string.IsNullOrWhiteSpace(input)));

            string lastName = AnsiConsole.Prompt(
                new TextPrompt<string>("[white]Please enter your last name:[/]")
                    .ValidationErrorMessage("[red]Last name cannot be empty[/]")
                    .Validate(input => !string.IsNullOrWhiteSpace(input)));

            string email = AnsiConsole.Prompt(
                new TextPrompt<string>("[white]Please enter your email address (must include @):[/]")
                    .ValidationErrorMessage("[red]Invalid email[/]")
                    .Validate(input => input.Contains("@")));

            string phone = AnsiConsole.Prompt(
                new TextPrompt<string>("[white]Please enter your phone number:[/]")
                    .ValidationErrorMessage("[red]Invalid phone number![/]")
                    .Validate(input => long.TryParse(input, out _)));

            return new Guest
            {
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                PhoneNumber = phone
            };
        }

    }
}
