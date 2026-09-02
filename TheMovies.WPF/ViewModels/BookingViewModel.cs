using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using TheMovies.Core.Models;

namespace TheMovies.WPF.ViewModels
{
    public class BookingViewModel : INotifyPropertyChanged, IDataErrorInfo
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
            set { _email = value; OnPropertyChanged(); }
        }

        private string _phoneNumber = string.Empty;
        public string PhoneNumber
        {
            get => _phoneNumber;
            set { _phoneNumber = value; OnPropertyChanged(); }
        }

        private int _seatsLeft;
        public int SeatsLeft
        {
            get => _seatsLeft;
            set { _seatsLeft = value; OnPropertyChanged(); OnPropertyChanged(nameof(CanBook)); }
        }
        private DateTime _screeningTime;

        public DateTime ScreeningTime
        {
            get => _screeningTime;
            set { _screeningTime = value; OnPropertyChanged(); }
        }
        public bool CanBook
        {
            get
            {
                // BookingAmount must be a positive integer and <= SeatsLeft
                if (!int.TryParse(BookingAmount, out int amt)) return false;
                if (amt < 1) return false;
                if (SeatsLeft < 1) return false;
                return amt <= SeatsLeft;
            }
        }

        // Simple convenience: create a Booking from the entered values
        public Booking ToBooking()
        {
            int amount = 1;
            if (!int.TryParse(BookingAmount, out amount)) amount = 1;

            return new Booking
            {
                BookingAmount = amount,
                Email = this.Email,
                PhoneNumber = this.PhoneNumber
            };
        }

        #region IDataErrorInfo
        public string Error => null;

        public string this[string columnName]
        {
            get
            {
                if (columnName == nameof(BookingAmount))
                {
                    if (!int.TryParse(BookingAmount, out int amt))
                        return "Indtast et tal for antal.";

                    if (amt < 1) return "Antal skal være mindst 1.";

                    if (SeatsLeft < 1) return "Salen er udsolgt.";

                    if (amt > SeatsLeft) return $"Der er kun {SeatsLeft} pladser tilbage.";
                }

                return string.Empty;
            }
        }
        #endregion

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
