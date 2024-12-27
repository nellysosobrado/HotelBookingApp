using System;
using System.ComponentModel.DataAnnotations;

namespace HotelBookingApp
{
    public class Payment
    {
        [Key] 
        public int PaymentId { get; set; }

        public int InvoiceId { get; set; } 

        [Required]  
        public DateTime PaymentDate { get; set; }

        [Required]  
        public decimal AmountPaid { get; set; }

       
        public Invoice Invoice { get; set; }
    }
}
