using TheMovies.Core.Interfaces;
using TheMovies.Core.Models;

namespace TheMovies.Core.Services
{
    public class BookingService
    {
        private readonly IBookingRepository _bookingRepository;

        public BookingService(IBookingRepository bookingRepository)
        {
            _bookingRepository = bookingRepository;
        }

        public int GetSeatsLeft(int screeningId, int hallCapacity)
        {
            int alreadyBooked = _bookingRepository.LoadBookings()
                .Where(booking => booking.ScreeningId == screeningId)
                .Sum(booking => booking.BookingAmount);

            return hallCapacity - alreadyBooked;
        }

        public BookingCreationResult CreateBooking(
            Booking booking,
            int screeningId,
            int hallCapacity)
        {
            List<Booking> bookings = _bookingRepository.LoadBookings();

            if (booking.BookingAmount < 1)
                booking.BookingAmount = 1;

            int alreadyBooked = bookings
                .Where(existingBooking => existingBooking.ScreeningId == screeningId)
                .Sum(existingBooking => existingBooking.BookingAmount);

            int seatsLeft = hallCapacity - alreadyBooked;
            if (booking.BookingAmount > seatsLeft)
            {
                string message = seatsLeft > 0
                    ? $"Der er kun {seatsLeft} pladser tilbage i salen. Vælg et lavere antal."
                    : "Salen er udsolgt for denne forestilling.";

                return new BookingCreationResult(false, message, null);
            }

            booking.Id = bookings.Any()
                ? bookings.Max(existingBooking => existingBooking.Id) + 1
                : 1;
            booking.ScreeningId = screeningId;

            bookings.Add(booking);
            _bookingRepository.SaveBookings(bookings);

            return new BookingCreationResult(true, "Booking gennemført.", booking);
        }
    }

    public record BookingCreationResult(
        bool IsSuccess,
        string Message,
        Booking? Booking);
}
