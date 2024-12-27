using HotelBookingApp.Repositories;
using System;
using Spectre.Console;
using HotelBookingApp.Entities;
using HotelBookingApp.Data;

namespace HotelBookingApp
{
    public class BookingController
    {
        private readonly BookingRepository _bookingRepository;
        private readonly RoomRepository _roomRepository;

        public BookingController(BookingRepository bookingRepository, RoomRepository roomRepository = null)
        {
            _bookingRepository = bookingRepository;
            _roomRepository = roomRepository;
        }

        public void SearchAvailableRooms()
        {
            Console.Clear();
            Console.WriteLine("Function: Find Available Rooms");

            DateTime startDate = SelectDate("Select start date:");

            DateTime endDate = SelectDate("Select end date:");

            if (endDate <= startDate)
            {
                Console.WriteLine("End date must be after start date.");
                Console.ReadKey();
                return;
            }

            Console.WriteLine("Enter the number of guests:");
            if (!int.TryParse(Console.ReadLine(), out int guestCount) || guestCount <= 0)
            {
                Console.WriteLine("Invalid number of guests.");
                return;
            }

            var availableRooms = _bookingRepository.GetAvailableRooms(startDate, endDate, guestCount);

            if (!availableRooms.Any())
            {
                Console.WriteLine("No available rooms found.");
            }
            else
            {
                Console.WriteLine($"\nAvailable rooms from {startDate:yyyy-MM-dd} to {endDate:yyyy-MM-dd}:");
                foreach (var room in availableRooms)
                {
                    Console.WriteLine($"Room {room.RoomId}: {room.Type}, {room.PricePerNight:C} per night");
                }
            }

            Console.WriteLine("\nPress any key to return to the menu...");
            Console.ReadKey();
        }

        private DateTime SelectDate(string prompt)
        {
            DateTime currentDate = DateTime.Now.Date; 
            DateTime selectedDate = new DateTime(currentDate.Year, currentDate.Month, 1); 

            while (true)
            {
                Console.Clear();
                Console.WriteLine(prompt);
                RenderCalendar(selectedDate);

                var key = Console.ReadKey(true).Key;
                switch (key)
                {
                    case ConsoleKey.RightArrow:
                        selectedDate = selectedDate.AddDays(1);
                        break;
                    case ConsoleKey.LeftArrow:
                        if (selectedDate.AddDays(-1) >= currentDate)
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
                        if (selectedDate >= currentDate)
                            return selectedDate;
                        AnsiConsole.MarkupLine("[red]You cannot select a date in the past![/]");
                        Console.ReadKey();
                        break;
                    case ConsoleKey.Escape:
                        return DateTime.MinValue; 
                }
            }
        }


        private void RenderCalendar(DateTime selectedDate)
        {
            var calendarContent = new StringWriter();

            calendarContent.WriteLine($"[red]{selectedDate:MMMM}[/]".ToUpper());
            calendarContent.WriteLine("Mon  Tue  Wed  Thu  Fri  Sat  Sun");
            calendarContent.WriteLine("─────────────────────────────────");

            DateTime firstDayOfMonth = new DateTime(selectedDate.Year, selectedDate.Month, 1);
            int daysInMonth = DateTime.DaysInMonth(selectedDate.Year, selectedDate.Month);
            int startDay = (int)firstDayOfMonth.DayOfWeek;
            startDay = (startDay == 0) ? 6 : startDay - 1; 

            for (int i = 0; i < startDay; i++)
            {
                calendarContent.Write("     ");
            }

            for (int day = 1; day <= daysInMonth; day++)
            {
                if (day == selectedDate.Day)
                {
                    calendarContent.Write($"[green]{day,2}[/]   ");
                }
                else
                {
                    calendarContent.Write($"{day,2}   ");
                }

                if ((startDay + day) % 7 == 0)
                {
                    calendarContent.WriteLine();
                }
            }

            var panel = new Panel(calendarContent.ToString())
            {
                Border = BoxBorder.Double,
                Header = new PanelHeader($"[red]{selectedDate:yyyy}[/]", Justify.Center)
            };

            AnsiConsole.Write(panel);
            Console.WriteLine();
            AnsiConsole.MarkupLine("\nUse arrow keys [blue]\u25C4 \u25B2 \u25BA \u25BC[/] to navigate and [green]Enter[/] to select.");
        }


        public void DisplayAllGuestInfo()
        {
            DisplayGuestOptions();
        }

        public void DisplayGuestOptions()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("SELECT WHAT TO VIEW:");
                Console.WriteLine(new string('-', 40));
                Console.WriteLine("1. Active Bookings");
                Console.WriteLine("2. History of Previous Guests");
                Console.WriteLine("3. All Registered Guests");
                Console.WriteLine("4. History of removed bookings");
                Console.WriteLine("5. Return to Main Menu");
                Console.WriteLine(new string('-', 40));

                Console.Write("Enter your choice (1-4): ");
                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        Console.Clear();
                        DisplayActiveBookings();
                        Console.WriteLine("Enter anywhere to go back");
                        Console.ReadKey();
                        break;
                    case "2":
                        DisplayPreviousGuestHistory();
                        break;
                    case "3":
                        DisplayAllRegisteredGuests();
                        break;
                    case "4":
                        DisplayCanceledBookings();
                        break;
                    case "5":
                        return;
                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        Console.ReadKey();
                        break;
                }
            }
        }
        private void DisplayActiveBookings()
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
                        booking.Guest.GuestId.ToString(), // Display Guest ID here
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

            Console.WriteLine("\nPress any key to return...");
            Console.ReadKey();
        }
        public void EditBooking()
        {
            Console.Clear();
            Console.WriteLine("EDIT BOOKING");
            DisplayActiveBookings();

            Console.Write("Enter Booking ID to edit: ");
            if (!int.TryParse(Console.ReadLine(), out int bookingId))
            {
                Console.WriteLine("Invalid Booking ID. Press any key to return...");
                Console.ReadKey();
                return;
            }

            var booking = _bookingRepository.GetBookingById(bookingId);

            if (booking == null)
            {
                Console.WriteLine("Booking not found. Press any key to return...");
                Console.ReadKey();
                return;
            }

            Console.WriteLine("\nWhat would you like to do?");
            Console.WriteLine("1. Update Booking Information");
            Console.WriteLine("2. Cancel Booking");

            Console.Write("Enter your choice: ");
            if (!int.TryParse(Console.ReadLine(), out int choice) || choice < 1 || choice > 3)
            {
                Console.WriteLine("Invalid choice. Press any key to return...");
                Console.ReadKey();
                return;
            }

            switch (choice)
            {
                case 1: 
                    UpdateBooking(booking);
                    break;

                case 2: 
                    CancelBooking();
                    break;
            }
        }

        private void UpdateBooking(Booking booking)
        {
            Console.WriteLine("\nUPDATE BOOKING");

            Console.WriteLine($"Current Room ID: {booking.RoomId}");
            Console.Write("Enter new Room ID (leave blank to keep current): ");
            var newRoomIdInput = Console.ReadLine();
            if (int.TryParse(newRoomIdInput, out int newRoomId))
            {
                booking.RoomId = newRoomId;
            }

            Console.WriteLine($"Current Check-In Date: {booking.CheckInDate:yyyy-MM-dd}");
            Console.Write("Enter new Check-In Date (leave blank to keep current): ");
            var newCheckInDateInput = Console.ReadLine();
            if (DateTime.TryParse(newCheckInDateInput, out DateTime newCheckInDate))
            {
                booking.CheckInDate = newCheckInDate;
            }

            Console.WriteLine($"Current Check-Out Date: {booking.CheckOutDate:yyyy-MM-dd}");
            Console.Write("Enter new Check-Out Date (leave blank to keep current): ");
            var newCheckOutDateInput = Console.ReadLine();
            if (DateTime.TryParse(newCheckOutDateInput, out DateTime newCheckOutDate))
            {
                booking.CheckOutDate = newCheckOutDate;
            }

            _bookingRepository.UpdateBooking(booking);

            Console.WriteLine($"Booking {booking.BookingId} updated successfully. Press any key to return...");
            Console.ReadKey();
        }

        public void CancelBooking()
        {
            Console.Clear();
            Console.WriteLine("\nCANCEL BOOKING");

            // Begär inmatning av Booking ID från användaren
            Console.Write("Enter Booking ID to cancel: ");
            if (!int.TryParse(Console.ReadLine(), out int bookingId))
            {
                Console.WriteLine("Invalid Booking ID. Press any key to return...");
                Console.ReadKey();
                return;
            }

            var booking = _bookingRepository.GetBookingById(bookingId);

            if (booking == null)
            {
                Console.WriteLine("Booking not found. Press any key to return...");
                Console.ReadKey();
                return;
            }

            // Markera bokningen som avbruten genom att sätta IsCanceled till true
            booking.IsCanceled = true;

            // Uppdatera bokningen i databasen
            _bookingRepository.UpdateBooking(booking);

            Console.WriteLine($"Booking {booking.BookingId} has been canceled.\n");

            // Visa alla avbrutna bokningar
            DisplayCanceledBookings();
        }


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
        public void CheckIn()
        {
            while (true)
            {
                Console.Clear();
                
                
                Console.WriteLine("CHECK IN GUEST");
                DisplayActiveBookings();
                Console.Write("Enter 'ESC' to go back " +
                    "\nEnter guest 'booking ID' to check them in: ");
                string input = Console.ReadLine()?.Trim();
                if (input?.ToUpper() == "ESC") break;

                if (!int.TryParse(input, out int checkInId))
                {
                    Console.WriteLine("Invalid Booking ID. Try again...");
                    Console.ReadKey();
                    continue;
                }

                var booking = _bookingRepository.GetBookingById(checkInId);

                if (booking == null)
                {
                    Console.WriteLine($"Booking with ID {checkInId} does not exist.");
                    Console.ReadKey();
                    continue;
                }

                if (booking.IsCheckedIn)
                {
                    Console.WriteLine($"Booking ID {checkInId} is already checked in.");
                    Console.ReadKey();
                    continue;
                }

                _bookingRepository.CheckInGuest(booking);

                Console.WriteLine($"Guest {booking.Guest.FirstName} {booking.Guest.LastName} successfully checked in!");
                Console.WriteLine($"Booking ID: {booking.BookingId}, Room ID: {booking.RoomId}");
                Console.ReadKey();
            }
        }


        public void CheckOut()
        {
            Console.Clear();
            DisplayActiveBookings();  // Visa de aktiva bokningarna

            Console.Write("Enter Guest ID to check out: ");
            if (!int.TryParse(Console.ReadLine(), out int guestId))
            {
                Console.WriteLine("Invalid Guest ID.");
                Console.WriteLine("Press any key to return to the main menu...");
                Console.ReadKey();  // Vänta på att användaren trycker en knapp
                return;
            }

            var booking = _bookingRepository.GetActiveBookingByGuestId(guestId);
            if (booking == null)
            {
                Console.WriteLine("No active booking found.");
                Console.WriteLine("Press any key to return to the main menu...");
                Console.ReadKey();  // Vänta på att användaren trycker en knapp
                return;
            }

            var invoice = _bookingRepository.GetInvoiceByBookingId(booking.BookingId)
                          ?? _bookingRepository.GenerateInvoiceForBooking(booking);

            Console.WriteLine($"Invoice Total: {invoice.TotalAmount:C}");
            Console.Write("Enter payment amount: ");
            if (!decimal.TryParse(Console.ReadLine(), out decimal paymentAmount) || paymentAmount < invoice.TotalAmount)
            {
                Console.WriteLine("Invalid or insufficient amount.");
                Console.WriteLine("Press any key to return to the main menu...");
                Console.ReadKey();  // Vänta på att användaren trycker en knapp
                return;
            }

            _bookingRepository.ProcessPayment(invoice, paymentAmount);

            // Uppdatera bokningen och rummet
            booking.IsCheckedOut = true;
            booking.BookingStatus = true;
            booking.CheckOutDate = DateTime.Now;

            _bookingRepository.UpdateBooking(booking);

            var room = _roomRepository.GetRoomById(booking.RoomId);
            if (room != null)
            {
                room.IsAvailable = true;  // Gör rummet tillgängligt
                _roomRepository.UpdateRoom(room);
            }

            Console.WriteLine("Guest successfully checked out and payment processed.");
            Console.WriteLine("Press any key to return to the main menu...");
            Console.ReadKey();  // Vänta på användaren att trycka en knapp innan man återgår
        }

        public void DisplayCanceledBookings()
        {
            Console.Clear();
            Console.WriteLine("CANCELED BOOKINGS HISTORY");
            Console.WriteLine(new string('-', 80));

            // Filtrera bokningar där IsCanceled är true
            var canceledBookings = _bookingRepository.GetAllBookings()
                .Where(b => b.IsCanceled)  // Endast bokningar som har blivit avbrutna
                .ToList();

            if (!canceledBookings.Any())
            {
                Console.WriteLine("No canceled bookings found.");
            }
            else
            {
                var table = new Table();
                table.Border(TableBorder.Rounded);

                table.AddColumn("[bold yellow]Booking ID[/]");
                table.AddColumn("[bold yellow]Guest ID[/]");
                table.AddColumn("[bold yellow]Room ID[/]");
                table.AddColumn("[bold yellow]Check-In Date[/]");
                table.AddColumn("[bold yellow]Check-Out Date[/]");

                // Lägg till rader för varje avbruten bokning
                foreach (var booking in canceledBookings)
                {
                    table.AddRow(
                        booking.BookingId.ToString(),
                        booking.GuestId.ToString(),
                        booking.RoomId.ToString(),
                        booking.CheckInDate.HasValue ? booking.CheckInDate.Value.ToString("yyyy-MM-dd") : "[grey]N/A[/]",
                        booking.CheckOutDate.HasValue ? booking.CheckOutDate.Value.ToString("yyyy-MM-dd") : "[grey]N/A[/]"
                    );
                }

                AnsiConsole.Write(table);
            }

            Console.WriteLine("Press any key to return...");
            Console.ReadKey();
        }





        public void PayInvoiceBeforeCheckout()
        {
            Console.Clear();
            DisplayActiveBookings();
            Console.Write("Enter Booking ID: ");
            if (!int.TryParse(Console.ReadLine(), out int bookingId))
            {
                Console.WriteLine("Invalid Booking ID.");
                return;
            }

            var invoice = _bookingRepository.GetInvoiceByBookingId(bookingId);
            if (invoice == null || invoice.IsPaid)
            {
                Console.WriteLine("No unpaid invoice found.");
                return;
            }

            Console.WriteLine($"Invoice Total: {invoice.TotalAmount:C}");
            Console.Write("Enter payment amount: ");
            if (!decimal.TryParse(Console.ReadLine(), out decimal paymentAmount) || paymentAmount < invoice.TotalAmount)
            {
                Console.WriteLine("Invalid or insufficient amount.");
                return;
            }

            _bookingRepository.ProcessPayment(invoice, paymentAmount);
            Console.WriteLine("Invoice paid successfully.");
            Console.ReadKey();
        }
        


        public void CheckInOrCheckOut()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("CHECK-IN / CHECK-OUT GUEST");

                // Display active bookings
                DisplayActiveBookings();

                Console.WriteLine("\nEnter 'ESC' to go back.");
                Console.Write("Enter Booking ID to Check-In/Check-Out: ");

                string input = Console.ReadLine()?.Trim();
                if (input?.ToUpper() == "ESC") break;

                if (!int.TryParse(input, out int bookingId))
                {
                    AnsiConsole.MarkupLine("[red]Invalid input. Please enter a valid numeric Booking ID.[/]");
                    Console.ReadKey();
                    continue;
                }

                // Filter active bookings
                var activeBookings = _bookingRepository.GetAllBookings()
                    .Where(b => !b.IsCanceled && b.BookingStatus != true) // Only active bookings
                    .ToList();

                var booking = activeBookings.FirstOrDefault(b => b.BookingId == bookingId);

                if (booking == null)
                {
                    AnsiConsole.MarkupLine($"[red]Booking ID {bookingId} is not listed in active bookings. Please choose a valid ID from the active table.[/]");
                    Console.ReadKey();
                    continue;
                }

                Console.WriteLine("\nWhat action would you like to perform?");
                Console.WriteLine("1. Check In");
                Console.WriteLine("2. Check Out");

                Console.Write("Enter your choice (1 or 2): ");
                if (!int.TryParse(Console.ReadLine(), out int actionChoice) || actionChoice < 1 || actionChoice > 2)
                {
                    AnsiConsole.MarkupLine("[red]Invalid choice. Please enter 1 for Check In or 2 for Check Out.[/]");
                    Console.ReadKey();
                    continue;
                }

                switch (actionChoice)
                {
                    case 1: // Check-In
                        HandleCheckIn(booking);
                        break;

                    case 2: // Check-Out
                        HandleCheckOut(booking);
                        break;
                }

                Console.WriteLine("Press any key to return...");
                Console.ReadKey();
            }
        }

        public void HandleCheckIn(Booking booking)
        {
            if (booking.IsCheckedIn)
            {
                AnsiConsole.MarkupLine($"[yellow]Booking ID {booking.BookingId} is already checked in.[/]");
                return;
            }

            // Fråga om gästen vill betala vid incheckning
            bool payAtCheckIn = AnsiConsole.Confirm("Would the guest like to pay at check-in?");

            if (payAtCheckIn)
            {
                var invoice = _bookingRepository.GetInvoiceByBookingId(booking.BookingId)
                                  ?? _bookingRepository.GenerateInvoiceForBooking(booking);

                AnsiConsole.MarkupLine($"Invoice Total: [green]{invoice.TotalAmount:C}[/]");

                decimal paymentAmount = 0;
                bool validPayment = false;

                // Loopa tills användaren skriver in exakt korrekt belopp
                while (!validPayment)
                {
                    Console.Write("Enter payment amount: ");
                    if (decimal.TryParse(Console.ReadLine(), out paymentAmount))
                    {
                        if (paymentAmount == invoice.TotalAmount)
                        {
                            validPayment = true;
                        }
                        else
                        {
                            AnsiConsole.MarkupLine($"[red]Incorrect amount. The exact amount to pay is {invoice.TotalAmount:C}. Please try again.[/]");
                        }
                    }
                    else
                    {
                        AnsiConsole.MarkupLine("[red]Invalid input. Please enter a numeric value.[/]");
                    }
                }

                // Behandla betalningen
                _bookingRepository.ProcessPayment(invoice, paymentAmount);
            }

            // Uppdatera bokningen som incheckad
            booking.IsCheckedIn = true;
            _bookingRepository.UpdateBooking(booking);

            AnsiConsole.MarkupLine($"[green]Guest {booking.Guest.FirstName} {booking.Guest.LastName} successfully checked in![/]");
            AnsiConsole.MarkupLine($"[green]Booking ID: {booking.BookingId}, Room ID: {booking.RoomId}[/]");
        }




        public void HandleCheckOut(Booking booking)
        {
            if (!booking.IsCheckedIn)
            {
                AnsiConsole.MarkupLine($"[red]Booking ID {booking.BookingId} has not been checked in yet. You cannot check out before check-in.[/]");
                return;
            }

            if (booking.IsCheckedOut)
            {
                AnsiConsole.MarkupLine($"[yellow]Booking ID {booking.BookingId} is already checked out.[/]");
                return;
            }

            var invoice = _bookingRepository.GetInvoiceByBookingId(booking.BookingId)
                              ?? _bookingRepository.GenerateInvoiceForBooking(booking);

            if (invoice.IsPaid)
            {
                AnsiConsole.MarkupLine($"[green]Booking ID {booking.BookingId} is already fully paid. No further payment is required.[/]");
            }
            else
            {
                AnsiConsole.MarkupLine($"Invoice Total: [green]{invoice.TotalAmount:C}[/]");

                decimal paymentAmount = 0;
                bool validPayment = false;

                // Loopa tills användaren skriver in exakt korrekt belopp
                while (!validPayment)
                {
                    Console.Write("Enter payment amount: ");
                    if (decimal.TryParse(Console.ReadLine(), out paymentAmount))
                    {
                        if (paymentAmount == invoice.TotalAmount)
                        {
                            validPayment = true;
                        }
                        else
                        {
                            AnsiConsole.MarkupLine($"[red]Incorrect amount. The exact amount to pay is {invoice.TotalAmount:C}. Please try again.[/]");
                        }
                    }
                    else
                    {
                        AnsiConsole.MarkupLine("[red]Invalid input. Please enter a numeric value.[/]");
                    }
                }

                // Behandla betalningen
                _bookingRepository.ProcessPayment(invoice, paymentAmount);
            }

            // Markera bokningen som utcheckad
            booking.IsCheckedOut = true;
            booking.CheckOutDate = DateTime.Now;
            booking.BookingStatus = true;

            _bookingRepository.UpdateBooking(booking);

            var room = _roomRepository.GetRoomById(booking.RoomId);
            if (room != null)
            {
                room.IsAvailable = true;
                _roomRepository.UpdateRoom(room);
            }

            AnsiConsole.MarkupLine("[green]Guest successfully checked out.[/]");
        }





        //public void CheckInOrCheckOut()
        //{
        //    while (true)
        //    {
        //        Console.Clear();
        //        Console.WriteLine("CHECK-IN / CHECK-OUT GUEST");

        //        // Display active bookings
        //        DisplayActiveBookings();

        //        Console.WriteLine("\nEnter 'ESC' to go back.");
        //        Console.WriteLine("Enter Booking ID to Check-In/Check-Out:");

        //        string input = Console.ReadLine()?.Trim();
        //        if (input?.ToUpper() == "ESC") break;

        //        if (!int.TryParse(input, out int bookingId))
        //        {
        //            Console.WriteLine("Invalid Booking ID. Try again...");
        //            Console.ReadKey();
        //            continue;
        //        }

        //        var booking = _bookingRepository.GetBookingById(bookingId);

        //        if (booking == null)
        //        {
        //            Console.WriteLine($"Booking with ID {bookingId} does not exist.");
        //            Console.ReadKey();
        //            continue;
        //        }

        //        Console.WriteLine("\nWhat action would you like to perform?");
        //        Console.WriteLine("1. Check In");
        //        Console.WriteLine("2. Check Out");

        //        Console.Write("Enter your choice (1 or 2): ");
        //        if (!int.TryParse(Console.ReadLine(), out int actionChoice) || actionChoice < 1 || actionChoice > 2)
        //        {
        //            Console.WriteLine("Invalid choice. Try again...");
        //            Console.ReadKey();
        //            continue;
        //        }

        //        switch (actionChoice)
        //        {
        //            case 1: // Check-In
        //                if (booking.IsCheckedIn)
        //                {
        //                    Console.WriteLine($"Booking ID {bookingId} is already checked in.");
        //                }
        //                else
        //                {
        //                    // Ask if guest wants to pay at check-in
        //                    bool payAtCheckIn = AnsiConsole.Confirm("Would the guest like to pay at check-in?");

        //                    if (payAtCheckIn)
        //                    {
        //                        // Generate and display the invoice for payment at check-in
        //                        var invoice = _bookingRepository.GetInvoiceByBookingId(booking.BookingId)
        //                                              ?? _bookingRepository.GenerateInvoiceForBooking(booking);

        //                        Console.WriteLine($"Invoice Total: {invoice.TotalAmount:C}");

        //                        decimal paymentAmount = 0;
        //                        bool validPayment = false;

        //                        // Loop until a valid payment amount is provided
        //                        while (!validPayment)
        //                        {
        //                            Console.Write("Enter payment amount: ");
        //                            if (decimal.TryParse(Console.ReadLine(), out paymentAmount) && paymentAmount >= invoice.TotalAmount)
        //                            {
        //                                validPayment = true; // Payment is valid
        //                            }
        //                            else
        //                            {
        //                                Console.WriteLine("[red]Invalid or insufficient amount. Please try again.[/]");
        //                            }
        //                        }

        //                        _bookingRepository.ProcessPayment(invoice, paymentAmount);  // Process the payment immediately

        //                        // Update booking status to checked-in
        //                        booking.IsCheckedIn = true;
        //                        _bookingRepository.UpdateBooking(booking);

        //                        Console.WriteLine($"Guest {booking.Guest.FirstName} {booking.Guest.LastName} successfully checked in and paid!");
        //                        Console.WriteLine($"Booking ID: {booking.BookingId}, Room ID: {booking.RoomId}");
        //                    }
        //                    else
        //                    {
        //                        _bookingRepository.CheckInGuest(booking);  // Just check-in without payment
        //                        booking.IsCheckedIn = true;
        //                        _bookingRepository.UpdateBooking(booking);
        //                        Console.WriteLine($"Guest {booking.Guest.FirstName} {booking.Guest.LastName} successfully checked in without payment.");
        //                    }
        //                }
        //                break;

        //            case 2: // Check-Out
        //                if (!booking.IsCheckedIn)
        //                {
        //                    Console.WriteLine($"Booking ID {bookingId} has not checked in yet. You cannot check out before check-in.");
        //                }
        //                else if (booking.IsCheckedOut)
        //                {
        //                    Console.WriteLine($"Booking ID {bookingId} is already checked out.");
        //                }
        //                else
        //                {
        //                    // Kontrollera fakturans betalstatus
        //                    var invoice = _bookingRepository.GetInvoiceByBookingId(booking.BookingId)
        //                                          ?? _bookingRepository.GenerateInvoiceForBooking(booking);

        //                    if (invoice.IsPaid)
        //                    {
        //                        Console.WriteLine($"Booking ID {bookingId} is already fully paid. No further payment is required.");
        //                    }
        //                    else
        //                    {
        //                        Console.WriteLine($"Invoice Total: {invoice.TotalAmount:C}");

        //                        decimal paymentAmount = 0;
        //                        bool validPayment = false;

        //                        // Loop tills en giltig betalning är genomförd
        //                        while (!validPayment)
        //                        {
        //                            Console.Write("Enter payment amount: ");
        //                            if (decimal.TryParse(Console.ReadLine(), out paymentAmount) && paymentAmount >= invoice.TotalAmount)
        //                            {
        //                                validPayment = true; // Betalning är giltig
        //                            }
        //                            else
        //                            {
        //                                Console.WriteLine("[red]Invalid or insufficient amount. Please try again.[/]");
        //                            }
        //                        }

        //                        _bookingRepository.ProcessPayment(invoice, paymentAmount);
        //                    }

        //                    // Uppdatera bokningen och rummets status
        //                    booking.IsCheckedOut = true;
        //                    booking.CheckOutDate = DateTime.Now;
        //                    booking.BookingStatus = true;

        //                    _bookingRepository.UpdateBooking(booking);

        //                    var room = _roomRepository.GetRoomById(booking.RoomId);
        //                    if (room != null)
        //                    {
        //                        room.IsAvailable = true; // Gör rummet tillgängligt igen
        //                        _roomRepository.UpdateRoom(room);
        //                    }

        //                    Console.WriteLine("Guest successfully checked out.");
        //                }
        //                break;

        //        }

        //        Console.WriteLine("Press any key to return...");
        //        Console.ReadKey();
        //    }
        //}
        //public void CheckInOrCheckOut()
        //{
        //    while (true)
        //    {
        //        Console.Clear();
        //        Console.WriteLine("CHECK-IN / CHECK-OUT GUEST");

        //        // Display active bookings
        //        DisplayActiveBookings();

        //        Console.WriteLine("\nEnter 'ESC' to go back.");
        //        Console.WriteLine("Enter Booking ID to Check-In/Check-Out:");

        //        string input = Console.ReadLine()?.Trim();
        //        if (input?.ToUpper() == "ESC") break;

        //        if (!int.TryParse(input, out int bookingId))
        //        {
        //            Console.WriteLine("Invalid Booking ID. Try again...");
        //            Console.ReadKey();
        //            continue;
        //        }

        //        var booking = _bookingRepository.GetBookingById(bookingId);

        //        if (booking == null)
        //        {
        //            Console.WriteLine($"Booking with ID {bookingId} does not exist.");
        //            Console.ReadKey();
        //            continue;
        //        }

        //        Console.WriteLine("\nWhat action would you like to perform?");
        //        Console.WriteLine("1. Check In");
        //        Console.WriteLine("2. Check Out");

        //        Console.Write("Enter your choice (1 or 2): ");
        //        if (!int.TryParse(Console.ReadLine(), out int actionChoice) || actionChoice < 1 || actionChoice > 2)
        //        {
        //            Console.WriteLine("Invalid choice. Try again...");
        //            Console.ReadKey();
        //            continue;
        //        }

        //        switch (actionChoice)
        //        {
        //            case 1: // Check-In
        //                if (booking.IsCheckedIn)
        //                {
        //                    Console.WriteLine($"Booking ID {bookingId} is already checked in.");
        //                }
        //                else
        //                {
        //                    // Ask if guest wants to pay at check-in
        //                    bool payAtCheckIn = AnsiConsole.Confirm("Would the guest like to pay at check-in?");

        //                    if (payAtCheckIn)
        //                    {
        //                        // Generate and display the invoice for payment at check-in
        //                        var invoice = _bookingRepository.GetInvoiceByBookingId(booking.BookingId)
        //                                              ?? _bookingRepository.GenerateInvoiceForBooking(booking);

        //                        Console.WriteLine($"Invoice Total: {invoice.TotalAmount:C}");

        //                        decimal paymentAmount = 0;
        //                        bool validPayment = false;

        //                        // Loop until a valid payment amount is provided
        //                        while (!validPayment)
        //                        {
        //                            Console.Write("Enter payment amount: ");
        //                            if (decimal.TryParse(Console.ReadLine(), out paymentAmount) && paymentAmount >= invoice.TotalAmount)
        //                            {
        //                                validPayment = true; // Payment is valid
        //                            }
        //                            else
        //                            {
        //                                Console.WriteLine("[red]Invalid or insufficient amount. Please try again.[/]");
        //                            }
        //                        }

        //                        _bookingRepository.ProcessPayment(invoice, paymentAmount);  // Process the payment immediately

        //                        // Update booking status to checked-in
        //                        booking.IsCheckedIn = true;
        //                        _bookingRepository.UpdateBooking(booking);

        //                        Console.WriteLine($"Guest {booking.Guest.FirstName} {booking.Guest.LastName} successfully checked in and paid!");
        //                        Console.WriteLine($"Booking ID: {booking.BookingId}, Room ID: {booking.RoomId}");
        //                    }
        //                    else
        //                    {
        //                        _bookingRepository.CheckInGuest(booking);  // Just check-in without payment
        //                        booking.IsCheckedIn = true;
        //                        _bookingRepository.UpdateBooking(booking);
        //                        Console.WriteLine($"Guest {booking.Guest.FirstName} {booking.Guest.LastName} successfully checked in without payment.");
        //                    }
        //                }
        //                break;

        //            case 2: // Check-Out
        //                if (!booking.IsCheckedIn)
        //                {
        //                    Console.WriteLine($"Booking ID {bookingId} has not checked in yet. You cannot check out before check-in.");
        //                }
        //                else if (booking.IsCheckedOut)
        //                {
        //                    Console.WriteLine($"Booking ID {bookingId} is already checked out.");
        //                }
        //                else
        //                {
        //                    // Process checkout and ensure payment
        //                    var invoice = _bookingRepository.GetInvoiceByBookingId(booking.BookingId)
        //                                          ?? _bookingRepository.GenerateInvoiceForBooking(booking);

        //                    Console.WriteLine($"Invoice Total: {invoice.TotalAmount:C}");

        //                    decimal paymentAmount = 0;
        //                    bool validPayment = false;

        //                    // Loop until a valid payment amount is provided
        //                    while (!validPayment)
        //                    {
        //                        Console.Write("Enter payment amount: ");
        //                        if (decimal.TryParse(Console.ReadLine(), out paymentAmount) && paymentAmount >= invoice.TotalAmount)
        //                        {
        //                            validPayment = true; // Payment is valid
        //                        }
        //                        else
        //                        {
        //                            Console.WriteLine("[red]Invalid or insufficient amount. Please try again.[/]");
        //                        }
        //                    }

        //                    _bookingRepository.ProcessPayment(invoice, paymentAmount);

        //                    // Update the booking and room status
        //                    booking.IsCheckedOut = true;
        //                    booking.CheckOutDate = DateTime.Now;
        //                    booking.BookingStatus = true; 

        //                    _bookingRepository.UpdateBooking(booking);

        //                    var room = _roomRepository.GetRoomById(booking.RoomId);
        //                    if (room != null)
        //                    {
        //                        room.IsAvailable = true;  
        //                        _roomRepository.UpdateRoom(room);
        //                    }

        //                    Console.WriteLine("Guest successfully checked out and payment processed.");
        //                }
        //                break;
        //        }

        //        Console.WriteLine("Press any key to return...");
        //        Console.ReadKey();
        //    }
        //}






    }
}
