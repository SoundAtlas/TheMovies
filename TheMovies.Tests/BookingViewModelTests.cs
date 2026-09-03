using TheMovies.WPF.ViewModels;

namespace TheMovies.Tests
{
    [TestClass]
    public class BookingViewModelTests
    {
        private static BookingViewModel CreateValidViewModel() => new()
        {
            BookingAmount = "1",
            SeatsLeft = 10,
            Email = "guest@example.com",
            PhoneNumber = "12345678"
        };

        [TestMethod]
        public void CanBook_WithValidFields_ReturnsTrue()
        {
            Assert.IsTrue(CreateValidViewModel().CanBook);
        }

        [TestMethod]
        [DataRow("0", 10)]
        [DataRow("1", 0)]
        [DataRow("11", 10)]
        [DataRow("not a number", 10)]
        public void CanBook_WithInvalidAmountOrInsufficientSeats_ReturnsFalse(
            string bookingAmount,
            int seatsLeft)
        {
            var viewModel = CreateValidViewModel();
            viewModel.BookingAmount = bookingAmount;
            viewModel.SeatsLeft = seatsLeft;

            Assert.IsFalse(viewModel.CanBook);
        }

        [TestMethod]
        [DataRow("")]
        [DataRow("guest@example")]
        [DataRow("guest.example.com")]
        public void CanBook_WithInvalidEmail_ReturnsFalse(string email)
        {
            var viewModel = CreateValidViewModel();
            viewModel.Email = email;

            Assert.IsFalse(viewModel.CanBook);
        }

        [TestMethod]
        [DataRow("")]
        [DataRow("1234567")]
        [DataRow("1234567890123456")]
        [DataRow("1234A678")]
        public void CanBook_WithInvalidPhoneNumber_ReturnsFalse(string phoneNumber)
        {
            var viewModel = CreateValidViewModel();
            viewModel.PhoneNumber = phoneNumber;

            Assert.IsFalse(viewModel.CanBook);
        }

        [TestMethod]
        [DataRow("12345678")]
        [DataRow("123456789012345")]
        public void CanBook_WithPhoneNumberAtLengthBoundaries_ReturnsTrue(string phoneNumber)
        {
            var viewModel = CreateValidViewModel();
            viewModel.PhoneNumber = phoneNumber;

            Assert.IsTrue(viewModel.CanBook);
        }
    }
}
