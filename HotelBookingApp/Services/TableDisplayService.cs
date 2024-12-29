using HotelBookingApp.Entities;
using Spectre.Console;
using System.Collections.Generic;
using System.Linq;

namespace HotelBookingApp.Services.DisplayServices
{
    public class TableDisplayService
    {
        public void DisplayRooms(IEnumerable<Room> rooms, string title, bool includeDeleted)
        {
            if (rooms == null || !rooms.Any())
            {
                AnsiConsole.Markup($"[red]No rooms found for {title}[/].\n");
                return;
            }

            var table = new Table()
                .Border(TableBorder.Rounded)
                .AddColumn("[bold]Room ID[/]")
                .AddColumn("[bold]Type[/]")
                .AddColumn("[bold]Price[/]")
                .AddColumn("[bold]Size (sqm)[/]");

            if (!includeDeleted)
            {
                table.AddColumn("[bold]Max People[/]");
            }

            foreach (var room in rooms)
            {
                if (includeDeleted)
                {
                    table.AddRow(
                        room.RoomId.ToString(),
                        room.Type,
                        room.PricePerNight.ToString("C"),
                        room.SizeInSquareMeters.ToString()
                    );
                }
                else
                {
                    table.AddRow(
                        room.RoomId.ToString(),
                        room.Type,
                        room.PricePerNight.ToString("C"),
                        room.SizeInSquareMeters.ToString(),
                        room.TotalPeople.ToString("F1")
                    );
                }
            }

            AnsiConsole.Markup($"[bold green]{title}[/]\n");
            AnsiConsole.Write(table);
        }

        public void DisplayBookedRooms(IEnumerable<Room> bookedRooms, string title)
        {
            if (bookedRooms == null || !bookedRooms.Any())
            {
                AnsiConsole.Markup($"[red]No {title} found.[/]\n");
                return;
            }

            var table = new Table()
                .Border(TableBorder.Rounded)
                .AddColumn("[bold]Room ID[/]")
                .AddColumn("[bold]Booked By[/]")
                .AddColumn("[bold]Start Date[/]")
                .AddColumn("[bold]End Date[/]");

            foreach (var room in bookedRooms)
            {
                foreach (var booking in room.Bookings.Where(b => !b.IsCanceled))
                {
                    table.AddRow(
                        room.RoomId.ToString(),
                        $"{booking.Guest?.FirstName ?? "Unknown"} {booking.Guest?.LastName ?? "Unknown"}",
                        booking.CheckInDate?.ToString("yyyy-MM-dd") ?? "N/A",
                        booking.CheckOutDate?.ToString("yyyy-MM-dd") ?? "N/A"
                    );
                }
            }

            AnsiConsole.Markup($"[bold green]{title}[/]\n");
            AnsiConsole.Write(table);
        }
    }
}
