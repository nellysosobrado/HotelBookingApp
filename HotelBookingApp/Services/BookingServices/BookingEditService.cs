using HotelBookingApp.Entities;
using HotelBookingApp.Repositories;
using Spectre.Console;
using System;
using System.Linq;

namespace HotelBookingApp.Services.BookingServices
{
    public class BookingEditService
    {
        private readonly BookingRepository _bookingRepository;
        private readonly RoomRepository _roomRepository;
        private readonly GuestBookings _guestBookings;

        public BookingEditService(BookingRepository bookingRepository, RoomRepository roomRepository, GuestBookings guestBookings)
        {
            _bookingRepository = bookingRepository;
            _roomRepository = roomRepository;
            _guestBookings = guestBookings;
        }

        public void EditBooking()
        {
            Console.Clear();

            var editableBookings = _bookingRepository.GetEditableBookings().ToList();

            if (!editableBookings.Any())
            {
                AnsiConsole.Markup("[red]No editable bookings available. Press any key to return...[/]\n");
                Console.ReadKey();
                return;
            }

            DisplayBookings(editableBookings);

            AnsiConsole.Markup("Enter a [green]Booking ID[/] you wish to edit:\n");
            if (!int.TryParse(Console.ReadLine(), out int bookingId))
            {
                AnsiConsole.Markup("[red]Invalid Booking ID. Press any key to return...[/]\n");
                Console.ReadKey();
                return;
            }

            var booking = editableBookings.FirstOrDefault(b => b.BookingId == bookingId);
            if (booking == null)
            {
                AnsiConsole.Markup("[red]Booking not found or not editable. Press any key to return...[/]\n");
                Console.ReadKey();
                return;
            }

            EditBookingDetails(booking);
        }

        private void DisplayBookings(IEnumerable<Entities.Booking> bookings)
        {
            var table = new Table()
                .Border(TableBorder.Rounded)
                .AddColumn("[bold yellow]Booking ID[/]")
                .AddColumn("[bold yellow]Guest Name[/]")
                .AddColumn("[bold yellow]Room ID[/]");

            foreach (var booking in bookings)
            {
                table.AddRow(booking.BookingId.ToString(), $"{booking.Guest.FirstName} {booking.Guest.LastName}", booking.RoomId.ToString());
            }

            AnsiConsole.Write(table);
        }

        private void EditBookingDetails(Entities.Booking booking)
        {
            var invoice = _bookingRepository.GetInvoiceByBookingId(booking.BookingId);

            while (true)
            {
                Console.Clear();
                AnsiConsole.Markup($"[bold green]Editing Booking ID {booking.BookingId}[/]\n");
                DisplayBookingDetails(booking, invoice);

                var action = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("[bold green]What would you like to edit?[/]")
                        .AddChoices("Edit Guest Information", "Edit Room Information", "Edit Check-In Date", "Edit Check-Out Date", "Confirm Edit", "Cancel")
                        .HighlightStyle(new Style(foreground: Color.Green))
                );

                if (action == "Cancel")
                    return;

                HandleEditAction(booking, invoice, action);
            }
        }

        private void HandleEditAction(Entities.Booking booking, Invoice invoice, string action)
        {
            switch (action)
            {
                case "Edit Guest Information":
                    EditGuestInformation(booking);
                    break;
                case "Edit Room Information":
                    var room = _roomRepository.GetRoomById(booking.RoomId);
                    if (room != null)
                        EditRoomDetails(room, booking);
                    else
                        AnsiConsole.MarkupLine("[red]Room not found for this booking.[/]");
                    break;
                case "Edit Check-In Date":
                    EditCheckInDate(booking, invoice);
                    break;
                case "Edit Check-Out Date":
                    EditCheckOutDate(booking, invoice);
                    break;
                case "Confirm Edit":
                    ConfirmBookingEdit(booking);
                    return;
            }
        }

        private void EditGuestInformation(Entities.Booking booking)
        {
            var guest = booking.Guest;

            while (true)
            {
                Console.Clear();
                AnsiConsole.MarkupLine("[bold yellow]Editing Guest Information[/]\n");

                var guestInfoTable = new Table()
                    .Border(TableBorder.Rounded)
                    .AddColumn("[bold yellow]Field[/]")
                    .AddColumn("[bold yellow]Value[/]")
                    .AddRow("First Name", guest.FirstName)
                    .AddRow("Last Name", guest.LastName)
                    .AddRow("Email", guest.Email)
                    .AddRow("Phone Number", guest.PhoneNumber);

                AnsiConsole.Write(guestInfoTable);

                var fieldToEdit = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("[bold green]Select a field to edit or confirm changes:[/]")
                        .AddChoices(new[] { "First Name", "Last Name", "Email", "Phone Number", "Save updates", "Back" })
                        .HighlightStyle(new Style(foreground: Color.Green))
                );

                switch (fieldToEdit)
                {
                    case "First Name":
                        guest.FirstName = AnsiConsole.Ask<string>("Enter [green]new First Name[/]:");
                        break;
                    case "Last Name":
                        guest.LastName = AnsiConsole.Ask<string>("Enter [green]new Last Name[/]:");
                        break;
                    case "Email":
                        guest.Email = AnsiConsole.Ask<string>("Enter [green]new Email[/]:");
                        break;
                    case "Phone Number":
                        guest.PhoneNumber = AnsiConsole.Ask<string>("Enter [green]new Phone Number[/]:");
                        break;
                    case "Save updates":
                        SaveGuestUpdates(guest);
                        return;
                    case "Back":
                        return;
                }
            }
        }

        private void SaveGuestUpdates(Guest guest)
        {
            try
            {
                _bookingRepository.UpdateGuest(guest);
                AnsiConsole.MarkupLine("[green]Guest information updated successfully![/]");
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]Error updating guest: {ex.Message}[/]");
            }
            Console.ReadKey();
        }

        public void EditRoomDetails(Room room, Entities.Booking booking)
        {
            while (true)
            {
                Console.Clear();
                AnsiConsole.Markup($"[bold yellow]Editing Room ID {room.RoomId}[/]\n");

                room = _roomRepository.GetRoomById(room.RoomId);

                DisplayRoomDetailsTable(room);

                var action = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("[bold green]What would you like to do?[/]")
                        .AddChoices(new[] {
                    "Unbook room, and select new room by date",
                    "Unbook room, and select new room by searching total people",
                    "Finish Editing"
                        })
                        .HighlightStyle(new Style(foreground: Color.Green))
                );

                switch (action)
                {
                    case "Unbook room, and select new room by date":
                        HandleRoomSelectionByDate(room, booking);
                        break;

                    case "Unbook room, and select new room by searching total people":
                        HandleRoomSelectionByCapacity(room, booking);
                        break;

                    case "Finish Editing":
                        AnsiConsole.Markup("[green]Finished editing room details.[/]\n");
                        return;

                    default:
                        AnsiConsole.Markup("[red]Invalid option selected. Please try again.[/]\n");
                        break;
                }

                Console.WriteLine("\nPress any key to continue...");
                Console.ReadKey();
            }
        }


        private void DisplayRoomDetailsTable(Room room)
        {
            var roomDetailsTable = new Table()
                .Border(TableBorder.Rounded)
                .AddColumn("[bold]Property[/]")
                .AddColumn("[bold]Value[/]")
                .AddRow("Type", room.Type)
                .AddRow("Price Per Night", room.PricePerNight.ToString("C"))
                .AddRow("Size (sqm)", room.SizeInSquareMeters.ToString())
                .AddRow("Extra Beds", room.ExtraBeds.ToString())
                .AddRow("Total People Capacity", room.TotalPeople.ToString());
            AnsiConsole.Write(roomDetailsTable);
        }


        private void HandleRoomSelectionByDate(Room room, Entities.Booking booking)
        {
            string roomTypeByDate = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[yellow]Please select new room type:[/]")
                    .AddChoices("Single", "Double")
            );

            DateTime startDateByDate = _guestBookings.SelectDateWithCalendar("[yellow]Select check-in date:[/]", roomTypeByDate);
            if (startDateByDate == DateTime.MinValue)
            {
                AnsiConsole.MarkupLine("[red]Date selection canceled.[/] Returning to menu...");
                return;
            }

            DateTime endDateByDate = _guestBookings.SelectDateWithCalendar("[yellow]Select check-out date:[/]", roomTypeByDate);
            if (endDateByDate == DateTime.MinValue)
            {
                AnsiConsole.MarkupLine("[red]Date selection canceled.[/] Returning to menu...");
                return;
            }

            if (endDateByDate < startDateByDate)
            {
                AnsiConsole.MarkupLine("[red]Check-out date cannot be before check-in date![/]");
                return;
            }

            if (endDateByDate == startDateByDate)
            {
                endDateByDate = endDateByDate.AddDays(1).AddSeconds(-1); 
            }

            var availableRoomsByDate = _roomRepository.GetAvailableRoomsByDate(startDateByDate, endDateByDate, roomTypeByDate);
            if (!availableRoomsByDate.Any())
            {
                AnsiConsole.MarkupLine("[red]No available rooms found for the selected dates and room type.[/]");
                return;
            }

            DisplayAvailableRooms(availableRoomsByDate);

            int newRoomIdByDate = AnsiConsole.Prompt(
                new TextPrompt<int>("[yellow]Enter Room ID to select:[/]")
                    .ValidationErrorMessage("[red]Invalid Room ID![/]")
                    .Validate(input => availableRoomsByDate.Any(r => r.RoomId == input))
            );

            UpdateRoomAndBooking(room, booking, newRoomIdByDate, availableRoomsByDate);

            booking.CheckInDate = startDateByDate;
            booking.CheckOutDate = endDateByDate;

            _bookingRepository.UpdateBooking(booking);

            AnsiConsole.MarkupLine("[green]Booking updated successfully![/]");
        }



        private void HandleRoomSelectionByCapacity(Room room, Entities.Booking booking)
        {
            try
            {
                int totalPeople = AnsiConsole.Ask<int>("[yellow]Enter the total number of people:[/]");

                var roomsByCapacity = _roomRepository.GetRoomsByCapacity(totalPeople);
                if (!roomsByCapacity.Any())
                {
                    AnsiConsole.MarkupLine("[red]No rooms available for the selected capacity.[/]");
                    return;
                }

                DisplayAvailableRooms(roomsByCapacity);

                int selectedRoomId = AnsiConsole.Prompt(
                    new TextPrompt<int>("[yellow]Enter Room ID to view availability:[/]")
                        .ValidationErrorMessage("[red]Invalid Room ID![/]")
                        .Validate(input => roomsByCapacity.Any(r => r.RoomId == input))
                );

                var selectedRoom = roomsByCapacity.First(r => r.RoomId == selectedRoomId);

                DateTime startDateByCapacity = _guestBookings.SelectDateWithCalendar("[yellow]Select check-in date:[/]", selectedRoom.Type);
                if (startDateByCapacity == DateTime.MinValue)
                {
                    AnsiConsole.MarkupLine("[red]Date selection canceled.[/] Returning to menu...");
                    return;
                }

                DateTime endDateByCapacity = _guestBookings.SelectDateWithCalendar("[yellow]Select check-out date:[/]", selectedRoom.Type);
                if (endDateByCapacity == DateTime.MinValue || endDateByCapacity <= startDateByCapacity)
                {
                    AnsiConsole.MarkupLine("[red]Check-out date must be after check-in date![/]");
                    return;
                }

                if (!_roomRepository.IsRoomAvailable(selectedRoomId, startDateByCapacity, endDateByCapacity))
                {
                    AnsiConsole.MarkupLine("[red]The selected room is not available for the chosen dates.[/]");
                    return;
                }

                UpdateRoomAndBooking(room, booking, selectedRoomId, roomsByCapacity);

                booking.CheckInDate = startDateByCapacity;
                booking.CheckOutDate = endDateByCapacity;

                _bookingRepository.UpdateBooking(booking);

                AnsiConsole.MarkupLine("[green]Room and booking updated successfully![/]");
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]An error occurred while updating the booking: {ex.Message}[/]");
            }
        }

        private void UpdateRoomAndBooking(Room currentRoom, Entities.Booking booking, int newRoomId, IEnumerable<Room> availableRooms)
        {
            try
            {
                var newRoom = availableRooms.First(r => r.RoomId == newRoomId);

                if (currentRoom != null)
                {
                    currentRoom.IsAvailable = true;
                    _roomRepository.UpdateRoom(currentRoom);
                }

                newRoom.IsAvailable = false;
                _roomRepository.UpdateRoom(newRoom);

                booking.RoomId = newRoomId;
                _bookingRepository.UpdateBooking(booking);

                AnsiConsole.MarkupLine("[green]Room and booking updated successfully![/]");
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]Error updating room or booking: {ex.Message}[/]");
            }
        }


        private void DisplayAvailableRooms(IEnumerable<Room> rooms)
        {
            var table = new Table()
                .Border(TableBorder.Rounded)
                .AddColumn("[bold]Room ID[/]")
                .AddColumn("[bold]Type[/]")
                .AddColumn("[bold]Price Per Night[/]")
                .AddColumn("[bold]Size (sqm)[/]")
                .AddColumn("[bold]Total People Capacity[/]");

            foreach (var room in rooms)
            {
                table.AddRow(
                    room.RoomId.ToString(),
                    room.Type,
                    room.PricePerNight.ToString("C"),
                    room.SizeInSquareMeters.ToString(),
                    room.TotalPeople.ToString()
                );
            }

            AnsiConsole.Write(table);
        }

      
        private void EditCheckInDate(Entities.Booking booking, Invoice invoice)
        {
            var newCheckInDate = AnsiConsole.Ask<DateTime>("Enter [green]new Check-In Date (yyyy-MM-dd)[/]:");
            if (newCheckInDate >= DateTime.Now.Date)
            {
                booking.CheckInDate = newCheckInDate;
                UpdateInvoice(booking, invoice);
                AnsiConsole.MarkupLine("[green]Check-In Date updated successfully![/]");
            }
            else
            {
                AnsiConsole.MarkupLine("[red]Invalid Check-In Date.[/]");
            }
            Console.ReadKey();
        }

        private void EditCheckOutDate(Entities.Booking booking, Invoice invoice)
        {
            var newCheckOutDate = AnsiConsole.Ask<DateTime>("Enter [green]new Check-Out Date (yyyy-MM-dd)[/]:");
            if (newCheckOutDate > booking.CheckInDate)
            {
                booking.CheckOutDate = newCheckOutDate;
                UpdateInvoice(booking, invoice);
                AnsiConsole.MarkupLine("[green]Check-Out Date updated successfully![/]");
            }
            else
            {
                AnsiConsole.MarkupLine("[red]Invalid Check-Out Date.[/]");
            }
            Console.ReadKey();
        }

        private void UpdateInvoice(Entities.Booking booking, Invoice invoice)
        {
            invoice.TotalAmount = _bookingRepository.CalculateTotalAmount(booking);
            _bookingRepository.UpdateInvoice(invoice);
        }

        private void ConfirmBookingEdit(Entities.Booking booking)
        {
            try
            {
                _bookingRepository.UpdateBooking(booking);
                AnsiConsole.MarkupLine("[green]Booking updated successfully![/]");
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]Error updating booking: {ex.Message}[/]");
            }
            Console.ReadKey();
        }


        private void DisplayBookingDetails(Entities.Booking booking, Invoice invoice)
        {
            var room = _roomRepository.GetRoomById(booking.RoomId); 
            var bookingDetailsTable = new Table()
                .Border(TableBorder.Rounded)
                .AddColumn("[bold yellow]Detail[/]")
                .AddColumn("[bold yellow]Information[/]")
                .AddRow("Guest", $"{booking.Guest.FirstName} {booking.Guest.LastName}")
                .AddRow("Email", booking.Guest.Email)
                .AddRow("Phone Number", booking.Guest.PhoneNumber)
                .AddRow("Room ID", booking.RoomId.ToString())
                .AddRow("Room Type", room?.Type ?? "[red]N/A[/]")
                .AddRow("Check-In Date", booking.CheckInDate?.ToString("yyyy-MM-dd") ?? "[grey]Not Set[/]")
                .AddRow("Check-Out Date", booking.CheckOutDate?.ToString("yyyy-MM-dd") ?? "[grey]Not Set[/]")
                .AddRow("Amount", $"{invoice.TotalAmount:C}")
                .AddRow("Booking Status", booking.IsCheckedIn ? "[green]Checked In[/]" : "[yellow]Not Checked In[/]");

            AnsiConsole.Write(bookingDetailsTable);
        }


    }
}
