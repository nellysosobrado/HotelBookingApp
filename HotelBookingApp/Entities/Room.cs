using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBookingApp.Entities
{
    /// <summary>
    /// Creates a 'Room' class. This will be read from the Entitiyframeworkcore and transforms it into a table inside the databas, along with the properties witch wil be the attriburtes in the database tab le 'room'
    /// 
    /// </summary>
    public class Room//table
    {
        //backing field/properties//atributes
        [Key]
        public int RoomId { get; set; }
        [Required]
        public string Type { get; set; }
        public int ExtraBeds { get; set; }
        public bool IsAvailable { get; set; }

        public decimal TotalPeople { get; set; }

        //--------



        [Required]
        public decimal PricePerNight { get; set; } 

        [Required]
        public int SizeInSquareMeters { get; set; }

        //relation to booking
        public ICollection<Booking> Bookings { get; set; }

    }
}
