using System.ComponentModel;
using TheMovies.Core.Models;

namespace TheMovies.WPF.ViewModels
{
    public class BookingViewModel : ViewModelBase, IDataErrorInfo
    {
        private string _bookingAmount = "1";
        public string BookingAmount
        {
            get => _bookingAmount;
            set
            {
                if (_bookingAmount == value) return;

                _bookingAmount = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CanBook));
            }
        }

        private string _email = string.Empty;
        public string Email
        {
            get => _email;
            set
            {
                _email = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CanBook));
            }
        }

        private string _phoneNumber = string.Empty;
        public string PhoneNumber
        {
            get => _phoneNumber;
            set
            {
                _phoneNumber = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CanBook));
            }
        }

        private int _seatsLeft;
        public int SeatsLeft
        {
            get => _seatsLeft;
            set
            {
                _seatsLeft = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CanBook));
            }
        }

        private DateTime _screeningTime;
        public DateTime ScreeningTime
        {
            get => _screeningTime;
            set
            {
                _screeningTime = value;
                OnPropertyChanged();
            }
        }

        public bool CanBook =>
                int.TryParse(BookingAmount, out int amount) &&
                amount >= 1 &&
                amount <= SeatsLeft &&
                !string.IsNullOrWhiteSpace(Email) &&
                Email.Contains('@') &&
                Email.Contains('.') &&
                !string.IsNullOrWhiteSpace(PhoneNumber) &&
                PhoneNumber.All(char.IsDigit) &&
                PhoneNumber.Length is >= 8 and <= 15;


        public Booking ToBooking()
        {
            int amount = 1;

            if (!int.TryParse(BookingAmount, out amount))
                amount = 1;

            return new Booking
            {
                BookingAmount = amount,
                Email = Email,
                PhoneNumber = PhoneNumber
            };
        }

        #region IDataErrorInfo

        public string Error => string.Empty;

        public string this[string columnName]
        {
            get
            {
                if (columnName == nameof(BookingAmount))
                {
                    if (!int.TryParse(BookingAmount, out int amt))
                        return "Indtast et tal for antal.";

                    if (amt < 1)
                        return "Antal skal være mindst 1.";

                    if (SeatsLeft < 1)
                        return "Salen er udsolgt.";

                    if (amt > SeatsLeft)
                        return $"Der er kun {SeatsLeft} pladser tilbage.";
                }

                if (columnName == nameof(Email))
                {
                    if (string.IsNullOrWhiteSpace(Email))
                        return "Email skal udfyldes.";

                    if (!Email.Contains('@') || !Email.Contains('.'))
                        return "Email skal indeholde '.' og '@'.";
                }

                if (columnName == nameof(PhoneNumber))
                {
                    if (string.IsNullOrWhiteSpace(PhoneNumber))
                        return "Telefonnummer skal udfyldes.";

                    if (!PhoneNumber.All(char.IsDigit))
                        return "Telefonnummer må kun indeholde tal.";

                    if (PhoneNumber.Length < 8 || PhoneNumber.Length > 15)
                        return "Telefonnummer skal være mellem 8 og 15 cifre.";
                }

                return string.Empty;
            }
        }

        #endregion


    }
}
