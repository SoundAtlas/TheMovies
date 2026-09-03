using System;
using System.Collections.Generic;
using System.Text;

namespace TheMovies.Core.Models
{
    public class Booking
    {
        public int Id { get; set; }
        public int BookingAmount { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }

        // Id of the screening this booking belongs to
        public int ScreeningId { get; set; }

        //Shows what date the booked screening is
        public DateTime ScreeningTime { get; set; }


    }
}
