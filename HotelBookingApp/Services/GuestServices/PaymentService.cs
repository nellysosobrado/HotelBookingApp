using HotelBookingApp.Repositories;
using HotelBookingApp.Services.BookingServices;
using HotelBookingApp.Services.DisplayServices;
using Spectre.Console;
using System;

namespace HotelBookingApp.Services.GuestServices
{
    public class PaymentService
    {
        private readonly BookingRepository _bookingRepository;
        private readonly TableDisplayService _tableDisplayService;
        private readonly UnpaidBookingService _unpaidBookingService;

        public PaymentService(BookingRepository bookingRepository,
            TableDisplayService tableDisplayService,
            UnpaidBookingService unpaidBookingService)
        {
            _bookingRepository = bookingRepository;
            _tableDisplayService = tableDisplayService;
            _unpaidBookingService = unpaidBookingService;
        }
        public void Start()
        {
            bool exit = false;

            while (!exit)
            {
                Console.Clear();
                AnsiConsole.MarkupLine("[blue bold]Welcome to Payment Management[/]");

                var choice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("[green]Select an option:[/]")
                        .AddChoices("Pay Guest Invoice", "Handle Unpaid Bookings", "Exit"));

                switch (choice)
                {
                    case "Pay Guest Invoice":
                        PayInvoiceBeforeCheckout();
                        break;

                    case "Handle Unpaid Bookings":
                        _unpaidBookingService.HandleUnpaidBookings();
                        break;

                    case "Exit":
                        exit = true;
                        AnsiConsole.MarkupLine("[yellow]Exiting payment service. Goodbye![/]");
                        break;

                    default:
                        AnsiConsole.MarkupLine("[red]Invalid option. Please try again.[/]");
                        break;
                }
            }
        }

        public void Payment()
        {
            PayInvoiceBeforeCheckout();
            _unpaidBookingService.HandleUnpaidBookings();
        }

        public void PayInvoiceBeforeCheckout()
        {
            while (true)
            {
                Console.Clear();
                _tableDisplayService.DisplayActiveBookings();
                Console.Write("Enter Booking ID, to pay the guest invoice (type 'back' to go back): ");
                var input = Console.ReadLine();

                if (input?.ToLower() == "back")
                    return;

                if (!int.TryParse(input, out int bookingId))
                {
                    Console.WriteLine("Invalid Booking ID. Please try again.");
                    Console.ReadKey();
                    continue;
                }

                var invoice = _bookingRepository.GetInvoiceByBookingId(bookingId);
                if (invoice == null || invoice.IsPaid)
                {
                    Console.WriteLine("No unpaid invoice found for this booking.");
                    Console.ReadKey();
                    continue;
                }

                Console.WriteLine($"Invoice Total: {invoice.TotalAmount:C}");

                var confirmPayment = AnsiConsole.Confirm($"Do you want to pay this invoice of {invoice.TotalAmount:C}?");
                if (!confirmPayment)
                {
                    Console.WriteLine("Payment cancelled. Returning to the main menu.");
                    Console.ReadKey();
                    continue;
                }

                _bookingRepository.ProcessPayment(invoice, invoice.TotalAmount);
                Console.WriteLine("Invoice paid successfully.");

                Console.Write("Do you want to pay another invoice? (yes/no): ");
                var choice = Console.ReadLine()?.ToLower();

                if (choice == "no")
                {
                    Console.WriteLine("Returning to the main menu.");
                    Console.ReadKey();
                    continue;
                }
            }
        }


    }
}
