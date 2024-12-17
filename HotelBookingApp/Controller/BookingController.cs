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

        public void CheckIn()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("CHECK IN GUEST");

                Console.WriteLine("Enter 'Exit' to go back to the main menu");
                Console.Write("Enter Booking ID to check in: ");

                string input = Console.ReadLine()?.Trim();

                if (input?.ToLower() == "exit")
                {
                    break;
                }

                if (!int.TryParse(input, out int checkInId))
                {
                    Console.WriteLine("Invalid Booking ID. Press any key to try again...");
                    Console.ReadKey();
                    continue;
                }

                var booking = _bookingRepository.GetBookingById(checkInId);
                if (booking == null)
                {
                    Console.WriteLine("Booking not found. Press any key to try again...");
                    Console.ReadKey();
                    continue;
                }

                if (booking.IsCheckedIn)
                {
                    Console.WriteLine("Guest is already checked in. Press any key to try again...");
                    Console.ReadKey();
                    continue;
                }

                booking.IsCheckedIn = true;
                booking.CheckInDate = DateTime.Now;
                _bookingRepository.UpdateBooking(booking);

                var guest = _bookingRepository.GetGuestById(booking.GuestId);
                var room = _bookingRepository.GetRoomById(booking.RoomId);

                Console.Clear();
                Console.WriteLine($"\nGuest {guest?.FirstName} {guest?.LastName} has been successfully checked in.");
                Console.WriteLine(new string('-', 60));
                Console.WriteLine($"{"Booking ID",-15}{"Guest",-20}{"Room ID",-10}{"Check-In Date",-15}");
                Console.WriteLine(new string('-', 60));
                Console.WriteLine($"{booking.BookingId,-15}{guest?.FirstName + " " + guest?.LastName,-20}{room?.RoomId,-10}{booking.CheckInDate,-15:yyyy-MM-dd HH:mm}");
                Console.WriteLine(new string('-', 60));

                Console.WriteLine("\nPress any key to continue...");
                Console.ReadKey();
            }
        }

        public void CheckOut()
        {
            Console.Clear();
            Console.WriteLine("Page: Checkout()");
            Console.Write("Enter Guest ID to check out: ");

            if (!int.TryParse(Console.ReadLine(), out int guestId))
            {
                Console.WriteLine("Invalid Guest ID. Press any key to return...");
                Console.ReadKey();
                return;
            }

            var booking = _bookingRepository.GetActiveBookingByGuestId(guestId);
            if (booking == null)
            {
                Console.WriteLine("No active booking found for this guest. Press any key to return...");
                Console.ReadKey();
                return;
            }

            Console.WriteLine("\nGuest Details:");
            Console.WriteLine($"Booking ID: {booking.BookingId}");
            Console.WriteLine($"Room ID: {booking.RoomId}");
            Console.WriteLine($"Check-In Status: {(booking.IsCheckedIn ? "Checked In" : "Not Checked In")}");
            Console.WriteLine($"Check-Out Status: {(booking.IsCheckedOut ? "Checked Out" : "Not Checked Out")}");

            var invoice = _bookingRepository.GetInvoiceByBookingId(booking.BookingId);

            if (invoice == null)
            {
                Console.WriteLine("\nGenerating invoice...");
                invoice = new Invoice
                {
                    BookingId = booking.BookingId,
                    TotalAmount = _bookingRepository.CalculateTotalAmount(booking),
                    IsPaid = false,
                    PaymentDeadline = DateTime.Now.AddDays(7)
                };

                _bookingRepository.AddInvoice(invoice);
                _bookingRepository.UpdateBookingAndInvoice(booking, invoice);

                Console.WriteLine("Invoice generated successfully.");
            }

            Console.WriteLine($"\nInvoice Details:");
            Console.WriteLine($"Invoice ID: {invoice.InvoiceId}");
            Console.WriteLine($"Total Amount: {invoice.TotalAmount:C}");
            Console.WriteLine($"Payment Deadline: {invoice.PaymentDeadline:yyyy-MM-dd}");

            Console.Write("\nEnter payment amount: ");
            if (!decimal.TryParse(Console.ReadLine(), out decimal paymentAmount) || paymentAmount < invoice.TotalAmount)
            {
                Console.WriteLine("Invalid or insufficient payment amount. Press any key to return...");
                Console.ReadKey();
                return;
            }

            var payment = new Payment
            {
                InvoiceId = invoice.InvoiceId,
                PaymentDate = DateTime.Now,
                AmountPaid = paymentAmount
            };

            _bookingRepository.AddPayment(payment);

            invoice.IsPaid = true;
            booking.IsCheckedIn = false;
            booking.IsCheckedOut = true;
            booking.BookingStatus = true;
            booking.CheckOutDate = DateTime.Now;

            _bookingRepository.UpdateBookingAndInvoice(booking, invoice);

            Console.WriteLine("\nPayment processed successfully.");
            Console.WriteLine($"Booking with Booking ID {booking.BookingId} has been successfully checked out and marked as completed.");
            Console.WriteLine("\nPress any key to return...");
            Console.ReadKey();
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
                    Console.WriteLine($"Status: {status}\tCheck-In Date: {booking.CheckInDate:yyyy-MM-dd}");
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
        public void PayInvoiceBeforeCheckout()
        {
            Console.Clear();
            Console.WriteLine("PAY INVOICE BEFORE CHECKOUT");
            Console.WriteLine(new string('-', 60));

            Console.Write("Enter Booking ID: ");
            if (!int.TryParse(Console.ReadLine(), out int bookingId))
            {
                Console.WriteLine("Invalid Booking ID. Press any key to return...");
                Console.ReadKey();
                return;
            }

            // Hämta bokningen och dess faktura
            var booking = _bookingRepository.GetBookingById(bookingId);
            if (booking == null)
            {
                Console.WriteLine("Booking not found. Press any key to return...");
                Console.ReadKey();
                return;
            }

            var invoice = _bookingRepository.GetInvoiceByBookingId(bookingId);
            if (invoice == null)
            {
                Console.WriteLine("No invoice found for this booking. Press any key to return...");
                Console.ReadKey();
                return;
            }

            if (invoice.IsPaid)
            {
                Console.WriteLine("Invoice is already paid. Press any key to return...");
                Console.ReadKey();
                return;
            }

            // Visa fakturadetaljer
            Console.WriteLine($"Invoice ID: {invoice.InvoiceId}");
            Console.WriteLine($"Total Amount: {invoice.TotalAmount:C}");
            Console.WriteLine($"Payment Deadline: {invoice.PaymentDeadline:yyyy-MM-dd}");
            Console.WriteLine(new string('-', 60));

            // Ange betalningsbelopp
            Console.Write("Enter payment amount: ");
            if (!decimal.TryParse(Console.ReadLine(), out decimal paymentAmount) || paymentAmount < invoice.TotalAmount)
            {
                Console.WriteLine("Invalid or insufficient payment amount. Press any key to return...");
                Console.ReadKey();
                return;
            }

            // Skapa och spara betalning
            var payment = new Payment
            {
                InvoiceId = invoice.InvoiceId,
                PaymentDate = DateTime.Now,
                AmountPaid = paymentAmount
            };

            _bookingRepository.AddPayment(payment);

            // Uppdatera fakturan
            invoice.IsPaid = true;
            _bookingRepository.UpdateInvoice(invoice);

            Console.WriteLine("\nPayment processed successfully.");
            Console.WriteLine($"Invoice {invoice.InvoiceId} marked as Paid.");
            Console.WriteLine("Press any key to return...");
            Console.ReadKey();
        }


    }
}
