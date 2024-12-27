using HotelBookingApp.Data;
using HotelBookingApp.Entities;
using HotelBookingApp.Repositories;
using Microsoft.EntityFrameworkCore;
using Spectre.Console;
using System;
using System.Linq;

namespace HotelBookingApp.Controllers
{
    public class GuestController
    {
        private readonly GuestRepository _guestRepository;
        private readonly BookingRepository _bookingRepository;

        public GuestController(AppDbContext context, BookingRepository bookingRepository, GuestRepository guestRepository)
        {
            _guestRepository = guestRepository;
            _bookingRepository = bookingRepository;
        }
        public void RegisterNewGuest()
        {
            Console.Clear();
            AnsiConsole.MarkupLine("[bold green]Register New Guest[/]\n");

            var guest = CollectGuestInformation();
            if (guest == null)
            {
                AnsiConsole.MarkupLine("[red]Registration has been canceled.[/]");
                return;
            }

            bool createBooking = AnsiConsole.Confirm("Would you like to create a booking for this guest?");
            if (createBooking)
            {
                var booking = CollectBookingDetailsWithCalendar(guest);
                if (booking == null)
                {
                    AnsiConsole.MarkupLine("[red]Booking has been canceled.[/]");
                    return;
                }

                // Beräkna det totala beloppet för bokningen
                decimal totalAmount = _guestRepository.CalculateTotalAmount(booking);

                // Skapa faktura kopplad till bokningen
                var invoice = new Invoice
                {
                    BookingId = booking.BookingId,
                    TotalAmount = totalAmount,
                    IsPaid = false,
                    PaymentDeadline = booking.CheckOutDate?.AddDays(7) ?? DateTime.Now.AddDays(7)
                };

                // Registrera gästen med bokning och faktura
                _guestRepository.RegisterNewGuestWithBookingAndInvoice(guest, booking, invoice);

                AnsiConsole.MarkupLine("[bold green]Guest has been successfully registered and booked![/]");
                AnsiConsole.MarkupLine($"[yellow]Invoice created:[/] Total Amount: {totalAmount:C}, Payment Deadline: {invoice.PaymentDeadline:yyyy-MM-dd}");
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
            string firstName = AnsiConsole.Prompt(
                new TextPrompt<string>("[yellow]Please enter your first name:[/]")
                    .ValidationErrorMessage("[red]First name cannot be empty[/]")
                    .Validate(input => !string.IsNullOrWhiteSpace(input)));

            string lastName = AnsiConsole.Prompt(
                new TextPrompt<string>("[yellow]Please enter your last name:[/]")
                    .ValidationErrorMessage("[red]Last name cannot be empty[/]")
                    .Validate(input => !string.IsNullOrWhiteSpace(input)));

            string email = AnsiConsole.Prompt(
                new TextPrompt<string>("[yellow]Please enter your email address (must include @):[/]")
                    .ValidationErrorMessage("[red]Invalid email[/]")
                    .Validate(input => input.Contains("@")));

            string phone = AnsiConsole.Prompt(
                new TextPrompt<string>("[yellow]Please enter your phone number:[/]")
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

        private Booking CollectBookingDetailsWithCalendar(Guest guest)
        {
            string roomType = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[yellow]Please select room type:[/]")
                    .AddChoices("Single", "Double"));

            DateTime startDate = SelectDateWithCalendar("[yellow]Select check-in date:[/]", roomType);
            if (startDate == DateTime.MinValue)
            {
                return null;
            }

            DateTime endDate = SelectDateWithCalendar("[yellow]Select check-out date:[/]", roomType);
            if (endDate == DateTime.MinValue || endDate <= startDate)
            {
                AnsiConsole.MarkupLine("[red]Check-out date must be after check-in date![/]");
                return null;
            }

            var availableRooms = _guestRepository.GetAvailableRooms(startDate, endDate, roomType);
            if (!availableRooms.Any())
            {
                AnsiConsole.MarkupLine("[red]No available rooms found for the selected dates and room type.[/]");
                return null;
            }

            AnsiConsole.MarkupLine("\n[bold green]Available Rooms:[/]");
            var table = new Table()
                .Border(TableBorder.Rounded)
                .AddColumn("[blue]Room ID[/]")
                .AddColumn("[blue]Room Type[/]")
                .AddColumn("[blue]Price per Night[/]");

            foreach (var room in availableRooms)
            {
                table.AddRow(room.RoomId.ToString(), room.Type, room.PricePerNight.ToString("C"));
            }

            AnsiConsole.Write(table);

            int roomId = AnsiConsole.Prompt(
                new TextPrompt<int>("[yellow]Enter Room ID to book:[/]")
                    .ValidationErrorMessage("[red]Invalid Room ID![/]")
                    .Validate(input => availableRooms.Any(r => r.RoomId == input)));

            return new Booking
            {
                RoomId = roomId,
                CheckInDate = startDate,
                CheckOutDate = endDate,
                GuestId = guest.GuestId,
                IsCheckedIn = false,
                IsCheckedOut = false,
                BookingStatus = false
            };
        }

        private DateTime SelectDateWithCalendar(string prompt, string roomType)
        {
            DateTime currentDate = DateTime.Now.Date;
            DateTime selectedDate = currentDate;

            while (true)
            {
                Console.Clear();
                Console.WriteLine(prompt);
                RenderCalendar(selectedDate, roomType);

                var key = Console.ReadKey(true).Key;
                switch (key)
                {
                    case ConsoleKey.RightArrow:
                        selectedDate = selectedDate.AddDays(1);
                        break;
                    case ConsoleKey.LeftArrow:
                        if (selectedDate > currentDate)
                            selectedDate = selectedDate.AddDays(-1);
                        break;
                    case ConsoleKey.UpArrow:
                        if (selectedDate.AddDays(-7) >= currentDate)
                            selectedDate = selectedDate.AddDays(-7);
                        break;
                    case ConsoleKey.DownArrow:
                        selectedDate = selectedDate.AddDays(7);
                        break;
                    case ConsoleKey.Enter:
                        return selectedDate;
                    case ConsoleKey.Escape:
                        return DateTime.MinValue; // Cancel
                }
            }
        }

        private void RenderCalendar(DateTime selectedDate, string roomType)
        {
            var calendarContent = new StringWriter();
            calendarContent.WriteLine($"[bold yellow]{selectedDate:MMMM yyyy}[/]".ToUpper());
            calendarContent.WriteLine("Mon  Tue  Wed  Thu  Fri  Sat  Sun");
            calendarContent.WriteLine("─────────────────────────────────");

            // Hämta första dagen i månaden och antal dagar
            DateTime firstDayOfMonth = new DateTime(selectedDate.Year, selectedDate.Month, 1);
            int daysInMonth = DateTime.DaysInMonth(selectedDate.Year, selectedDate.Month);
            int startDay = (int)firstDayOfMonth.DayOfWeek;
            startDay = (startDay == 0) ? 6 : startDay - 1; // Söndag som sista dag

            // Hämta bokade datum för angivet rumstyp
            var bookedDates = _guestRepository.GetBookedDates(selectedDate.Month, selectedDate.Year, roomType);

            // Fyll i tomma celler före första dagen i månaden
            for (int i = 0; i < startDay; i++)
            {
                calendarContent.Write("     "); // Skapa utrymme för veckostart
            }

            // Skapa kalender med markerade dagar
            for (int day = 1; day <= daysInMonth; day++)
            {
                DateTime currentDate = new DateTime(selectedDate.Year, selectedDate.Month, day);

                if (currentDate == selectedDate)
                {
                    calendarContent.Write($"[blue]{day,2}[/]   "); // Markera valt datum
                }
                else if (bookedDates.Contains(currentDate))
                {
                    calendarContent.Write($"[red]{day,2}[/]   "); // Markera bokade datum
                }
                else if (currentDate < DateTime.Now.Date)
                {
                    calendarContent.Write($"[grey]{day,2}[/]   "); // Markera förflutna datum
                }
                else
                {
                    calendarContent.Write($"{day,2}   "); // Tillgängliga datum
                }

                // Bryt rad för varje vecka (7 dagar)
                if ((startDay + day) % 7 == 0)
                {
                    calendarContent.WriteLine();
                }
            }

            // Skapa och visa kalenderpanelen
            var panel = new Panel(calendarContent.ToString())
            {
                Border = BoxBorder.Double,
                Header = new PanelHeader($"[yellow]{selectedDate:yyyy}[/]", Justify.Center)
            };

            AnsiConsole.Write(panel);
            Console.WriteLine();
            AnsiConsole.MarkupLine("[blue]Use arrow keys to navigate and Enter to select a date. Press Escape to cancel.[/]");
        }



        //public void RegisterNewGuest()
        //{
        //    Console.Clear();
        //    AnsiConsole.MarkupLine("[bold green]Register New Guest[/]\n");

        //    string firstName = "", lastName = "", email = "", phone = "";
        //    bool editDetails = true;

        //    while (editDetails)
        //    {
        //        firstName = AnsiConsole.Prompt(
        //            new TextPrompt<string>("[yellow]Please enter your first name:[/]")
        //                .ValidationErrorMessage("[red]First name cannot be empty[/]")
        //                .Validate(input => !string.IsNullOrWhiteSpace(input)));

        //        lastName = AnsiConsole.Prompt(
        //            new TextPrompt<string>("[yellow]Please enter your last name:[/]")
        //                .ValidationErrorMessage("[red]Last name cannot be empty[/]")
        //                .Validate(input => !string.IsNullOrWhiteSpace(input)));

        //        email = AnsiConsole.Prompt(
        //            new TextPrompt<string>("[yellow]Please enter your email address (must include @):[/]")
        //                .ValidationErrorMessage("[red]Invalid email[/]")
        //                .Validate(input => input.Contains("@")));

        //        phone = AnsiConsole.Prompt(
        //            new TextPrompt<string>("[yellow]Please enter your phone number:[/]")
        //                .ValidationErrorMessage("[red]Invalid phone number![/]")
        //                .Validate(input => long.TryParse(input, out _)));

        //        AnsiConsole.MarkupLine("\n[bold green]Summary of your information:[/]");
        //        var table = new Table()
        //            .Border(TableBorder.Rounded)
        //            .AddColumn("[blue]Field[/]")
        //            .AddColumn("[blue]Value[/]")
        //            .AddRow("First Name", firstName)
        //            .AddRow("Last Name", lastName)
        //            .AddRow("Email", email)
        //            .AddRow("Phone Number", phone);

        //        AnsiConsole.Write(table);

        //        var choice = AnsiConsole.Prompt(
        //            new SelectionPrompt<string>()
        //                .Title("[yellow]What would you like to do?[/]")
        //                .AddChoices("Confirm and Continue", "Edit Information", "Cancel Registration"));

        //        if (choice == "Confirm and Continue")
        //        {
        //            editDetails = false;
        //        }
        //        else if (choice == "Cancel Registration")
        //        {
        //            AnsiConsole.MarkupLine("[red]Registration has been canceled.[/]");
        //            return;
        //        }
        //    }

        //    var newGuest = new Guest
        //    {
        //        FirstName = firstName,
        //        LastName = lastName,
        //        Email = email,
        //        PhoneNumber = phone
        //    };

        //    bool createBooking = AnsiConsole.Confirm("Would you like to create a booking for this guest?");
        //    if (createBooking)
        //    {
        //        int guestCount = AnsiConsole.Prompt(
        //            new TextPrompt<int>("[yellow]Enter total number of guests:[/]")
        //                .ValidationErrorMessage("[red]Total guests must be a number![/]")
        //                .Validate(input => input > 0));

        //        string roomType = AnsiConsole.Prompt(
        //            new SelectionPrompt<string>()
        //                .Title("[yellow]Please select room type:[/]")
        //                .AddChoices("Single", "Double"));

        //        DateTime startDate = DateTime.MinValue, endDate = DateTime.MinValue;
        //        bool editDates = true;
        //        List<Room> availableRooms = null;

        //        while (editDates)
        //        {
        //            startDate = SelectDate("[yellow]Select check-in date:[/]", roomType);
        //            endDate = SelectDate("[yellow]Select check-out date:[/]", roomType);

        //            if (endDate <= startDate)
        //            {
        //                AnsiConsole.MarkupLine("[red]Check-out date must be after the check-in date![/]");
        //                continue;
        //            }

        //            availableRooms = _guestRepository.GetAvailableRooms(startDate, endDate, guestCount)
        //                .Where(r => r.Type == roomType)
        //                .ToList();

        //            if (!availableRooms.Any())
        //            {
        //                AnsiConsole.MarkupLine("[red]No available rooms found for the selected room type. Please try again.[/]");
        //                continue;
        //            }

        //            AnsiConsole.MarkupLine("\n[bold green]Summary of selected dates:[/]");
        //            var dateTable = new Table()
        //                .Border(TableBorder.Rounded)
        //                .AddColumn("[blue]Field[/]")
        //                .AddColumn("[blue]Value[/]")
        //                .AddRow("Check-in Date", startDate.ToShortDateString())
        //                .AddRow("Check-out Date", endDate.ToShortDateString());

        //            AnsiConsole.Write(dateTable);

        //            var dateChoice = AnsiConsole.Prompt(
        //                new SelectionPrompt<string>()
        //                    .Title("[yellow]What would you like to do?[/]")
        //                    .AddChoices("Confirm and Continue", "Edit Dates", "Cancel Booking"));

        //            if (dateChoice == "Confirm and Continue")
        //            {
        //                editDates = false;
        //            }
        //            else if (dateChoice == "Cancel Booking")
        //            {
        //                AnsiConsole.MarkupLine("[red]Booking has been canceled.[/]");
        //                return;
        //            }
        //        }

        //        AnsiConsole.MarkupLine("\n[bold green]Available Rooms:[/]");
        //        var roomTable = new Table()
        //            .Border(TableBorder.Rounded)
        //            .AddColumn("[blue]Room ID[/]")
        //            .AddColumn("[blue]Room Type[/]")
        //            .AddColumn("[blue]Price per Night[/]");

        //        foreach (var room in availableRooms)
        //        {
        //            roomTable.AddRow(room.RoomId.ToString(), room.Type, room.PricePerNight.ToString("C"));
        //        }

        //        AnsiConsole.Write(roomTable);

        //        int roomId = AnsiConsole.Prompt(
        //            new TextPrompt<int>("[yellow]Enter Room ID to book:[/]")
        //                .ValidationErrorMessage("[red]Invalid Room ID![/]")
        //                .Validate(input => availableRooms.Any(r => r.RoomId == input)));

        //        var selectedRoom = availableRooms.First(r => r.RoomId == roomId);

        //        int extraBeds = 0;
        //        if (selectedRoom.Type == "Double")
        //        {
        //            extraBeds = AnsiConsole.Prompt(
        //                new TextPrompt<int>("[yellow]How many extra beds would you like? (0-2):[/]")
        //                    .ValidationErrorMessage("[red]Enter a number between 0 and 2.[/]")
        //                    .Validate(input => input >= 0 && input <= 2));

        //            selectedRoom.TotalPeople += extraBeds;
        //        }

        //        var booking = new Booking
        //        {
        //            RoomId = roomId,
        //            CheckInDate = startDate,
        //            CheckOutDate = endDate,
        //            IsCheckedIn = false,
        //            IsCheckedOut = false,
        //            BookingStatus = false
        //        };

        //        decimal totalAmount = _guestRepository.CalculateTotalAmount(booking);
        //        totalAmount += extraBeds * selectedRoom.ExtraBedPrice;

        //        var invoice = new Invoice
        //        {
        //            BookingId = booking.BookingId,
        //            TotalAmount = totalAmount,
        //            IsPaid = false,
        //            PaymentDeadline = endDate.AddDays(7)
        //        };

        //        _guestRepository.RegisterNewGuestWithBooking(newGuest, booking, invoice);

        //        AnsiConsole.MarkupLine("[bold green]\nGuest has been successfully registered and booked![/]");
        //        Console.ReadKey();
        //    }
        //}

        private DateTime SelectDate(string prompt, string selectedRoomType)
        {
            DateTime currentDate = DateTime.Now.Date;
            DateTime selectedDate = currentDate;  

            while (true)
            {
                Console.Clear();
                Console.WriteLine(prompt);
                RenderCalendar(selectedDate, selectedRoomType);  

                var key = Console.ReadKey(true).Key;
                switch (key)
                {
                    case ConsoleKey.RightArrow:
                        selectedDate = selectedDate.AddDays(1); 
                        break;
                    case ConsoleKey.LeftArrow:
                        selectedDate = selectedDate.AddDays(-1);  
                        break;
                    case ConsoleKey.UpArrow:
                        selectedDate = selectedDate.AddDays(-7); 
                        break;
                    case ConsoleKey.DownArrow:
                        selectedDate = selectedDate.AddDays(7);  
                        break;
                    case ConsoleKey.Enter:
                        var bookings = _bookingRepository.GetAllBookings()
                            .Where(b => b.CheckInDate.HasValue && b.CheckOutDate.HasValue)
                            .Where(b => b.CheckInDate.Value.Date <= selectedDate.Date && b.CheckOutDate.Value.Date >= selectedDate.Date)
                            .Where(b => b.Room.Type == selectedRoomType)  
                            .ToList();

                        if (bookings.Any())
                        {
                            AnsiConsole.MarkupLine("[red]The selected date is already booked for this room type.[/]");  
                            Console.ReadKey(true);
                            continue;  
                        }

                        if (selectedDate >= currentDate)
                            return selectedDate;  

                        AnsiConsole.MarkupLine("[red]The date cannot be in the past.[/]");  
                        Console.ReadKey(true);
                        break;
                    case ConsoleKey.Escape:
                        return DateTime.MinValue; 
                }
            }
        }



        //private void RenderCalendar(DateTime selectedDate, string selectedRoomType)
        //{
        //    var calendarContent = new StringWriter();
        //    calendarContent.WriteLine($"[bold yellow]{selectedDate:MMMM yyyy}[/]".ToUpper());
        //    calendarContent.WriteLine("Mon  Tue  Wed  Thu  Fri  Sat  Sun");
        //    calendarContent.WriteLine("─────────────────────────────────");

        //    DateTime firstDayOfMonth = new DateTime(selectedDate.Year, selectedDate.Month, 1);
        //    int daysInMonth = DateTime.DaysInMonth(selectedDate.Year, selectedDate.Month);
        //    int startDay = (int)firstDayOfMonth.DayOfWeek;
        //    startDay = (startDay == 0) ? 6 : startDay - 1; // Justera för att börja från måndag

        //    // Hämta bokningar för det valda rummet och datumet
        //    var bookings = _bookingRepository.GetAllBookings()
        //        .Where(b => b.CheckInDate.HasValue && b.CheckOutDate.HasValue)
        //        .Where(b => b.CheckInDate.Value.Month == selectedDate.Month && b.CheckInDate.Value.Year == selectedDate.Year)
        //        .Where(b => b.Room.Type == selectedRoomType)  // Filtrera på rumstyp
        //        .ToList();

        //    HashSet<DateTime> bookedDates = new HashSet<DateTime>();
        //    HashSet<DateTime> availableDatesAfterCheckout = new HashSet<DateTime>();

        //    // Lägg till alla datum mellan check-in och check-out som bokade
        //    foreach (var booking in bookings)
        //    {
        //        for (DateTime date = booking.CheckInDate.Value; date <= booking.CheckOutDate.Value; date = date.AddDays(1))
        //        {
        //            bookedDates.Add(date);  // Lägg till alla dagar för bokningen
        //        }
        //        // Lägg till datum efter utcheckning som tillgängliga
        //        availableDatesAfterCheckout.Add(booking.CheckOutDate.Value.AddDays(1));
        //    }

        //    // Fyll i tomma celler innan första dagen
        //    for (int i = 0; i < startDay; i++)
        //    {
        //        calendarContent.Write("     "); // Fyller tomt innan första dagen
        //    }

        //    // Loop genom alla dagar i månaden
        //    for (int day = 1; day <= daysInMonth; day++)
        //    {
        //        DateTime currentDateToDisplay = new DateTime(selectedDate.Year, selectedDate.Month, day);
        //        string dateDisplay = currentDateToDisplay.Day.ToString("D2");

        //        // Markera det valda datumet
        //        if (currentDateToDisplay.Date == selectedDate.Date)
        //        {
        //            calendarContent.Write($"[blue]{dateDisplay}[/]   ");  // Markera valda datumet med blått
        //        }
        //        //// Om datumet är bokat, markera det som rött
        //        //else if (bookedDates.Contains(currentDateToDisplay))
        //        //{
        //        //    calendarContent.Write($"[red]{dateDisplay}[/]   ");
        //        //}
        //        //// Om datumet är tillgängligt efter utcheckning, markera det som grönt
        //        //else if (availableDatesAfterCheckout.Contains(currentDateToDisplay))
        //        //{
        //        //    calendarContent.Write($"[green]{dateDisplay}[/]   ");
        //        //}
        //        // Om datumet är förflutet, markera det som överstruket i grått
        //        else if (currentDateToDisplay < DateTime.Now.Date)
        //        {
        //            calendarContent.Write($"[grey]{dateDisplay}[/]   ");
        //        }
        //        // Om datumet är tillgängligt, markera det som normalt
        //        else
        //        {
        //            calendarContent.Write($"{dateDisplay}   ");
        //        }

        //        // När veckan är slut (7 dagar), börja på en ny rad
        //        if ((startDay + day) % 7 == 0)
        //        {
        //            calendarContent.WriteLine();
        //        }
        //    }

        //    // Skapa och visa kalendern för varje rum
        //    var panel = new Panel(calendarContent.ToString())
        //    {
        //        Border = BoxBorder.Double,
        //        Header = new PanelHeader($"[red]{selectedDate:yyyy}[/]", Justify.Center)
        //    };

        //    AnsiConsole.Write(panel);
        //    Console.WriteLine();
        //    AnsiConsole.MarkupLine("\nUse arrow keys [blue]\u25C4 \u25B2 \u25BA \u25BC[/] to navigate and [green]Enter[/] to select.");
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


        private DateTime PromptForDate(string message)
        {
            while (true)
            {
                Console.Write(message);
                if (DateTime.TryParse(Console.ReadLine(), out DateTime date))
                {
                    if (date.Date < DateTime.Now.Date)
                    {
                        Console.WriteLine("The date cannot be in the past. Please enter a valid future date.");
                        continue;
                    }
                    return date;
                }
                Console.WriteLine("Invalid date format. Please use yyyy-MM-dd.");
            }
        }

        public void UpdateGuestInformation()
        {
            Console.Clear();
            Console.WriteLine("UPDATE GUEST DETAILS");

            var guests = _guestRepository.GetAllGuests();

            if (!guests.Any())
            {
                Console.WriteLine("No guests found.");
                Console.ReadKey(true);
                return;
            }

            Console.WriteLine("\nSelect a guest to update by ID:");
            foreach (var guest in guests)
            {
                Console.WriteLine($"ID: {guest.GuestId} | Name: {guest.FirstName} {guest.LastName}");
            }

            var guestId = PromptForInt("Enter Guest ID: ");
            var selectedGuest = _guestRepository.GetGuestById(guestId);

            if (selectedGuest == null)
            {
                Console.WriteLine("Guest not found. Returning to menu...");
                Console.ReadKey(true);
                return;
            }

            Console.WriteLine("\nLeave fields blank to keep current value.");

            selectedGuest.FirstName = PromptInput($"Current First Name: {selectedGuest.FirstName}\nEnter new First Name: ", selectedGuest.FirstName);
            selectedGuest.LastName = PromptInput($"Current Last Name: {selectedGuest.LastName}\nEnter new Last Name: ", selectedGuest.LastName);
            selectedGuest.Email = PromptInput($"Current Email: {selectedGuest.Email}\nEnter new Email: ", selectedGuest.Email);
            selectedGuest.PhoneNumber = PromptInput($"Current Phone Number: {selectedGuest.PhoneNumber}\nEnter new Phone Number: ", selectedGuest.PhoneNumber);

            _guestRepository.UpdateGuest(selectedGuest);

            Console.WriteLine("\nGuest information updated successfully!");
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey(true);
        }

        public void ViewAllGuests()
        {
            Console.Clear();
            Console.WriteLine("ALL GUESTS");

            var guests = _guestRepository.GetGuestsWithBookings();

            if (!guests.Any())
            {
                Console.WriteLine("No guests found.");
                Console.ReadKey(true);
                return;
            }

            foreach (var entry in guests)
            {
                var guest = ((dynamic)entry).Guest;
                var bookings = ((dynamic)entry).Bookings as IEnumerable<dynamic>;

                foreach (var booking in bookings)
                {
                    var invoice = booking.Invoice != null
                        ? $"Amount: {booking.Invoice.TotalAmount:C}, Status: {(booking.Invoice.IsPaid ? "Paid" : "Not Paid")}"
                        : "No Invoice"; 

                    Console.WriteLine($"Guest: {guest.FirstName} {guest.LastName} | Room: {booking.RoomId} | " +
                                      $"Check-In: {booking.CheckInDate:yyyy-MM-dd} | Check-Out: {booking.CheckOutDate:yyyy-MM-dd} | {invoice}");
                }
            }

            Console.WriteLine("\nPress any key to return...");
            Console.ReadKey(true);
        }



        private string PromptInput(string message, string defaultValue = null)
        {
            Console.Write(message);
            var input = Console.ReadLine();
            return string.IsNullOrWhiteSpace(input) ? defaultValue : input;
        }

        private int PromptForInt(string message)
        {
            int result;
            while (true)
            {
                Console.Write(message);
                if (int.TryParse(Console.ReadLine(), out result))
                {
                    return result;
                }
                Console.WriteLine("Invalid input. Please enter a valid number.");
            }
        }

    }
}
