using HotelBookingApp.Entities;
using HotelBookingApp.Repositories;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBookingApp.Services.RoomServices
{
    public class FindRoomByDate
    {
        private readonly RoomRepository _roomRepository;
        private readonly BookingRepository _bookingRepository;

        public FindRoomByDate(RoomRepository roomRepository, BookingRepository bookingRepository)
        {
            _roomRepository = roomRepository;
            _bookingRepository = bookingRepository;
        }

        public void Execute()
        {
            Console.Clear();

            DisplayAllRooms();

            var startDate = GetDate("Enter [green]Start Date (yyyy-MM-dd)[/]:");
            var endDate = GetDate("Enter [green]End Date (yyyy-MM-dd)[/]:");

            if (!ValidateDateRange(startDate, endDate)) return;

            var availableRooms = GetAvailableRooms(startDate, endDate);

            DisplayAvailableRooms(availableRooms, startDate, endDate);
        }

        private DateTime GetDate(string prompt)
        {
            return AnsiConsole.Ask<DateTime>(prompt);
        }

        private bool ValidateDateRange(DateTime startDate, DateTime endDate)
        {
            var dateRangeValidator = new DateRangeValidator();
            var dateValidationResult = dateRangeValidator.Validate((startDate, endDate));

            if (dateValidationResult.IsValid) return true;

            AnsiConsole.Markup("[red]Validation Errors:[/]\n");
            foreach (var error in dateValidationResult.Errors)
            {
                AnsiConsole.Markup($"[red]- {error.ErrorMessage}[/]\n");
            }
            Console.ReadKey();
            return false;
        }

        private List<Room> GetAvailableRooms(DateTime startDate, DateTime endDate)
        {
            var rooms = _roomRepository.GetRoomsWithBookings();

            var canceledBookingIds = _bookingRepository.GetCanceledBookingsHistory()
                .Select(cb => cb.BookingId)
                .ToHashSet(); 

            return rooms
                .Where(room =>
                    !room.IsDeleted && 
                    !room.Bookings.Any(booking =>
                        !canceledBookingIds.Contains(booking.BookingId) && 
                        booking.CheckInDate.HasValue &&
                        booking.CheckOutDate.HasValue &&
                        !(booking.CheckOutDate.Value < startDate || booking.CheckInDate.Value > endDate))) 
                .ToList();
        }

        private void DisplayAvailableRooms(List<Room> availableRooms, DateTime startDate, DateTime endDate)
        {
            if (!availableRooms.Any())
            {
                AnsiConsole.Markup("[red]No rooms available for the selected dates.[/]\n");
            }
            else
            {
                var table = new Table()
                    .Border(TableBorder.Rounded)
                    .AddColumn("[bold]Room ID[/]")
                    .AddColumn("[bold]Type[/]")
                    .AddColumn("[bold]Price[/]")
                    .AddColumn("[bold]Size (sqm)[/]")
                    .AddColumn("[bold]Total People (sqm)[/]");

                foreach (var room in availableRooms)
                {
                    table.AddRow(
                        room.RoomId.ToString(),
                        room.Type,
                        room.PricePerNight.ToString("C"),
                        room.SizeInSquareMeters.ToString(),
                        room.TotalPeople.ToString()
                    );
                }
                Console.Clear();
                AnsiConsole.Markup($"[bold green]Available Rooms from {startDate:yyyy-MM-dd} to {endDate:yyyy-MM-dd}:[/]\n");
                AnsiConsole.Write(table);
            }

            Console.WriteLine("\nPress any key to go back..");
            Console.ReadKey();
        }

        private void DisplayAllRooms()
        {
            var allRooms = _roomRepository.GetRoomsWithBookings();

            var table = new Table()
                .Border(TableBorder.Rounded)
                .AddColumn("[bold]Room ID[/]")
                .AddColumn("[bold]Type[/]")
                .AddColumn("[bold]Price[/]")
                .AddColumn("[bold]Size (sqm)[/]")
                .AddColumn("[bold]Total People (sqm)[/]");

            foreach (var room in allRooms)
            {
                table.AddRow(
                    room.RoomId.ToString(),
                    room.Type,
                    room.PricePerNight.ToString("C"),
                    room.SizeInSquareMeters.ToString(),
                    room.TotalPeople.ToString()
                );
            }

            AnsiConsole.Markup("[bold green]All Rooms:[/]\n");
            AnsiConsole.Write(table);
        }
    }

}
