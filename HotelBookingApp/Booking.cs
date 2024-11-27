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
        public int GuestId { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public bool IsCheckedIn { get; set; } // Redan används för check-in status
        public bool IsCheckedOut { get; set; } // Lägg till denna för att spåra check-out status
    }

}
