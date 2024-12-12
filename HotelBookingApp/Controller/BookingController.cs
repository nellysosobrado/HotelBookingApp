using Microsoft.EntityFrameworkCore;
using System;

namespace HotelBookingApp;

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

            Console.WriteLine("Enter 'Exit' to go back to main menu");
            Console.Write("Enter Booking ID to check in: ");

            string input = Console.ReadLine()?.Trim();

            if (input?.ToLower() == "exit")
            {
                break;
            }

            if (int.TryParse(input, out int checkInId))
            {
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
                Console.WriteLine($"\nGuest {guest?.FirstName + " " + guest?.LastName} has been successfully checked in.");
                Console.WriteLine(new string('-', 60));
                Console.WriteLine($"{"Booking ID",-15}{"Guest",-20}{"Room ID",-10}{"Check-In Date",-15}");
                Console.WriteLine(new string('-', 60));
                Console.WriteLine($"{booking.BookingId,-15}{guest?.FirstName + " " + guest?.LastName,-20}{room?.RoomId,-10}{booking.CheckInDate,-15:yyyy-MM-dd HH:mm}");
                Console.WriteLine(new string('-', 60));

                Console.WriteLine("\nPress any key to continue...");
                Console.ReadKey();
            }
            else if (input == string.Empty)
            {
                Console.WriteLine("Input cannot be empty. Press any key to try again...");
                Console.ReadKey();
            }
            else
            {
                Console.WriteLine("Invalid Booking ID. Press any key to try again...");
                Console.ReadKey();
            }
        }

    }
    public void CheckOut()
    {
        Console.Clear();
        Console.WriteLine("Page: Checkout()");
        Console.WriteLine("Enter Guest ID to check out:");

        if (int.TryParse(Console.ReadLine(), out int guestId))
        {
            var booking = _bookingRepository.GetActiveBookingByGuestId(guestId);

            if (booking == null)
            {
                Console.WriteLine("No active booking found for this guest.");
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

            Console.WriteLine("\nEnter payment amount:");
            if (decimal.TryParse(Console.ReadLine(), out decimal paymentAmount))
            {
                if (paymentAmount < invoice.TotalAmount)
                {
                    Console.WriteLine("Insufficient payment. The guest must pay the full amount.");
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
                Console.ReadKey();
            }
            else
            {
                Console.WriteLine("Invalid payment amount.");
            }
        }
        else
        {
            Console.WriteLine("Invalid Guest ID.");
        }
    }

}
