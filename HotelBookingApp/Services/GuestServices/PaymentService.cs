﻿using HotelBookingApp.Repositories;
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

        public void PayInvoiceBeforeCheckout()
        {
            while (true)
            {
                Console.Clear();
                var activeBookings = _bookingRepository.GetActiveBookings().ToList();

                _tableDisplayService.DisplayBookingTable(activeBookings, "Active Bookings:");
                Console.WriteLine(new string('-', 100));
                Console.Write("Enter Booking ID to pay the guest invoice (type 'back' to go back): ");
                var input = Console.ReadLine();

                if (input?.ToLower() == "back")
                    return;

                if (!int.TryParse(input, out int bookingId))
                {
                    AnsiConsole.MarkupLine("[red]Invalid Booking ID. Please try again[/]");
                    Console.ReadKey();
                    continue;
                }

                var invoice = _bookingRepository.GetInvoiceByBookingId(bookingId);
                if (invoice == null || invoice.IsPaid)
                {
                    AnsiConsole.MarkupLine("[red]No unpaid invoice found for this booking[/]");
                    Console.ReadKey();
                    continue;
                }

                AnsiConsole.MarkupLine($"[green]Invoice Total: {invoice.TotalAmount:C}[/]");
                var confirmPayment = AnsiConsole.Confirm($"Do you want to pay this invoice of {invoice.TotalAmount:C}?");
                if (!confirmPayment)
                {
                    AnsiConsole.MarkupLine("[red]Payment cancelled. Enter any key to continue[/]");
                    Console.ReadKey();
                    continue;
                }

                _bookingRepository.ProcessPayment(invoice, invoice.TotalAmount);
                AnsiConsole.MarkupLine("[green]Invoice paid successfully.[/]");

                AnsiConsole.MarkupLine("[white]Do you want to pay another invoice? (yes/no).[/]");
                var choice = Console.ReadLine()?.ToLower();

                if (choice == "no")
                {
                    Console.WriteLine("Enter any key to continue.");
                    Console.ReadKey();
                    return; 
                }
            }
        }
    }
}
