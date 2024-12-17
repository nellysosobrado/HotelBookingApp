using HotelBookingApp.Data;
using HotelBookingApp.Entities;
using HotelBookingApp.Repositories;
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

            _guestRepository.AddGuest(newGuest);

            Console.WriteLine("\nGuest registered successfully!");

            // Fråga om användaren vill skapa en bokning
            Console.Write("\nDo you want to create a booking for this guest now? (Y/N): ");
            var choice = Console.ReadLine()?.Trim().ToUpper();

            if (choice == "Y")
            {
                // Hämta antal personer och datum
                int guestCount = PromptForInt("Enter number of guests: ");
                DateTime startDate = PromptForDate("Enter start date (yyyy-MM-dd): ");
                DateTime endDate = PromptForDate("Enter end date (yyyy-MM-dd): ");

                // Hämta tillgängliga rum
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

                // Välj rum
                Console.Write("Enter Room ID to book: ");
                if (!int.TryParse(Console.ReadLine(), out int roomId) || !availableRooms.Any(r => r.RoomId == roomId))
                {
                    Console.WriteLine("Invalid Room ID. Booking not created.");
                    Console.ReadKey(true);
                    return;
                }

                // Skapa bokning
                var newBooking = new Booking
                {
                    GuestId = newGuest.GuestId,
                    RoomId = roomId,
                    CheckInDate = startDate,
                    CheckOutDate = endDate,
                    IsCheckedIn = false,
                    IsCheckedOut = false,
                    BookingStatus = false
                };

                _guestRepository.AddBooking(newBooking);

                Console.WriteLine("\nBooking created successfully!");
            }

            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey(true);
        }
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
