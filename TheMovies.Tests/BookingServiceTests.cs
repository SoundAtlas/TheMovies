using TheMovies.Core.Interfaces;
using TheMovies.Core.Models;
using TheMovies.Core.Services;

namespace TheMovies.Tests
{
    [TestClass]
    public class BookingServiceTests
    {
        [TestMethod]
        [TestCategory("Scenario 3")]
        public void CreateBooking_WithValidInput_AddsBooking()
        {
            // Scenario 3: A valid booking is stored for the selected screening.
            InMemoryBookingRepository bookingRepository = new();
            BookingService service = new(bookingRepository);
            Booking booking = new()
            {
                BookingAmount = 2,
                Email = "guest@example.com",
                PhoneNumber = "12345678"
            };

            BookingCreationResult result = service.CreateBooking(
                booking,
                screeningId: 7,
                hallCapacity: 10);

            Assert.IsTrue(result.IsSuccess);
            Assert.HasCount(1, bookingRepository.Bookings);
            Assert.AreEqual(1, bookingRepository.Bookings[0].Id);
            Assert.AreEqual(7, bookingRepository.Bookings[0].ScreeningId);
            Assert.AreEqual(2, bookingRepository.Bookings[0].BookingAmount);
            Assert.AreEqual("guest@example.com", bookingRepository.Bookings[0].Email);
            Assert.AreEqual("12345678", bookingRepository.Bookings[0].PhoneNumber);
        }

        [TestMethod]
        [TestCategory("Scenario 3")]
        public void CreateBooking_WhenBookingAmountExceedsSeatsLeft_DoesNotAddBooking()
        {
            // Scenario 3: A booking cannot exceed the selected screening's remaining capacity.
            InMemoryBookingRepository bookingRepository = new(
                new Booking
                {
                    Id = 1,
                    ScreeningId = 7,
                    BookingAmount = 8,
                    Email = "existing@example.com",
                    PhoneNumber = "87654321"
                });
            BookingService service = new(bookingRepository);
            Booking booking = new()
            {
                BookingAmount = 3,
                Email = "guest@example.com",
                PhoneNumber = "12345678"
            };

            BookingCreationResult result = service.CreateBooking(
                booking,
                screeningId: 7,
                hallCapacity: 10);

            Assert.IsFalse(result.IsSuccess);
            Assert.HasCount(1, bookingRepository.Bookings);
            Assert.AreEqual(0, bookingRepository.SaveCallCount);
            StringAssert.Contains(result.Message, "kun 2 pladser tilbage");
        }

        private sealed class InMemoryBookingRepository : IBookingRepository
        {
            public List<Booking> Bookings { get; private set; }
            public int SaveCallCount { get; private set; }

            public InMemoryBookingRepository(params Booking[] bookings)
            {
                Bookings = bookings.ToList();
            }

            public List<Booking> LoadBookings()
            {
                return Bookings.ToList();
            }

            public void SaveBookings(List<Booking> bookings)
            {
                Bookings = bookings.ToList();
                SaveCallCount++;
            }
        }
    }
}
