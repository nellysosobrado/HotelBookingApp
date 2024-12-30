using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HotelBookingApp.Entities
{
    public class Booking
    {
        [Key]
        public int BookingId { get; set; }

        public int RoomId { get; set; }

        public Room Room { get; set; }
        public int GuestId { get; set; }
        public Guest Guest { get; set; }
        public DateTime? CheckInDate { get; set; }
        public DateTime? CheckOutDate { get; set; }
        public bool IsCheckedIn { get; set; }
        public bool IsCheckedOut { get; set; }

        public bool BookingStatus { get; set; }
        public bool IsCanceled { get; set; }
        public ICollection<Invoice> Invoices { get; set; }
        public int ExtraBeds { get; set; }
        public DateTime CreatedDate { get; set; } // Lägg till detta
        public DateTime? CanceledDate { get; set; } // Lagra annulleringsdatum
    }
}
