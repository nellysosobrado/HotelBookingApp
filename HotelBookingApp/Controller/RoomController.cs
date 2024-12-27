using HotelBookingApp.Entities;
using HotelBookingApp.Repositories;
using Spectre.Console;
using System;
using System.Linq;

namespace HotelBookingApp.Controllers
{
    public class RoomController
    {
        private readonly RoomRepository _roomRepository;
        private readonly BookingRepository _bookingRepository;

        public RoomController(RoomRepository roomRepository, BookingRepository bookingRepository)
        {
            _roomRepository = roomRepository;
            _bookingRepository = bookingRepository;
        }

        public void AddNewRoom()
        {
            Console.Clear();
            AnsiConsole.Write(new Panel("[bold yellow]REGISTER NEW ROOM[/]").Expand());

            string roomType = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Select [green]Room Type[/]:")
                    .AddChoices("Single", "Double"));

            decimal price;
            while (true)
            {
                try
                {
                    price = AnsiConsole.Ask<decimal>("Enter [green]Price Per Night[/]:");
                    if (price <= 0)
                        throw new ArgumentException("[red]Price must be greater than 0.[/]");
                    break;
                }
                catch (Exception ex)
                {
                    AnsiConsole.MarkupLine($"[red]{ex.Message}[/]");
                }
            }

            int size;
            while (true)
            {
                try
                {
                    size = AnsiConsole.Ask<int>("Enter [green]Size in Square Meters[/]:");
                    if (size <= 0)
                        throw new ArgumentException("[red]Size must be greater than 0.[/]");
                    break;
                }
                catch (Exception ex)
                {
                    AnsiConsole.MarkupLine($"[red]{ex.Message}[/]");
                }
            }

            int extraBeds = 0;
            int maxPeople = roomType == "Double" ? 2 : 1;

            if (roomType == "Double")
            {
                while (true)
                {
                    try
                    {
                        extraBeds = AnsiConsole.Ask<int>("Enter [green]Number of Extra Beds[/] (0-2):");
                        if (extraBeds < 0 || extraBeds > 2)
                            throw new ArgumentException("[red]Extra beds must be between 0 and 2.[/]");
                        maxPeople += extraBeds;
                        break;
                    }
                    catch (Exception ex)
                    {
                        AnsiConsole.MarkupLine($"[red]{ex.Message}[/]");
                    }
                }
            }

            var newRoom = new Room
            {
                Type = roomType,
                PricePerNight = price,
                SizeInSquareMeters = size,
                ExtraBeds = extraBeds,
                IsAvailable = true,
                TotalPeople = maxPeople
            };

            var validator = new RoomValidator();
            var validationResult = validator.Validate(newRoom);

            if (!validationResult.IsValid)
            {
                AnsiConsole.Write(
                    new Panel("[bold red]Validation errors:[/]").Expand());
                foreach (var error in validationResult.Errors)
                {
                    AnsiConsole.MarkupLine($"[red]- {error.ErrorMessage}[/]");
                }
                return;
            }

            _roomRepository.AddRoom(newRoom);

            var table = new Table()
                .Border(TableBorder.Rounded)
                .AddColumn("[green]Property[/]")
                .AddColumn("[cyan]Value[/]")
                .AddRow("Room Type", roomType)
                .AddRow("Price Per Night", price.ToString("C"))
                .AddRow("Size in Square Meters", size.ToString())
                .AddRow("Extra Beds", extraBeds.ToString())
                .AddRow("Max People Allowed", maxPeople.ToString());

            AnsiConsole.Write(new Panel(table).Header("[green]Room Added Successfully![/]"));

            Console.WriteLine("\nPress any key to return to the menu...");
            Console.ReadKey();
        }
        public void EditRoom()
        {
            Console.Clear();
            AnsiConsole.Markup("[bold yellow]=== EDIT ROOM ===[/]\n");

            Console.Write("Enter Room ID: ");
            if (!int.TryParse(Console.ReadLine(), out var roomId))
            {
                AnsiConsole.Markup("[red]Invalid Room ID.[/]\n");
                return;
            }

            var room = _roomRepository.GetRoomById(roomId);
            if (room == null)
            {
                AnsiConsole.Markup("[red]Room not found.[/]\n");
                return;
            }

            AnsiConsole.Markup("[bold]Leave blank to keep current value.[/]\n");

            Console.Write($"Current Type: {room.Type}\nEnter new Type: ");
            var newType = Console.ReadLine()?.Trim();
            if (!string.IsNullOrWhiteSpace(newType))
                room.Type = newType;

            Console.Write($"Current Price: {room.PricePerNight}\nEnter new Price: ");
            if (decimal.TryParse(Console.ReadLine(), out var price))
                room.PricePerNight = price;

            Console.Write($"Current Size: {room.SizeInSquareMeters}\nEnter new Size: ");
            if (int.TryParse(Console.ReadLine(), out var size))
                room.SizeInSquareMeters = size;

            _roomRepository.UpdateRoom(room);
        }

        public void ViewAllRooms()
        {
            Console.Clear();
            AnsiConsole.MarkupLine("[bold yellow]View Rooms and Availability[/]\n");

            string roomType = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[yellow]Select room type to view:[/]")
                    .AddChoices("Single", "Double"));

            var rooms = _roomRepository.GetRoomsWithBookings()
                .Where(r => r.Type.Equals(roomType, StringComparison.OrdinalIgnoreCase))
                .ToList();

            if (!rooms.Any())
            {
                AnsiConsole.MarkupLine("[red]No rooms found for the selected type.[/]");
                return;
            }

            AnsiConsole.MarkupLine($"[bold green]{roomType} Rooms[/]\n");

            var table = new Table();
            table.Border(TableBorder.Rounded);
            table.AddColumn("[bold yellow]Room ID[/]");
            table.AddColumn("[bold yellow]Size (sqm)[/]");
            table.AddColumn("[bold yellow]Price Per Night[/]");
            table.AddColumn("[bold yellow]Booking Status[/]");
            table.AddColumn("[bold yellow]Start Date[/]");
            table.AddColumn("[bold yellow]End Date[/]");

            foreach (var room in rooms)
            {
                var activeBookings = room.Bookings.Where(b => !b.IsCheckedOut).ToList();

                if (activeBookings.Any())
                {
                    foreach (var booking in activeBookings)
                    {
                        string bookingStatus = booking.IsCheckedIn ? "[green]Checked In[/]" : "[blue]Not Checked In[/]";
                        string startDate = booking.CheckInDate.HasValue ? booking.CheckInDate.Value.ToString("yyyy-MM-dd") : "[grey]Not Set[/]";
                        string endDate = booking.CheckOutDate.HasValue ? booking.CheckOutDate.Value.ToString("yyyy-MM-dd") : "[grey]Not Set[/]";

                        table.AddRow(
                            room.RoomId.ToString(),
                            $"{room.SizeInSquareMeters} sqm",
                            $"{room.PricePerNight:C}",
                            bookingStatus,
                            startDate,
                            endDate
                        );
                    }
                }
                else
                {
                    table.AddRow(
                        room.RoomId.ToString(),
                        $"{room.SizeInSquareMeters} sqm",
                        $"{room.PricePerNight:C}",
                        "[green]Available[/]",
                        "[grey]N/A[/]",
                        "[grey]N/A[/]"
                    );
                }
            }

            AnsiConsole.Write(table);

            AnsiConsole.MarkupLine($"\n[bold green]Availability Calendar for {roomType} Rooms[/]\n");
            DateTime selectedDate = DateTime.Now.Date;
            while (true)
            {
                Console.Clear();
                AnsiConsole.MarkupLine($"[bold green]{roomType} Rooms[/]\n");
                AnsiConsole.Write(table);
                RenderCalendarForAvailability(selectedDate, roomType);

                var key = Console.ReadKey(true).Key;
                switch (key)
                {
                    case ConsoleKey.RightArrow:
                        selectedDate = selectedDate.AddMonths(1);
                        break;
                    case ConsoleKey.LeftArrow:
                        selectedDate = selectedDate.AddMonths(-1);
                        break;
                    case ConsoleKey.Escape:
                        return;
                    case ConsoleKey.Enter:
                        Console.Clear();
                        return;
                }
            }
        }

        private void RenderCalendarForAvailability(DateTime selectedDate, string roomType)
        {
            var calendarContent = new StringWriter();
            calendarContent.WriteLine($"[bold yellow]{selectedDate:MMMM yyyy}[/]".ToUpper());
            calendarContent.WriteLine("Mon  Tue  Wed  Thu  Fri  Sat  Sun");
            calendarContent.WriteLine("─────────────────────────────────");

            DateTime firstDayOfMonth = new DateTime(selectedDate.Year, selectedDate.Month, 1);
            int daysInMonth = DateTime.DaysInMonth(selectedDate.Year, selectedDate.Month);
            int startDay = (int)firstDayOfMonth.DayOfWeek;
            startDay = (startDay == 0) ? 6 : startDay - 1;

            var bookedDates = _bookingRepository.GetAllBookings()
                .Where(b => b.Room.Type.Equals(roomType, StringComparison.OrdinalIgnoreCase))
                .SelectMany(b => Enumerable.Range(0, 1 + (b.CheckOutDate.Value - b.CheckInDate.Value).Days)
                                            .Select(offset => b.CheckInDate.Value.AddDays(offset)))
                .ToHashSet();

            for (int i = 0; i < startDay; i++)
            {
                calendarContent.Write("     ");
            }

            for (int day = 1; day <= daysInMonth; day++)
            {
                DateTime currentDate = new DateTime(selectedDate.Year, selectedDate.Month, day);

                if (bookedDates.Contains(currentDate))
                {
                    calendarContent.Write($"[red]{day,2}[/]   ");
                }
                else if (currentDate < DateTime.Now.Date)
                {
                    calendarContent.Write($"[grey]{day,2}[/]   ");
                }
                else
                {
                    calendarContent.Write($"[green]{day,2}[/]   ");
                }

                if ((startDay + day) % 7 == 0)
                {
                    calendarContent.WriteLine();
                }
            }

            var panel = new Panel(calendarContent.ToString())
            {
                Border = BoxBorder.Double,
                Header = new PanelHeader($"[yellow]{selectedDate:yyyy}[/]", Justify.Center)
            };

            AnsiConsole.Write(panel);
        }


        public void DeleteRoom() 
        {
            Console.Clear();
            AnsiConsole.Markup("[bold yellow]=== DELETE ROOM ===[/]\n");

            Console.Write("Enter Room ID to delete: ");
            if (!int.TryParse(Console.ReadLine(), out var roomId))
            {
                AnsiConsole.Markup("[red]Invalid Room ID.[/]\n");
                return;
            }

            var room = _roomRepository.GetRoomById(roomId);
            if (room == null)
            {
                AnsiConsole.Markup("[red]Room not found.[/]\n");
                return;
            }

            Console.WriteLine($"Are you sure you want to delete Room ID {room.RoomId} (Type: {room.Type})? (Y/N)");
            var confirmation = Console.ReadLine()?.Trim().ToUpper();

            if (confirmation == "Y")
            {
                _roomRepository.DeleteRoom(roomId);
                AnsiConsole.Markup($"[green]Room ID {roomId} has been successfully deleted.[/]\n");
            }
            else
            {
                AnsiConsole.Markup("[yellow]Operation cancelled.[/]\n");
            }
        }

    }
}
