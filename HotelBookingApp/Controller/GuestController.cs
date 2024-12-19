﻿using HotelBookingApp.Data;
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

        public GuestController(AppDbContext context)
        {
            _guestRepository = new GuestRepository(context);
        }
        public void RegisterNewGuest()
        {
            Console.Clear();
            Console.WriteLine("REGISTER NEW GUEST");

            var firstName = PromptInput("Enter First Name: ");
            var lastName = PromptInput("Enter Last Name: ");
            var email = PromptInput("Enter Email: ");
            var phone = PromptInput("Enter Phone Number: ");

            if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName) ||
                string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(phone))
            {
                Console.WriteLine("All fields are required. Registration failed.");
                Console.ReadKey(true);
                return;
            }

            var newGuest = new Guest
            {
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                PhoneNumber = phone
            };

            Console.Write("\nDo you want to create a booking for this guest now? (Y/N): ");
            var choice = Console.ReadLine()?.Trim().ToUpper();

            Booking newBooking = null;
            Invoice newInvoice = null;

            if (choice == "Y")
            {
                int guestCount = PromptForInt("Enter number of guests: ");
                DateTime startDate = SelectDate("Select Start Date:");
                DateTime endDate = SelectDate("Select End Date:");

                if (endDate <= startDate)
                {
                    Console.WriteLine("End date must be after start date. Booking not created.");
                    Console.ReadKey(true);
                    return;
                }

                var availableRooms = _guestRepository.GetAvailableRooms(startDate, endDate, guestCount);

                if (!availableRooms.Any())
                {
                    Console.WriteLine("No available rooms found for the selected dates and number of guests.");
                    Console.ReadKey(true);
                    return;
                }

                Console.WriteLine("\nAvailable Rooms:");
                foreach (var room in availableRooms)
                {
                    string extraBedInfo = room.Type == "Double" ? $"Extra Beds: {room.ExtraBeds}" : "No extra beds";
                    Console.WriteLine($"Room ID: {room.RoomId} | Type: {room.Type} | Price: {room.PricePerNight:C} | {extraBedInfo}");
                }

                Console.Write("Enter Room ID to book: ");
                if (!int.TryParse(Console.ReadLine(), out int roomId) || !availableRooms.Any(r => r.RoomId == roomId))
                {
                    Console.WriteLine("Invalid Room ID. Booking not created.");
                    Console.ReadKey(true);
                    return;
                }

                newBooking = new Booking
                {
                    RoomId = roomId,
                    CheckInDate = startDate,
                    CheckOutDate = endDate,
                    IsCheckedIn = false,
                    IsCheckedOut = false,
                    BookingStatus = false
                };

                decimal totalAmount = _guestRepository.CalculateTotalAmount(newBooking);

                newInvoice = new Invoice
                {
                    TotalAmount = totalAmount,
                    IsPaid = false,
                    PaymentDeadline = endDate.AddDays(7)
                };
            }

            _guestRepository.RegisterNewGuestWithBooking(newGuest, newBooking, newInvoice);

            Console.WriteLine("\nGuest, booking, and invoice registered successfully!");
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey(true);
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
                        AnsiConsole.MarkupLine("[red]The date cannot be in the past.[/]");
                        Console.ReadKey(true);
                        break;
                    case ConsoleKey.Escape:
                        return DateTime.MinValue; // Returnera ogiltigt datum vid avbryt
                }
            }
        }

        private void RenderCalendar(DateTime selectedDate)
        {
            var calendarContent = new StringWriter();

            // Kalenderhuvud
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


        //public void RegisterNewGuest()
        //{
        //    Console.Clear();
        //    Console.WriteLine("REGISTER NEW GUEST");

        //    var firstName = PromptInput("Enter First Name: ");
        //    var lastName = PromptInput("Enter Last Name: ");
        //    var email = PromptInput("Enter Email: ");
        //    var phone = PromptInput("Enter Phone Number: ");

        //    if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName) ||
        //        string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(phone))
        //    {
        //        Console.WriteLine("All fields are required. Registration failed.");
        //        Console.ReadKey(true);
        //        return;
        //    }

        //    var newGuest = new Guest
        //    {
        //        FirstName = firstName,
        //        LastName = lastName,
        //        Email = email,
        //        PhoneNumber = phone
        //    };

        //    Console.Write("\nDo you want to create a booking for this guest now? (Y/N): ");
        //    var choice = Console.ReadLine()?.Trim().ToUpper();

        //    Booking newBooking = null;
        //    Invoice newInvoice = null;

        //    if (choice == "Y")
        //    {
        //        int guestCount = PromptForInt("Enter number of guests: ");
        //        DateTime startDate = PromptForDate("Enter start date (yyyy-MM-dd): ");
        //        DateTime endDate = PromptForDate("Enter end date (yyyy-MM-dd): ");

        //        var availableRooms = _guestRepository.GetAvailableRooms(startDate, endDate, guestCount);

        //        if (!availableRooms.Any())
        //        {
        //            Console.WriteLine("No available rooms found for the selected dates and number of guests.");
        //            Console.ReadKey(true);
        //            return;
        //        }

        //        Console.WriteLine("\nAvailable Rooms:");
        //        foreach (var room in availableRooms)
        //        {
        //            string extraBedInfo = room.Type == "Double" ? $"Extra Beds: {room.ExtraBeds}" : "No extra beds";
        //            Console.WriteLine($"Room ID: {room.RoomId} | Type: {room.Type} | Price: {room.PricePerNight:C} | {extraBedInfo}");
        //        }

        //        Console.Write("Enter Room ID to book: ");
        //        if (!int.TryParse(Console.ReadLine(), out int roomId) || !availableRooms.Any(r => r.RoomId == roomId))
        //        {
        //            Console.WriteLine("Invalid Room ID. Booking not created.");
        //            Console.ReadKey(true);
        //            return;
        //        }

        //        newBooking = new Booking
        //        {
        //            RoomId = roomId,
        //            CheckInDate = startDate,
        //            CheckOutDate = endDate,
        //            IsCheckedIn = false,
        //            IsCheckedOut = false,
        //            BookingStatus = false
        //        };

        //        decimal totalAmount = _guestRepository.CalculateTotalAmount(newBooking);

        //        newInvoice = new Invoice
        //        {
        //            TotalAmount = totalAmount,
        //            IsPaid = false,
        //            PaymentDeadline = endDate.AddDays(7)
        //        };
        //    }

        //    _guestRepository.RegisterNewGuestWithBooking(newGuest, newBooking, newInvoice);

        //    Console.WriteLine("\nGuest, booking, and invoice registered successfully!");
        //    Console.WriteLine("\nPress any key to continue...");
        //    Console.ReadKey(true);
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
            Console.WriteLine("ALL GUEST");

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

                var roomInfo = bookings.Any()
                    ? string.Join(", ", bookings.Select(b => $"Room {b.RoomId}"))
                    : "No bookings";

                Console.WriteLine($"ID: {guest.GuestId} | Name: {guest.FirstName} {guest.LastName} | Email: {guest.Email} | Phone: {guest.PhoneNumber} | Rooms: {roomInfo}");
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
        public void RemoveGuest()
        {
            Console.Clear();
            Console.WriteLine("REMOVE GUEST");

            var guests = _guestRepository.GetAllGuests();

            if (!guests.Any())
            {
                Console.WriteLine("No guests available to remove.");
                Console.ReadKey(true);
                return;
            }

            Console.WriteLine("\nSelect a guest to remove by ID:");
            foreach (var guest in guests)
            {
                Console.WriteLine($"ID: {guest.GuestId} | Name: {guest.FirstName} {guest.LastName}");
            }

            var guestId = PromptForInt("Enter Guest ID to remove: ");
            var selectedGuest = _guestRepository.GetGuestById(guestId);

            if (selectedGuest == null)
            {
                Console.WriteLine("Guest not found. Returning to menu...");
                Console.ReadKey(true);
                return;
            }

            Console.WriteLine($"\nAre you sure you want to remove guest '{selectedGuest.FirstName} {selectedGuest.LastName}'? (Y/N)");
            var confirmation = Console.ReadLine()?.Trim().ToUpper();

            if (confirmation == "Y")
            {
                _guestRepository.RemoveGuest(guestId);
                Console.WriteLine($"Guest '{selectedGuest.FirstName} {selectedGuest.LastName}' successfully removed.");
            }
            else
            {
                Console.WriteLine("Operation cancelled.");
            }

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey(true);
        }


    }
}
