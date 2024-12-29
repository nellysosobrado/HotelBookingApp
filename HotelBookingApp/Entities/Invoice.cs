using HotelBookingApp.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HotelBookingApp
{
    public class Invoice
    {
        [Key]
        public int InvoiceId { get; set; }

        public int BookingId { get; set; } 

        [Required]  
        public decimal TotalAmount { get; set; }

        public bool IsPaid { get; set; }

        [Required]  
        public DateTime PaymentDeadline { get; set; }

        public Booking Booking { get; set; }
        public ICollection<Payment> Payments { get; set; }
        public DateTime CreatedDate { get; set; } // Lägg till denna
    }
}
