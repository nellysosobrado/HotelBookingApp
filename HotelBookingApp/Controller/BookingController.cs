using HotelBookingApp.Repositories;
using System;

namespace HotelBookingApp
{
    public class BookingController
    {
        private readonly BookingRepository _bookingRepository;

        public BookingController(BookingRepository bookingRepository)
        {
            _bookingRepository = bookingRepository;
        }

        
        public void SearchAvailableRooms()
        {
            Console.Clear();
            Console.WriteLine("Function: Find available room");

            Console.WriteLine("Enter start date (yyyy-MM-dd):");
            if (!DateTime.TryParse(Console.ReadLine(), out DateTime startDate))
            {
                Console.WriteLine("Invalid date format.");
                return;
            }

            Console.WriteLine("Enter end date (yyyy-MM-dd):");
            if (!DateTime.TryParse(Console.ReadLine(), out DateTime endDate))
            {
                Console.WriteLine("Invalid date format.");
                return;
            }

            Console.WriteLine("Enter the number of guests:");
            if (!int.TryParse(Console.ReadLine(), out int guestCount))
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
                Console.WriteLine("4. Return to Main Menu");
                Console.WriteLine(new string('-', 40));

                Console.Write("Enter your choice (1-4): ");
                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        DisplayActiveBookings();
                        break;
                    case "2":
                        DisplayPreviousGuestHistory();
                        break;
                    case "3":
                        DisplayAllRegisteredGuests();
                        break;
                    case "4":
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
            Console.Clear();
            Console.WriteLine("ACTIVE BOOKINGS (Checked In and Upcoming Guests)");
            Console.WriteLine(new string('-', 60));

            var activeBookings = _bookingRepository.GetAllBookings()
                .Where(b => !b.IsCheckedOut) // Alla bokningar som inte är utcheckade
                .ToList();

            if (!activeBookings.Any())
            {
                Console.WriteLine("No active or upcoming bookings found.");
            }
            else
            {
                foreach (var booking in activeBookings)
                {
                    // Bestäm status: Checked In eller Upcoming
                    string status = booking.IsCheckedIn ? "Checked In" : "Not Checked In";

                    // Hämta pris från senaste fakturan om det finns en
                    var latestInvoice = booking.Invoices?.OrderByDescending(i => i.PaymentDeadline).FirstOrDefault();
                    string invoiceAmount = latestInvoice != null ? $"{latestInvoice.TotalAmount:C}" : "No Invoice";
                    string paymentStatus = latestInvoice != null
                        ? (latestInvoice.IsPaid ? "Paid" : "Not Paid")
                        : "No Invoice";

                    Console.WriteLine($"Guest: {booking.Guest.FirstName} {booking.Guest.LastName}");
                    Console.WriteLine($"Booking ID: {booking.BookingId}\tRoom: {booking.RoomId}");
                    Console.WriteLine($"Status: {status}\tCheck-In Date: {booking.CheckInDate:yyyy-MM-dd}\tCheck-Out Date: {booking.CheckOutDate:yyyy-MM-dd}");
                    Console.WriteLine($"Invoice Amount: {invoiceAmount}\tPayment Status: {paymentStatus}");
                    Console.WriteLine(new string('-', 60));
                }
            }

            Console.WriteLine("\nPress any key to return...");
            Console.ReadKey();
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
            Console.WriteLine("HISTORY OF PREVIOUS GUESTS (Checked Out)");
            Console.WriteLine(new string('-', 60));

            var previousBookings = _bookingRepository.GetAllBookings()
                .Where(b => b.IsCheckedOut).ToList();

            if (!previousBookings.Any())
            {
                Console.WriteLine("No previous guests found.");
            }
            else
            {
                foreach (var booking in previousBookings)
                {
                    Console.WriteLine($"Guest: {booking.Guest.FirstName} {booking.Guest.LastName}");
                    Console.WriteLine($"Booking ID: {booking.BookingId}\tRoom: {booking.RoomId}");
                    Console.WriteLine($"Checked Out On: {booking.CheckOutDate:yyyy-MM-dd}");
                    Console.WriteLine(new string('-', 60));
                }
            }

            Console.WriteLine("\nPress any key to return...");
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
                Console.WriteLine("No registered guests found.");
            }
            else
            {
                foreach (var guest in guests)
                {
                    Console.WriteLine($"Guest ID: {guest.GuestId}");
                    Console.WriteLine($"Name: {guest.FirstName} {guest.LastName}");
                    Console.WriteLine($"Email: {guest.Email}\tPhone: {guest.PhoneNumber}");
                    Console.WriteLine(new string('-', 60));
                }
            }

            Console.WriteLine("\nPress any key to return...");
            Console.ReadKey();
        }

        public void EditBooking()
        {
            Console.Clear();
            Console.WriteLine("EDIT BOOKING");

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

            Console.WriteLine("Enter new Room ID (leave blank to keep current):");
            var newRoomIdInput = Console.ReadLine();
            if (int.TryParse(newRoomIdInput, out int newRoomId))
            {
                booking.RoomId = newRoomId;
            }

            _bookingRepository.UpdateBooking(booking);

            Console.WriteLine($"Booking {bookingId} updated successfully. Press any key to return...");
            Console.ReadKey();
        }


        public void CancelBooking()
        {
            Console.Clear();
            Console.WriteLine("CANCEL BOOKING");

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

            _bookingRepository.DeleteBooking(booking);

            Console.WriteLine($"Booking {bookingId} cancelled successfully. Press any key to return...");
            Console.ReadKey();
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

                Console.Write("Enter Booking ID to check in (or 'Exit'): ");
                string input = Console.ReadLine()?.Trim();
                if (input?.ToLower() == "exit") break;

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
            Console.Write("Enter Guest ID to check out: ");
            if (!int.TryParse(Console.ReadLine(), out int guestId))
            {
                Console.WriteLine("Invalid Guest ID.");
                return;
            }

            var booking = _bookingRepository.GetActiveBookingByGuestId(guestId);
            if (booking == null)
            {
                Console.WriteLine("No active booking found.");
                return;
            }

            var invoice = _bookingRepository.GetInvoiceByBookingId(booking.BookingId)
                           ?? _bookingRepository.GenerateInvoiceForBooking(booking);

            Console.WriteLine($"Invoice Total: {invoice.TotalAmount:C}");
            Console.Write("Enter payment amount: ");
            if (!decimal.TryParse(Console.ReadLine(), out decimal paymentAmount) || paymentAmount < invoice.TotalAmount)
            {
                Console.WriteLine("Invalid or insufficient amount.");
                return;
            }

            _bookingRepository.ProcessPayment(invoice, paymentAmount);

            booking.IsCheckedOut = true;
            booking.BookingStatus = true;
            booking.CheckOutDate = DateTime.Now;

            _bookingRepository.UpdateBooking(booking);

            Console.WriteLine("Guest successfully checked out and payment processed.");
            Console.ReadKey();
        }

        public void PayInvoiceBeforeCheckout()
        {
            Console.Clear();
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



    }
}
