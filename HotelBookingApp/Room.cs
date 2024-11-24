using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBookingApp
{
    public class Room//table
    {
        //backing field/properties//atributes
        public int RoomId { get; set; }
        public string Type { get; set; }
        public int ExtraBeds { get; set; } 
        public bool IsAvailable { get; set; }

    }
}
