using HotelBookingApp.Data;
using HotelBookingApp.Entities;
using Spectre.Console;
using System;
using System.Linq;

namespace HotelBookingApp
{
    public static class ExistingGuests
    {
        public static void AssignRoomsToExistingGuests(AppDbContext context)
        {
            var guests = context.Guests.ToList();
            if (!guests.Any())
            {
                AnsiConsole.MarkupLine("[red]No guests found in the database.[/]");
                return;
            }

            var table = new Table()
                .Border(TableBorder.Rounded)
                .AddColumn("[bold blue]Guest[/]")
                .AddColumn("[bold blue]Room[/]")
                .AddColumn("[bold blue]Status[/]");

            // Progressbar setup
            AnsiConsole.Progress()
                .AutoRefresh(true)
                .AutoClear(false)
                .Start(ctx =>
                {
                    var task = ctx.AddTask("[green]Assigning rooms to guests...[/]", maxValue: guests.Count);

                    foreach (var guest in guests)
                    {
                        // Simulera tid för att ge en bättre visuell effekt
                        System.Threading.Thread.Sleep(500);

                        // Kontrollera om gästen redan har en bokning
                        var existingBooking = context.Bookings.FirstOrDefault(b => b.GuestId == guest.GuestId);
                        if (existingBooking != null)
                        {
                            table.AddRow(
                                $"{guest.FirstName} {guest.LastName}",
                                "-",
                                "[yellow]Already has a booking[/]"
                            );
                            task.Increment(1);
                            continue;
                        }

                        // Hitta ett tillgängligt rum
                        var availableRoom = context.Rooms
                            .FirstOrDefault(r => r.IsAvailable && !context.Bookings
                                .Any(b => b.RoomId == r.RoomId &&
                                          b.CheckInDate < DateTime.Now.AddDays(3) &&
                                          b.CheckOutDate > DateTime.Now));

                        if (availableRoom == null)
                        {
                            table.AddRow(
                                $"{guest.FirstName} {guest.LastName}",
                                "-",
                                "[red]No available rooms[/]"
                            );
                            task.Increment(1);
                            continue;
                        }

                        // Skapa en ny bokning
                        var newBooking = new Booking
                        {
                            GuestId = guest.GuestId,
                            RoomId = availableRoom.RoomId,
                            CheckInDate = DateTime.Now.AddDays(1),
                            CheckOutDate = DateTime.Now.AddDays(4),
                            IsCheckedIn = false,
                            IsCheckedOut = false,
                            BookingStatus = false
                        };

                        availableRoom.IsAvailable = false;

                        context.Bookings.Add(newBooking);
                        context.SaveChanges();

                        table.AddRow(
                            $"{guest.FirstName} {guest.LastName}",
                            $"{availableRoom.RoomId}",
                            "[green]Booking created[/]"
                        );

                        task.Increment(1);
                    }
                });

            // Visa resultat i en tabell
            AnsiConsole.Clear();
            AnsiConsole.MarkupLine("[bold green]All bookings for existing guests have been processed.[/]");
            AnsiConsole.Write(table);

            // Simulera en kort fördröjning innan det automatiskt går vidare
            AnsiConsole.MarkupLine("[yellow]Returning to the main menu in 5 seconds...[/]");
            System.Threading.Thread.Sleep(5000); // Väntar i 5 sekunder innan avslut
        }
    }
}
