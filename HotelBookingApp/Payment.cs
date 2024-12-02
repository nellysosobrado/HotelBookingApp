using System;

namespace HotelBookingApp
{
    public class Payment
    {
        public int PaymentId { get; set; }
        public int InvoiceId { get; set; } // Foreign Key
        public DateTime PaymentDate { get; set; }
        public decimal AmountPaid { get; set; }

        // Navigation property
        public Invoice Invoice { get; set; }
    }
}
