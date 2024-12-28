using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBookingApp.Entities
{

    public class Room
    {
        [Key]
        public int RoomId { get; set; }
        [Required]
        public string Type { get; set; }
        public int ExtraBeds { get; set; }
        public bool IsAvailable { get; set; }

        public decimal TotalPeople { get; set; }


        public decimal ExtraBedPrice { get; set; }

        [Required]
        public decimal PricePerNight { get; set; } 

        [Required]
        public int SizeInSquareMeters { get; set; }

        public ICollection<Booking> Bookings { get; set; }
        public bool IsDeleted { get; set; } 

    }
}
