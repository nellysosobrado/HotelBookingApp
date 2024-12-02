using System;
using System.Collections.Generic;

namespace HotelBookingApp
{
    public class Invoice
    {
        public int InvoiceId { get; set; }
        public int BookingId { get; set; } // Foreign Key
        public decimal TotalAmount { get; set; }
        public bool IsPaid { get; set; }
        public DateTime PaymentDeadline { get; set; }

        // Navigation property
        public Booking Booking { get; set; }
        public ICollection<Payment> Payments { get; set; }
    }
}
