using HotelBookingApp.Repositories;
using System;
using Spectre.Console;
using HotelBookingApp.Entities;
using HotelBookingApp.Data;
using HotelBookingApp.Controllers;
using HotelBookingApp.Services;
using HotelBookingApp.Services.DisplayServices;
using HotelBookingApp.Services.BookingServices;

//CRUD
//CREATE - Register new booking
//READ - Display : Active bookings, completed bookings, removed/canceled bookings
//UPDATE - Edit booking
//DELETE - Mark booking

namespace HotelBookingApp
{
    public class BookingController
    {
        private readonly BookingRepository _bookingRepository;
        private readonly RoomRepository _roomRepository;
        private readonly GuestRepository _guestRepository;
        private readonly GuestController _guestController;
        private readonly TableDisplayService _tableDisplayService;
        private readonly CheckInOutService _checkInOutService;
        private readonly BookingEditService _bookingEditService;
        private readonly UnbookBooking _unbookBooking;
        private readonly PaymentService _paymentService;


        public BookingController(
            BookingRepository bookingRepository,
            RoomRepository roomRepository, 
            GuestRepository guestRepository, 
            GuestController guestController,
            TableDisplayService tableDisplayService,
            CheckInOutService checkInOutService,
            BookingEditService bookingEditService,
            UnbookBooking unbookBooking,
            PaymentService paymentService)
        {
            _bookingRepository = bookingRepository;
            _roomRepository = roomRepository;
            _guestRepository = guestRepository;
            _guestController = guestController;
            _tableDisplayService = tableDisplayService;
            _checkInOutService = checkInOutService;
            _bookingEditService = bookingEditService;
            _unbookBooking = unbookBooking;
            _paymentService = paymentService;
        }

       
        //private DateTime SelectDate(string prompt)
        //{
        //    DateTime currentDate = DateTime.Now.Date; 
        //    DateTime selectedDate = new DateTime(currentDate.Year, currentDate.Month, 1); 

        //    while (true)
        //    {
        //        Console.Clear();
        //        Console.WriteLine(prompt);
        //        RenderCalendar(selectedDate);

        //        var key = Console.ReadKey(true).Key;
        //        switch (key)
        //        {
        //            case ConsoleKey.RightArrow:
        //                selectedDate = selectedDate.AddDays(1);
        //                break;
        //            case ConsoleKey.LeftArrow:
        //                if (selectedDate.AddDays(-1) >= currentDate)
        //                    selectedDate = selectedDate.AddDays(-1);
        //                break;
        //            case ConsoleKey.UpArrow:
        //                if (selectedDate.AddDays(-7) >= currentDate)
        //                    selectedDate = selectedDate.AddDays(-7);
        //                break;
        //            case ConsoleKey.DownArrow:
        //                selectedDate = selectedDate.AddDays(7);
        //                break;
        //            case ConsoleKey.Enter:
        //                if (selectedDate >= currentDate)
        //                    return selectedDate;
        //                AnsiConsole.MarkupLine("[red]You cannot select a date in the past![/]");
        //                Console.ReadKey();
        //                break;
        //            case ConsoleKey.Escape:
        //                return DateTime.MinValue; 
        //        }
        //    }
        //}


        //private void RenderCalendar(DateTime selectedDate)
        //{
        //    var calendarContent = new StringWriter();

        //    calendarContent.WriteLine($"[red]{selectedDate:MMMM}[/]".ToUpper());
        //    calendarContent.WriteLine("Mon  Tue  Wed  Thu  Fri  Sat  Sun");
        //    calendarContent.WriteLine("─────────────────────────────────");

        //    DateTime firstDayOfMonth = new DateTime(selectedDate.Year, selectedDate.Month, 1);
        //    int daysInMonth = DateTime.DaysInMonth(selectedDate.Year, selectedDate.Month);
        //    int startDay = (int)firstDayOfMonth.DayOfWeek;
        //    startDay = (startDay == 0) ? 6 : startDay - 1; 

        //    for (int i = 0; i < startDay; i++)
        //    {
        //        calendarContent.Write("     ");
        //    }

        //    for (int day = 1; day <= daysInMonth; day++)
        //    {
        //        if (day == selectedDate.Day)
        //        {
        //            calendarContent.Write($"[green]{day,2}[/]   ");
        //        }
        //        else
        //        {
        //            calendarContent.Write($"{day,2}   ");
        //        }

        //        if ((startDay + day) % 7 == 0)
        //        {
        //            calendarContent.WriteLine();
        //        }
        //    }

        //    var panel = new Panel(calendarContent.ToString())
        //    {
        //        Border = BoxBorder.Double,
        //        Header = new PanelHeader($"[red]{selectedDate:yyyy}[/]", Justify.Center)
        //    };

        //    AnsiConsole.Write(panel);
        //    Console.WriteLine();
        //    AnsiConsole.MarkupLine("\nUse arrow keys [blue]\u25C4 \u25B2 \u25BA \u25BC[/] to navigate and [green]Enter[/] to select.");
        //}


        public void DisplayAllGuestInfo()
        {
            
            DisplayGuestOptions();
   
        }

        public void DisplayGuestOptions()
        {
            AnsiConsole.MarkupLine("[bold yellow]Guests[/]");
            var options = new List<string>
    {
        "Display Active Bookings",
        "Display Booking History",
        "Display All Registered Guests",
        "Display History of Removed Bookings",
        "Remove a Guest",
        "Go back"
    };

            int selectedIndex = 0;

            while (true)
            {
                Console.Clear();

                for (int i = 0; i < options.Count; i++)
                {
                    if (i == selectedIndex)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"> {options[i]}");
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.WriteLine($"  {options[i]}");
                    }
                }

                var keyInfo = Console.ReadKey(true);

                switch (keyInfo.Key)
                {
                    case ConsoleKey.UpArrow:
                        selectedIndex = (selectedIndex - 1 + options.Count) % options.Count;
                        break;
                    case ConsoleKey.DownArrow:
                        selectedIndex = (selectedIndex + 1) % options.Count;
                        break;
                    case ConsoleKey.Enter:
                        switch (selectedIndex)
                        {
                            case 0:
                                Console.Clear();
                                DisplayActiveBookings();
                                Console.WriteLine("Press any key to go back...");
                                Console.ReadKey(true);
                                break;
                            case 1:
                                DisplayPreviousGuestHistory();
                                break;
                            case 2:
                                DisplayAllRegisteredGuests();
                                break;
                            case 3:
                                _tableDisplayService.DisplayCanceledBookings();
                                break;
                            case 4:
                                RemoveGuest();
                                break;
                            case 5:
                                return;
                        }
                        break;
                }
            }
        }
        private void RemoveGuest()
        {
            Console.Clear();
            AnsiConsole.MarkupLine("[bold yellow]Remove a Guest[/]");

            // Visa alla gäster
            var allGuests = _guestRepository.GetAllGuests();
            if (!allGuests.Any())
            {
                AnsiConsole.MarkupLine("[red]No registered guests found.[/]");
                Console.ReadKey();
                return;
            }

            var guestTable = new Table()
                .Border(TableBorder.Rounded)
                .AddColumn("[blue]Guest ID[/]")
                .AddColumn("[blue]Name[/]")
                .AddColumn("[blue]Lastname[/]")
                .AddColumn("[blue]Email[/]")
                .AddColumn("[blue]Phone[/]")
                .AddColumn("[blue]Status[/]");

            foreach (var guest in allGuests)
            {
                guestTable.AddRow(
                    guest.GuestId.ToString(),
                    guest.FirstName,
                    guest.LastName,
                    guest.Email,
                    guest.PhoneNumber,
                    guest.IsDeleted ? "[red]Deleted[/]" : "[green]Active[/]"
                );
            }

            AnsiConsole.Write(guestTable);

            // Be användaren välja en gäst att ta bort
            int guestId = AnsiConsole.Prompt(
                new TextPrompt<int>("[yellow]Enter the Guest ID to remove:[/]")
                    .ValidationErrorMessage("[red]Invalid Guest ID![/]")
                    .Validate(input => allGuests.Any(g => g.GuestId == input)));

            var selectedGuest = allGuests.First(g => g.GuestId == guestId);

            // Kontrollera om gästen har aktiva bokningar
            var guestBookings = _bookingRepository.GetBookingsByGuestId(guestId);
            if (guestBookings.Any(b => !b.IsCanceled))
            {
                AnsiConsole.MarkupLine("[red]This guest has active bookings and cannot be removed.[/]");
                Console.ReadKey();
                return;
            }

            // Utför soft delete
            selectedGuest.IsDeleted = true;
            _guestRepository.UpdateGuest(selectedGuest);

            AnsiConsole.MarkupLine($"[green]Guest {selectedGuest.FirstName} (ID: {selectedGuest.GuestId}) has been successfully removed.[/]");
            Console.ReadKey();
        }




        public void DisplayActiveBookings()
        {
           
            var activeBookings = _bookingRepository.GetAllBookings()
                .Where(b => !b.IsCanceled && b.BookingStatus != true) 
                .ToList();

            if (!activeBookings.Any())
            {
                AnsiConsole.Markup("[red]No active or upcoming bookings found.[/]");
            }
            else
            {
                var table = new Table();
                table.Border(TableBorder.Rounded);

                table.AddColumn("[bold yellow]Guest[/]");
                table.AddColumn("[bold yellow]Guest ID[/]"); 
                table.AddColumn("[bold yellow]Booking ID[/]");
                table.AddColumn("[bold yellow]Room[/]");
                table.AddColumn("[bold yellow]Status[/]");
                table.AddColumn("[bold yellow]Check-In Date[/]");
                table.AddColumn("[bold yellow]Check-Out Date[/]");
                table.AddColumn("[bold yellow]Invoice Amount[/]");
                table.AddColumn("[bold yellow]Payment Status[/]");

                foreach (var booking in activeBookings)
                {
                    string status = booking.IsCheckedIn ? "[green]Checked In[/]" : "[blue]Not Checked In[/]";
                    var latestInvoice = booking.Invoices?.OrderByDescending(i => i.PaymentDeadline).FirstOrDefault();
                    string invoiceAmount = latestInvoice != null ? $"{latestInvoice.TotalAmount:C}" : "[grey]No Invoice[/]";
                    string paymentStatus = latestInvoice != null
                        ? (latestInvoice.IsPaid ? "[green]Paid[/]" : "[red]Not Paid[/]")
                        : "[grey]No Invoice[/]";

                    string checkInDate = booking.CheckInDate.HasValue
                        ? booking.CheckInDate.Value.ToString("yyyy-MM-dd")
                        : "[grey]Not Set[/]";
                    string checkOutDate = booking.CheckOutDate.HasValue
                        ? booking.CheckOutDate.Value.ToString("yyyy-MM-dd")
                        : "[grey]Not Set[/]";

                    table.AddRow(
                        $"{booking.Guest.FirstName} {booking.Guest.LastName}",
                        booking.Guest.GuestId.ToString(),
                        booking.BookingId.ToString(),
                        booking.RoomId.ToString(),
                        status,
                        checkInDate,
                        checkOutDate,
                        invoiceAmount,
                        paymentStatus
                    );
                }

                AnsiConsole.Write(table);
            }
        }



        public void DisplayPaidBookings()
        {
            Console.Clear();
            Console.WriteLine("PAID BOOKINGS");
            Console.WriteLine(new string('-', 60));

            var paidBookings = _bookingRepository.GetPaidBookings();

            if (!paidBookings.Any())
            {
                Console.WriteLine("No paid bookings found.");
            }
            else
            {
                foreach (var booking in paidBookings)
                {
                    Console.WriteLine($"Guest: {booking.Guest.FirstName} {booking.Guest.LastName}");
                    Console.WriteLine($"Booking ID: {booking.BookingId}\tRoom: {booking.RoomId}");
                    Console.WriteLine("Status: Paid");
                    Console.WriteLine(new string('-', 60));
                }
            }

            Console.WriteLine("\nPress any key to return...");
            Console.ReadKey();
        }

        private void DisplayPreviousGuestHistory()
        {
            Console.Clear();

            var previousBookings = _bookingRepository.GetAllBookings()
                .Where(b => b.IsCheckedOut)
                .ToList();

            if (!previousBookings.Any())
            {
                AnsiConsole.Markup("[red]No previous guests found.[/]");
            }
            else
            {
                var table = new Table();
                table.Border(TableBorder.Rounded);

                table.AddColumn("[bold yellow]Guest[/]");
                table.AddColumn("[bold yellow]Booking ID[/]");
                table.AddColumn("[bold yellow]Room[/]");
                table.AddColumn("[bold yellow]Checked Out On[/]");

                foreach (var booking in previousBookings)
                {
                    string guestName = $"{booking.Guest.FirstName} {booking.Guest.LastName}";
                    string checkOutDate = booking.CheckOutDate.HasValue ? booking.CheckOutDate.Value.ToString("yyyy-MM-dd") : "[grey]N/A[/]";

                    table.AddRow(
                        guestName,
                        booking.BookingId.ToString(),
                        booking.RoomId.ToString(),
                        checkOutDate
                    );
                }

                AnsiConsole.Write(table);
            }

            AnsiConsole.Markup("\n[bold yellow]Press any key to return...[/]");
            Console.ReadKey();
        }

        public void DisplayAllRegisteredGuests()
        {
            Console.Clear();
            Console.WriteLine("ALL REGISTERED GUESTS");
            Console.WriteLine(new string('-', 60));

            var guests = _bookingRepository.GetAllBookings()
                .Select(b => b.Guest)
                .Distinct()
                .ToList();

            if (!guests.Any())
            {
                AnsiConsole.MarkupLine("[red]No registered guests found.[/]");
            }
            else
            {
                var table = new Table();

                table.AddColumn("Guest ID");
                table.AddColumn("Name");
                table.AddColumn("Email");
                table.AddColumn("Phone Number");

                foreach (var guest in guests)
                {
                    table.AddRow(
                        guest.GuestId.ToString(),
                        $"{guest.FirstName} {guest.LastName}",
                        guest.Email,
                        guest.PhoneNumber);
                }

                AnsiConsole.Write(table);
            }
            Console.ReadKey();
        }

        public void EditBooking()
        {
            
        }

        //public void EditBooking()
        //{
        //    Console.Clear();
        //    AnsiConsole.Markup("[bold yellow]Overview of all existing bookings[/]\n");

        //    DisplayActiveBookings();

        //    AnsiConsole.Markup("Enter a [green]Booking ID[/] you wish to edit:\n");
        //    if (!int.TryParse(Console.ReadLine(), out int bookingId))
        //    {
        //        AnsiConsole.Markup("[red]Invalid Booking ID. Press any key to return...[/]\n");
        //        Console.ReadKey();
        //        return;
        //    }

        //    var booking = _bookingRepository.GetBookingById(bookingId);

        //    if (booking == null)
        //    {
        //        AnsiConsole.Markup("[red]Booking not found. Press any key to return...[/]\n");
        //        Console.ReadKey();
        //        return;
        //    }

        //    while (true)
        //    {
        //        Console.Clear();
        //        AnsiConsole.Markup($"[bold green]Booking ID {booking.BookingId}[/]\n");

        //        var room = _roomRepository.GetRoomById(booking.RoomId);
        //        var invoice = _bookingRepository.GetInvoiceByBookingId(booking.BookingId);

        //        if (room != null)
        //        {
        //            invoice.TotalAmount = _bookingRepository.CalculateTotalAmount(booking);
        //            _bookingRepository.UpdateInvoice(invoice);
        //        }

        //        var bookingDetailsTable = new Table()
        //            .Border(TableBorder.Rounded)
        //            .AddColumn("[bold yellow]Detail[/]")
        //            .AddColumn("[bold yellow]Information[/]")
        //            .AddRow("Guest", $"{booking.Guest.FirstName} {booking.Guest.LastName}")
        //            .AddRow("Email", booking.Guest.Email)
        //            .AddRow("Phone Number", booking.Guest.PhoneNumber)
        //            .AddRow("Room ID", booking.RoomId.ToString())
        //            .AddRow("Room Type", room?.Type ?? "[red]N/A[/]")
        //            .AddRow("Check-In Date", booking.CheckInDate?.ToString("yyyy-MM-dd") ?? "[grey]Not Set[/]")
        //            .AddRow("Check-Out Date", booking.CheckOutDate?.ToString("yyyy-MM-dd") ?? "[grey]Not Set[/]")
        //            .AddRow("Amount", $"{invoice.TotalAmount:C}")
        //            .AddRow("Booking Status", booking.IsCheckedIn ? "[green]Checked In[/]" : "[yellow]Not Checked In[/]");
        //        AnsiConsole.Write(bookingDetailsTable);

        //        var action = AnsiConsole.Prompt(
        //            new SelectionPrompt<string>()
        //                .Title("[bold green]What would you like to edit?[/]")
        //                .AddChoices(new[] { "Edit Guest Information", "Edit Room Information", "Edit Check-In Date", "Edit Check-Out Date", "Confirm Edit", "Cancel" })
        //                .HighlightStyle(new Style(foreground: Color.Green))
        //        );

        //        switch (action)
        //        {
        //            case "Edit Guest Information":
        //                while (true)
        //                {
        //                    Console.Clear();

        //                    var guestInfoTable = new Table()
        //                        .Border(TableBorder.Rounded)
        //                        .AddColumn("[bold yellow]Detail[/]")
        //                        .AddColumn("[bold yellow]Information[/]")
        //                        .AddRow("First Name", booking.Guest.FirstName)
        //                        .AddRow("Last Name", booking.Guest.LastName)
        //                        .AddRow("Email", booking.Guest.Email)
        //                        .AddRow("Phone Number", booking.Guest.PhoneNumber);
        //                    AnsiConsole.Write(guestInfoTable);

        //                    var guestAction = AnsiConsole.Prompt(
        //                        new SelectionPrompt<string>()
        //                            .Title("[bold green]Which detail would you like to edit?[/]")
        //                            .AddChoices(new[] { "First Name", "Last Name", "Email", "Phone Number", "Confirm Edit", "Cancel" })
        //                            .HighlightStyle(new Style(foreground: Color.Green))
        //                    );

        //                    if (guestAction == "Go Back")
        //                        break;

        //                    switch (guestAction)
        //                    {
        //                        case "First Name":
        //                            booking.Guest.FirstName = AnsiConsole.Ask<string>("Enter [green]new First Name[/]:");
        //                            break;

        //                        case "Last Name":
        //                            booking.Guest.LastName = AnsiConsole.Ask<string>("Enter [green]new Last Name[/]:");
        //                            break;

        //                        case "Email":
        //                            booking.Guest.Email = AnsiConsole.Ask<string>("Enter [green]new Email[/]:");
        //                            break;

        //                        case "Phone Number":
        //                            booking.Guest.PhoneNumber = AnsiConsole.Ask<string>("Enter [green]new Phone Number[/]:");
        //                            break;

        //                        case "Confirm Edit":
        //                            try
        //                            {
        //                                _bookingRepository.UpdateGuest(booking.Guest); 
        //                                AnsiConsole.Markup("[green]Guest information updated successfully![/]\n");
        //                                Console.WriteLine("\nPress any key to return to booking menu...");
        //                                Console.ReadKey();
        //                                break;
        //                            }
        //                            catch (Exception ex)
        //                            {
        //                                AnsiConsole.Markup($"[red]Error updating guest: {ex.Message}[/]\n");
        //                                Console.WriteLine("\nPress any key to try again...");
        //                                Console.ReadKey();
        //                            }
        //                            return;

        //                        default:
        //                            AnsiConsole.Markup("[red]Invalid option. Try again.[/]\n");
        //                            break;
        //                    }
        //                }
        //                break;
        //            case "Edit Check-In Date":
        //                DateTime newCheckInDate = AnsiConsole.Ask<DateTime>("Enter [green]new Check-In Date (yyyy-MM-dd)[/]:");

        //                if (newCheckInDate < DateTime.Now.Date)
        //                {
        //                    AnsiConsole.Markup("[red]Check-In Date cannot be in the past.[/]");
        //                    Console.ReadKey();
        //                }
        //                else if (booking.CheckOutDate.HasValue && newCheckInDate >= booking.CheckOutDate.Value)
        //                {
        //                    AnsiConsole.Markup("[red]Check-In Date cannot be on or after the Check-Out Date.[/]");
        //                    Console.ReadKey();
        //                }
        //                else
        //                {
        //                    booking.CheckInDate = newCheckInDate;
        //                    try
        //                    {
        //                        invoice.TotalAmount = _bookingRepository.CalculateTotalAmount(booking); 
        //                        _bookingRepository.UpdateInvoice(invoice); 
        //                        AnsiConsole.Markup("[green]Check-In Date and invoice updated successfully![/]");
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        AnsiConsole.Markup($"[red]Error updating invoice: {ex.Message}[/]");
        //                    }
        //                    Console.ReadKey();
        //                }
        //                break;

        //            case "Edit Check-Out Date":
        //                DateTime newCheckOutDate = AnsiConsole.Ask<DateTime>("Enter [green]new Check-Out Date (yyyy-MM-dd)[/]:");

        //                if (newCheckOutDate <= booking.CheckInDate)
        //                {
        //                    AnsiConsole.Markup("[red]Check-Out Date must be after the Check-In Date.[/]");
        //                    Console.ReadKey();
        //                }
        //                else
        //                {
        //                    booking.CheckOutDate = newCheckOutDate;
        //                    try
        //                    {
        //                        invoice.TotalAmount = _bookingRepository.CalculateTotalAmount(booking); 
        //                        _bookingRepository.UpdateInvoice(invoice); 
        //                        AnsiConsole.Markup("[green]Check-Out Date and invoice updated successfully![/]");
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        AnsiConsole.Markup($"[red]Error updating invoice: {ex.Message}[/]");
        //                    }
        //                    Console.ReadKey();
        //                }
        //                break;

        //            case "Edit Room Information":
        //                var roomToEdit = _roomRepository.GetRoomById(booking.RoomId);
        //                if (roomToEdit == null)
        //                {
        //                    AnsiConsole.Markup("[red]Room not found for the booking. Please check again.[/]\n");
        //                    Console.ReadKey();
        //                    break;
        //                }

        //                EditRoomDetails(roomToEdit, booking);

        //                invoice.TotalAmount = _bookingRepository.CalculateTotalAmount(booking); 
        //                _bookingRepository.UpdateInvoice(invoice); 
        //                break;


        //            case "Confirm Edit":
        //                try
        //                {
        //                    _bookingRepository.UpdateBooking(booking); 
        //                    AnsiConsole.Markup("[green]Booking updated successfully![/]\n");
        //                }
        //                catch (Exception ex)
        //                {
        //                    AnsiConsole.Markup($"[red]Error updating booking: {ex.Message}[/]\n");
        //                }
        //                Console.WriteLine("\nPress any key to return...");
        //                Console.ReadKey();
        //                return;

        //            case "Cancel":
        //                return;

        //            default:
        //                AnsiConsole.Markup("[red]Invalid option selected. Try again.[/]\n");
        //                break;
        //        }
        //    }
        //}
        //public void EditRoomDetails(Room room, Booking booking)
        //{
        //    while (true)
        //    {
        //        Console.Clear();
        //        AnsiConsole.Markup($"[bold yellow]Editing Room ID {room.RoomId}[/]\n");

        //        var roomDetailsTable = new Table()
        //            .Border(TableBorder.Rounded)
        //            .AddColumn("[bold]Property[/]")
        //            .AddColumn("[bold]Value[/]")
        //            .AddRow("Type", room.Type)
        //            .AddRow("Price Per Night", room.PricePerNight.ToString("C"))
        //            .AddRow("Size (sqm)", room.SizeInSquareMeters.ToString())
        //            .AddRow("Extra Beds", room.ExtraBeds.ToString())
        //            .AddRow("Total People Capacity", room.TotalPeople.ToString());
        //        AnsiConsole.Write(roomDetailsTable);

        //        var action = AnsiConsole.Prompt(
        //            new SelectionPrompt<string>()
        //                .Title("What would you like to do?")
        //                .AddChoices(new[]
        //                {
        //            "Unbook room, and select new room by date",
        //            "Unbook room, and select new room by searching total people",
        //            "Finish Editing"
        //                })
        //                .HighlightStyle(new Style(foreground: Color.Green))
        //        );

        //        switch (action)
        //        {
        //            case "Unbook room, and select new room by date":
        //                string roomTypeByDate = AnsiConsole.Prompt(
        //                    new SelectionPrompt<string>()
        //                        .Title("[yellow]Please select new room type:[/]")
        //                        .AddChoices("Single", "Double"));

        //                DateTime startDateByDate = SelectDateWithCalendar("[yellow]Select check-in date:[/]", roomTypeByDate);
        //                if (startDateByDate == DateTime.MinValue)
        //                {
        //                    AnsiConsole.MarkupLine("[red]Date selection canceled.[/] Returning to menu...");
        //                    return;
        //                }

        //                DateTime endDateByDate = SelectDateWithCalendar("[yellow]Select check-out date:[/]", roomTypeByDate);
        //                if (endDateByDate == DateTime.MinValue || endDateByDate <= startDateByDate)
        //                {
        //                    AnsiConsole.MarkupLine("[red]Check-out date must be after check-in date![/]");
        //                    break;
        //                }

        //                var availableRoomsByDate = _roomRepository.GetAvailableRoomsByDate(startDateByDate, endDateByDate, roomTypeByDate);
        //                if (!availableRoomsByDate.Any())
        //                {
        //                    AnsiConsole.MarkupLine("[red]No available rooms found for the selected dates and room type.[/]");
        //                    break;
        //                }

        //                DisplayAvailableRooms(availableRoomsByDate);

        //                int newRoomIdByDate = AnsiConsole.Prompt(
        //                    new TextPrompt<int>("[yellow]Enter Room ID to select:[/]")
        //                        .ValidationErrorMessage("[red]Invalid Room ID![/]")
        //                        .Validate(input => availableRoomsByDate.Any(r => r.RoomId == input)));

        //                UpdateRoomAndBooking(room, booking, newRoomIdByDate, availableRoomsByDate);
        //                break;

        //            case "Unbook room, and select new room by searching total people":
        //                int totalPeople = AnsiConsole.Ask<int>("[yellow]Enter the total number of people:[/]");

        //                var roomsByCapacity = _roomRepository.GetRoomsByCapacity(totalPeople);
        //                if (!roomsByCapacity.Any())
        //                {
        //                    AnsiConsole.MarkupLine("[red]No rooms available for the selected capacity.[/]");
        //                    break;
        //                }

        //                DisplayAvailableRooms(roomsByCapacity);

        //                int selectedRoomId = AnsiConsole.Prompt(
        //                    new TextPrompt<int>("[yellow]Enter Room ID to view availability:[/]")
        //                        .ValidationErrorMessage("[red]Invalid Room ID![/]")
        //                        .Validate(input => roomsByCapacity.Any(r => r.RoomId == input)));

        //                var selectedRoom = roomsByCapacity.First(r => r.RoomId == selectedRoomId);

        //                DateTime startDateByCapacity = SelectDateWithCalendar("[yellow]Select check-in date:[/]", selectedRoom.Type);
        //                if (startDateByCapacity == DateTime.MinValue)
        //                {
        //                    AnsiConsole.MarkupLine("[red]Date selection canceled.[/] Returning to menu...");
        //                    return;
        //                }

        //                DateTime endDateByCapacity = SelectDateWithCalendar("[yellow]Select check-out date:[/]", selectedRoom.Type);
        //                if (endDateByCapacity == DateTime.MinValue || endDateByCapacity <= startDateByCapacity)
        //                {
        //                    AnsiConsole.MarkupLine("[red]Check-out date must be after check-in date![/]");
        //                    break;
        //                }

        //                if (!_roomRepository.IsRoomAvailable(selectedRoomId, startDateByCapacity, endDateByCapacity))
        //                {
        //                    AnsiConsole.MarkupLine("[red]The selected room is not available for the chosen dates.[/]");
        //                    break;
        //                }

        //                UpdateRoomAndBooking(room, booking, selectedRoomId, roomsByCapacity);
        //                break;

        //            case "Finish Editing":
        //                AnsiConsole.Markup("[green]Finished editing room details.[/]\n");
        //                return;

        //            default:
        //                AnsiConsole.Markup("[red]Invalid option selected. Please try again.[/]\n");
        //                break;
        //        }

        //        Console.WriteLine("\nPress any key to continue...");
        //        Console.ReadKey();
        //    }
        //}


        //private void DisplayAvailableRooms(IEnumerable<Room> rooms)
        //{
        //    var table = new Table()
        //        .Border(TableBorder.Rounded)
        //        .AddColumn("[blue]Room ID[/]")
        //        .AddColumn("[blue]Room Type[/]")
        //        .AddColumn("[blue]Price per Night[/]")
        //        .AddColumn("[blue]Total People Capacity[/]");

        //    foreach (var room in rooms)
        //    {
        //        table.AddRow(
        //            room.RoomId.ToString(),
        //            room.Type,
        //            room.PricePerNight.ToString("C"),
        //            room.TotalPeople.ToString());
        //    }

        //    AnsiConsole.Write(table);
        //}

        //private void UpdateRoomAndBooking(Room currentRoom, Booking booking, int newRoomId, IEnumerable<Room> availableRooms)
        //{
        //    currentRoom.IsAvailable = true;
        //    _roomRepository.UpdateRoom(currentRoom);

        //    var newRoom = availableRooms.First(r => r.RoomId == newRoomId);
        //    newRoom.IsAvailable = false;
        //    _roomRepository.UpdateRoom(newRoom);

        //    booking.RoomId = newRoom.RoomId;
        //    _bookingRepository.UpdateBooking(booking);

        //    AnsiConsole.Markup("[green]Room successfully updated in the booking![/]\n");
        //}



        //private void RenderCalendar(DateTime selectedDate, string roomType)
        //{
        //    var calendarContent = new StringWriter();
        //    calendarContent.WriteLine($"[bold yellow]{selectedDate:MMMM yyyy}[/]".ToUpper());
        //    calendarContent.WriteLine("Mon  Tue  Wed  Thu  Fri  Sat  Sun");
        //    calendarContent.WriteLine("─────────────────────────────────");

        //    DateTime firstDayOfMonth = new DateTime(selectedDate.Year, selectedDate.Month, 1);
        //    int daysInMonth = DateTime.DaysInMonth(selectedDate.Year, selectedDate.Month);
        //    int startDay = (int)firstDayOfMonth.DayOfWeek;
        //    startDay = (startDay == 0) ? 6 : startDay - 1;

        //    var bookedDates = _bookingRepository.GetAllBookings()
        //        .Where(b => b.Room.Type.Equals(roomType, StringComparison.OrdinalIgnoreCase))
        //        .SelectMany(b => Enumerable.Range(0, 1 + (b.CheckOutDate.Value - b.CheckInDate.Value).Days)
        //                                    .Select(offset => b.CheckInDate.Value.AddDays(offset)))
        //        .ToHashSet();

        //    for (int i = 0; i < startDay; i++)
        //    {
        //        calendarContent.Write("     ");
        //    }

        //    for (int day = 1; day <= daysInMonth; day++)
        //    {
        //        DateTime currentDate = new DateTime(selectedDate.Year, selectedDate.Month, day);

        //        if (currentDate == selectedDate)
        //        {
        //            calendarContent.Write($"[blue]{day,2}[/]   ");
        //        }
        //        else if (bookedDates.Contains(currentDate))
        //        {
        //            calendarContent.Write($"[red]{day,2}[/]   ");
        //        }
        //        else if (currentDate < DateTime.Now.Date)
        //        {
        //            calendarContent.Write($"[grey]{day,2}[/]   ");
        //        }
        //        else
        //        {
        //            calendarContent.Write($"{day,2}   ");
        //        }

        //        if ((startDay + day) % 7 == 0)
        //        {
        //            calendarContent.WriteLine();
        //        }
        //    }

        //    var panel = new Panel(calendarContent.ToString())
        //    {
        //        Border = BoxBorder.Double,
        //        Header = new PanelHeader($"[yellow]{selectedDate:yyyy}[/]", Justify.Center)
        //    };

        //    AnsiConsole.Write(panel);
        //    Console.WriteLine();
        //    AnsiConsole.MarkupLine("[blue]Use arrow keys to navigate and Enter to select a date. Press Escape to cancel.[/]");
        //}

        //private DateTime SelectDateWithCalendar(string prompt, string roomType)
        //{
        //    DateTime currentDate = DateTime.Now.Date;
        //    DateTime selectedDate = currentDate;

        //    // Hämta alla bokade datum för det valda rumstypen
        //    var bookedDates = _bookingRepository.GetAllBookings()
        //        .Where(b => b.CheckInDate.HasValue && b.CheckOutDate.HasValue) // Endast giltiga datum
        //        .Where(b => b.Room.Type.Equals(roomType, StringComparison.OrdinalIgnoreCase)) // Filtrera på rumstyp
        //        .SelectMany(b => Enumerable.Range(0, 1 + (b.CheckOutDate.Value - b.CheckInDate.Value).Days)
        //                                    .Select(offset => b.CheckInDate.Value.AddDays(offset))) // Skapa datumintervall
        //        .ToHashSet(); // För snabb lookup av bokade datum

        //    while (true)
        //    {
        //        Console.Clear();
        //        Console.WriteLine(prompt);
        //        RenderCalendar(selectedDate, roomType); // Visa kalender med det valda datumet markerat

        //        var key = Console.ReadKey(true).Key;
        //        switch (key)
        //        {
        //            case ConsoleKey.RightArrow:
        //                selectedDate = selectedDate.AddDays(1); // Nästa dag
        //                break;
        //            case ConsoleKey.LeftArrow:
        //                if (selectedDate > currentDate)
        //                    selectedDate = selectedDate.AddDays(-1); // Föregående dag, ej tidigare än idag
        //                break;
        //            case ConsoleKey.UpArrow:
        //                if (selectedDate.AddDays(-7) >= currentDate)
        //                    selectedDate = selectedDate.AddDays(-7); // Föregående vecka, ej tidigare än idag
        //                break;
        //            case ConsoleKey.DownArrow:
        //                selectedDate = selectedDate.AddDays(7); // Nästa vecka
        //                break;
        //            case ConsoleKey.Enter:
        //                // Kontrollera om datumet redan är bokat
        //                if (bookedDates.Contains(selectedDate))
        //                {
        //                    AnsiConsole.MarkupLine("[red]The selected date is already booked for this room type.[/]");
        //                    Console.ReadKey(true);
        //                    continue; // Låt användaren välja igen
        //                }

        //                // Validera att datumet inte är i det förflutna
        //                if (selectedDate >= currentDate)
        //                    return selectedDate;

        //                AnsiConsole.MarkupLine("[red]The date cannot be in the past.[/]");
        //                Console.ReadKey(true);
        //                break;
        //            case ConsoleKey.Escape:
        //                return DateTime.MinValue; // Returnera ett ogiltigt värde för att signalera avbrott
        //        }
        //    }
        //}


        //public void UnbookBookings()
        //{
        //    while (true)
        //    {
        //        Console.Clear();

        //        var notCheckedInBookings = _bookingRepository.GetAllBookings()
        //            .Where(b => !b.IsCanceled && !b.IsCheckedIn && !b.IsCheckedOut) // Endast bokningar som inte är incheckade
        //            .ToList();

        //        if (!notCheckedInBookings.Any())
        //        {
        //            AnsiConsole.Markup("[red]No bookings available to cancel[/]\n");
        //            Console.WriteLine("\nPress any key to return...");
        //            Console.ReadKey();
        //            return;
        //        }

        //        var bookingChoices = notCheckedInBookings
        //            .Select(b => $"{b.BookingId}: {b.Guest.FirstName} {b.Guest.LastName} (Room ID: {b.RoomId}, Check-In: {b.CheckInDate:yyyy-MM-dd})")
        //            .ToList();
        //        bookingChoices.Add("Back"); 

        //        var selectedBooking = AnsiConsole.Prompt(
        //            new SelectionPrompt<string>()
        //                .Title("[bold yellow] Select a booking to cancel:[/]")
        //                .AddChoices(bookingChoices)
        //                .HighlightStyle(new Style(foreground: Color.Green))
        //        );

        //        // Om användaren väljer "Back"
        //        if (selectedBooking == "Back")
        //            break;

        //        // Extrahera Booking ID från det valda alternativet
        //        var bookingId = int.Parse(selectedBooking.Split(':')[0]);
        //        var booking = notCheckedInBookings.FirstOrDefault(b => b.BookingId == bookingId);

        //        if (booking == null)
        //        {
        //            AnsiConsole.MarkupLine("[red]Booking not found. Please try again.[/]");
        //            Console.ReadKey();
        //            continue;
        //        }

        //        // Bekräfta avbokningen
        //        var confirm = AnsiConsole.Confirm($"Are you sure you want to cancel booking [yellow]{bookingId}[/] for [green]{booking.Guest.FirstName} {booking.Guest.LastName}[/]?");
        //        if (!confirm)
        //            continue;

        //        // Avbryt bokningen
        //        booking.IsCanceled = true;
        //        _bookingRepository.UpdateBooking(booking);

        //        AnsiConsole.MarkupLine($"[green]Booking {bookingId} has been successfully canceled.[/]");

        //        Console.WriteLine("\nPress any key to return...");
        //        Console.ReadKey();
        //        break;
        //    }
        //}




        public void DisplayExpiredBookings()
        {
            Console.Clear();
            Console.WriteLine("Unpaid Bookings");

            var expiredBookings = _bookingRepository.GetExpiredUnpaidBookings();

            if (!expiredBookings.Any())
            {
                Console.WriteLine("No expired unpaid bookings found.");
                Console.ReadKey();
                return;
            }

            foreach (var booking in expiredBookings)
            {
                _bookingRepository.CancelBooking(booking);
                Console.WriteLine($"Booking ID {booking.BookingId} for Guest ID {booking.GuestId} has been cancelled due to non-payment.");
            }

            Console.WriteLine("\nAll expired unpaid bookings have been processed.");
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }
        //public void CheckIn()
        //{
        //    while (true)
        //    {
        //        Console.Clear();
                
                
        //        Console.WriteLine("CHECK IN GUEST");
        //        DisplayActiveBookings();
        //        Console.Write("Enter 'ESC' to go back " +
        //            "\nEnter guest 'booking ID' to check them in: ");
        //        string input = Console.ReadLine()?.Trim();
        //        if (input?.ToUpper() == "ESC") break;

        //        if (!int.TryParse(input, out int checkInId))
        //        {
        //            Console.WriteLine("Invalid Booking ID. Try again...");
        //            Console.ReadKey();
        //            continue;
        //        }

        //        var booking = _bookingRepository.GetBookingById(checkInId);

        //        if (booking == null)
        //        {
        //            Console.WriteLine($"Booking with ID {checkInId} does not exist.");
        //            Console.ReadKey();
        //            continue;
        //        }

        //        if (booking.IsCheckedIn)
        //        {
        //            Console.WriteLine($"Booking ID {checkInId} is already checked in.");
        //            Console.ReadKey();
        //            continue;
        //        }

        //        _bookingRepository.CheckInGuest(booking);

        //        Console.WriteLine($"Guest {booking.Guest.FirstName} {booking.Guest.LastName} successfully checked in!");
        //        Console.WriteLine($"Booking ID: {booking.BookingId}, Room ID: {booking.RoomId}");
        //        Console.ReadKey();
        //    }
        //}
        //public void CheckOut()
        //{
        //    Console.Clear();
        //    DisplayActiveBookings();  

        //    Console.Write("Enter Guest ID to check out: ");
        //    if (!int.TryParse(Console.ReadLine(), out int guestId))
        //    {
        //        Console.WriteLine("Invalid Guest ID.");
        //        Console.WriteLine("Press any key to return to the main menu...");
        //        Console.ReadKey();  
        //        return;
        //    }

        //    var booking = _bookingRepository.GetActiveBookingByGuestId(guestId);
        //    if (booking == null)
        //    {
        //        Console.WriteLine("No active booking found.");
        //        Console.WriteLine("Press any key to return to the main menu...");
        //        Console.ReadKey();  
        //        return;
        //    }

        //    var invoice = _bookingRepository.GetInvoiceByBookingId(booking.BookingId)
        //                  ?? _bookingRepository.GenerateInvoiceForBooking(booking);

        //    Console.WriteLine($"Invoice Total: {invoice.TotalAmount:C}");
        //    Console.Write("Enter payment amount: ");
        //    if (!decimal.TryParse(Console.ReadLine(), out decimal paymentAmount) || paymentAmount < invoice.TotalAmount)
        //    {
        //        Console.WriteLine("Invalid or insufficient amount.");
        //        Console.WriteLine("Press any key to return to the main menu...");
        //        Console.ReadKey(); 
        //        return;
        //    }

        //    _bookingRepository.ProcessPayment(invoice, paymentAmount);

        //    booking.IsCheckedOut = true;
        //    booking.BookingStatus = true;
        //    booking.CheckOutDate = DateTime.Now;

        //    _bookingRepository.UpdateBooking(booking);

        //    var room = _roomRepository.GetRoomById(booking.RoomId);
        //    if (room != null)
        //    {
        //        room.IsAvailable = true; 
        //        _roomRepository.UpdateRoom(room);
        //    }

        //    Console.WriteLine("Guest successfully checked out and payment processed.");
        //    Console.WriteLine("Press any key to return to the main menu...");
        //    Console.ReadKey();  
        //}

   

        public void PayInvoiceBeforeCheckout()
        {
            Console.Clear();
            DisplayActiveBookings();
            Console.Write("Enter Booking ID: ");
            if (!int.TryParse(Console.ReadLine(), out int bookingId))
            {
                Console.WriteLine("Invalid Booking ID.");
                Console.ReadKey();
                return;
            }

            var invoice = _bookingRepository.GetInvoiceByBookingId(bookingId);
            if (invoice == null || invoice.IsPaid)
            {
                Console.WriteLine("No unpaid invoice found.");
                Console.ReadKey();
                return;
            }

            Console.WriteLine($"Invoice Total: {invoice.TotalAmount:C}");
            Console.Write("Enter payment amount: ");
            if (!decimal.TryParse(Console.ReadLine(), out decimal paymentAmount) || paymentAmount < invoice.TotalAmount)
            {
                Console.WriteLine("Invalid or insufficient amount.");
                Console.ReadKey();
                return;
            }

            _bookingRepository.ProcessPayment(invoice, paymentAmount);
            Console.WriteLine("Invoice paid successfully.");
            Console.ReadKey();
        }

        public void BookingManagement()
        {
            while (true)
            {
                Console.Clear();

                var activeBookings = _bookingRepository.GetAllBookings()
                    .Where(b => !b.IsCanceled && !b.IsCheckedOut)
                    .ToList();
                var completedBookings = _bookingRepository.GetAllBookings()
                    .Where(b => b.IsCheckedOut)
                    .ToList();
                var removedBookings = _bookingRepository.GetAllBookings()
                    .Where(b => b.IsCanceled)
                    .ToList();

                _tableDisplayService.DisplayBookingTable(activeBookings, "Active Bookings:");
                _tableDisplayService.DisplayBookingTable(completedBookings, "Completed Bookings:", includePaymentAndStatus: true);
                _tableDisplayService.DisplayBookingTable(removedBookings, "Unbooked Bookings:", includePaymentAndStatus: false);

                var action = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("[bold green]What would you like to do?[/]")
                        .AddChoices(new[] { "Check In/Check Out", "Register New Booking", "Edit Booking", "Unbook Booking", "Payment", "Go Back" })
                        .HighlightStyle(new Style(foreground: Color.Green))
                );

                switch (action)
                {
                    case "Check In/Check Out":
                        _checkInOutService.Execute();
                        break;
                    case "Register New Booking":
                        _guestController.RegisterNewGuest();
                        break;
                    case "Edit Booking":
                        _bookingEditService.EditBooking();
                        break;
                    case "Unbook Booking":
                        _unbookBooking.UnbookBookings();
                        break;
                    case "Payment":
                        _paymentService.PayInvoiceBeforeCheckout();
                        break;
                    case "Go Back":
                        return;
                    default:
                        AnsiConsole.Markup("[red]Invalid option. Try again.[/]\n");
                        break;
                }
            }
        }


    }
}
