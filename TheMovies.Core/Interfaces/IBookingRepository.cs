using System;
using System.Collections.Generic;
using System.Text;
using TheMovies.Core.Models;

namespace TheMovies.Core.Interfaces
{
    public interface IBookingRepository
    {
        List<Booking> LoadBookings();
        void SaveBookings(List<Booking> bookings);
    }
}
