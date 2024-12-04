using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBookingApp
{
    public class Booking
    {
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
        //true if booking is done
        public ICollection<Invoice> Invoices { get; set; }
    }

}
