using HotelBookingApp.Entities;
using System;
using System.ComponentModel.DataAnnotations;

namespace HotelBookingApp
{
    public class Guest
    {
        [Key]
        public int GuestId { get; set; }

        [Required] 
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; } = string.Empty;

        [Required] 
        public string Email { get; set; }

        [Required] 
        public string PhoneNumber { get; set; }
        public bool IsDeleted { get; set; }
        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    }
}
