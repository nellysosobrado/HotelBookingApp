using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBookingApp.Entities
{
    public class CanceledBookingHistory
    {
        public int Id { get; set; }
        public int BookingId { get; set; }
        public string GuestName { get; set; }
        public int RoomId { get; set; }
        public DateTime CanceledDate { get; set; }
        public string Reason { get; set; }
    }

}
