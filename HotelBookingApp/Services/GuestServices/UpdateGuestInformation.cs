using HotelBookingApp.Repositories;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBookingApp.Services.GuestServices
{
    public class UpdateGuestInformation
    {
        private readonly GuestRepository _guestRepository;

        public UpdateGuestInformation(GuestRepository guestRepository)
        {
            _guestRepository = guestRepository;
        }

        public void Run()
        {
            Console.Clear();
            AnsiConsole.MarkupLine("[bold cyan]UPDATE GUEST DETAILS[/]");

            var guests = _guestRepository.GetAllGuests();

            if (!guests.Any())
            {
                AnsiConsole.MarkupLine("[red]No guests found.[/]");
                Console.ReadKey(true);
                return;
            }

            var table = new Table();
            table.AddColumn("[bold yellow]ID[/]");
            table.AddColumn("[bold yellow]First Name[/]");
            table.AddColumn("[bold yellow]Last Name[/]");
            table.AddColumn("[bold yellow]Email[/]");
            table.AddColumn("[bold yellow]Phone Number[/]");

            void RefreshTable()
            {
                table.Rows.Clear();
                foreach (var guest in guests)
                {
                    table.AddRow(
                        guest.GuestId.ToString(),
                        guest.FirstName,
                        guest.LastName,
                        guest.Email,
                        guest.PhoneNumber
                    );
                }
                Console.Clear();
                AnsiConsole.Write(table);
            }

            RefreshTable();

            int guestId;
            while (true)
            {
                Console.Clear();
                AnsiConsole.Write(table);
                AnsiConsole.MarkupLine("[bold grey] To go back enter 'back'[/]");
                var input = AnsiConsole.Ask<string>("[bold green]\nEnter the Guest ID you want to update:[/]");
                if (input.Trim().ToLower() == "back")
                {
                    AnsiConsole.MarkupLine("[yellow]Returning to menu...[/]");
                    return;
                }

                if (!int.TryParse(input, out guestId))
                {
                    AnsiConsole.MarkupLine("[red]Enter a valid ID. Press any key to try again[/]");
                    continue;
                }

                var selectedGuest = _guestRepository.GetGuestById(guestId);

                if (selectedGuest == null)
                {
                    AnsiConsole.MarkupLine("[red]Guest not found. Press any key to try again.[/]");
                    Console.ReadKey();
                    continue;
                }

                while (true)
                {
                    var columnToUpdate = AnsiConsole.Prompt(
                        new SelectionPrompt<string>()
                            .HighlightStyle(new Style(foreground: Color.Green))
                            .Title("[bold green]Choose a option:[/]")
                            .AddChoices("First Name", "Last Name", "Email", "Phone Number", "Back")
                    );

                    if (columnToUpdate.ToLower() == "back")
                    {
                        AnsiConsole.MarkupLine("[yellow]Returning to menu...[/]");
                        return;
                    }

                    switch (columnToUpdate)
                    {
                        case "First Name":
                            selectedGuest.FirstName = AnsiConsole.Ask<string>(
                                $"[bold green]Current First Name:[/] {selectedGuest.FirstName}\nEnter new First Name ");
                            break;
                        case "Last Name":
                            selectedGuest.LastName = AnsiConsole.Ask<string>(
                                $"[bold green]Current Last Name:[/] {selectedGuest.LastName}\nEnter new Last Name ");
                            break;
                        case "Email":
                            while (true)
                            {
                                var email = AnsiConsole.Ask<string>(
                                    $"[bold green]Current Email:[/] {selectedGuest.Email}\nEnter new Email ");
                                if (string.IsNullOrWhiteSpace(email) || email.Contains("@"))
                                {
                                    selectedGuest.Email = email;
                                    break;
                                }
                                AnsiConsole.MarkupLine("[red]Invalid email format. Please enter a valid email address.[/]");
                            }
                            break;
                        case "Phone Number":
                            while (true)
                            {
                                var phone = AnsiConsole.Ask<string>(
                                    $"[bold green]Current Phone Number:[/] {selectedGuest.PhoneNumber}\nEnter new Phone Number ");
                                if (string.IsNullOrWhiteSpace(phone) || phone.All(char.IsDigit))
                                {
                                    selectedGuest.PhoneNumber = phone;
                                    break;
                                }
                                AnsiConsole.MarkupLine("[red]Invalid phone number. Please enter only numeric values.[/]");
                            }
                            break;
                    }

                    _guestRepository.UpdateGuest(selectedGuest);
                    AnsiConsole.MarkupLine("[bold green]\nGuest information has been updated! Press any key to continue[/]");
                    Console.ReadKey();
                    RefreshTable();
                }
            }
        }
    }
}
