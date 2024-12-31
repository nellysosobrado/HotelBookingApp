using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelBookingApp.Entities
{
    public class CanceledBookingHistory
    {
        public int Id { get; set; }

        public int BookingId { get; set; }
        public Booking Booking { get; set; }
        public int RoomId { get; set; } 
        public Room Room { get; set; } 
        public string GuestName { get; set; }
        public DateTime CanceledDate { get; set; }
        public string Reason { get; set; }
        public bool IsCanceled { get; set; }





    }
}
