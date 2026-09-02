using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using TheMovies.Core.Interfaces;
using TheMovies.Core.Models;

namespace TheMovies.WPF.ViewModels
{
    public class BookingDisplay
    {
        public int Id { get; set; }
        public int BookingAmount { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public int ScreeningId { get; set; }

        // display fields
        public string MovieTitle { get; set; }
        public string HallName { get; set; }
        public DateOnly? ScreeningDate { get; set; }
        public TimeOnly? ScreeningStart { get; set; }

        public string DisplayText => $"{BookingAmount} - {Email} - {PhoneNumber}";
    }

    public class BookingsViewModel : ViewModelBase
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly ICinemaRepository _cinemaRepository;
        private readonly IHallRepository _hallRepository;
        private readonly IMovieRepository _movieRepository;
        private readonly int? _filterScreeningId;

        public ObservableCollection<BookingDisplay> Bookings { get; set; } = new ObservableCollection<BookingDisplay>();

        private BookingDisplay? _selectedBooking;
        public BookingDisplay? SelectedBooking
        {
            get => _selectedBooking;
            set { _selectedBooking = value; OnPropertyChanged(); }
        }

        public BookingsViewModel(IBookingRepository bookingRepository, ICinemaRepository cinemaRepository, IHallRepository hallRepository, IMovieRepository movieRepository, int? filterScreeningId = null)
        {
            _bookingRepository = bookingRepository;
            _cinemaRepository = cinemaRepository;
            _hallRepository = hallRepository;
            _movieRepository = movieRepository;
            _filterScreeningId = filterScreeningId;
            Load();
        }

        public void Load()
        {
            Bookings.Clear();
            var bookings = _bookingRepository.LoadBookings();

            var cinemas = _cinemaRepository.LoadCinemas();
            var movies = _movieRepository.LoadMovies();
            var halls = _hallRepository.LoadHalls();

            foreach (var b in bookings)
            {
                // apply optional screening filter
                if (_filterScreeningId.HasValue && b.ScreeningId != _filterScreeningId.Value)
                    continue;
                Screening? found = null;
                foreach (var c in cinemas)
                {
                    found = c.Screenings.FirstOrDefault(s => s.Id == b.ScreeningId);
                    if (found != null) break;
                }

                string movieTitle = "Ukendt film";
                string hallName = "Ukendt sal";
                DateOnly? screeningDate = null;
                TimeOnly? screeningStart = null;

                if (found != null)
                {
                    var movie = movies.FirstOrDefault(m => m.Id == found.MovieId);
                    var hall = halls.FirstOrDefault(h => h.Id == found.HallId);
                    movieTitle = movie?.Title ?? movieTitle;
                    hallName = hall?.Name ?? hallName;
                    screeningDate = found.Date;
                    screeningStart = found.StartTime;
                }

                Bookings.Add(new BookingDisplay
                {
                    Id = b.Id,
                    BookingAmount = b.BookingAmount,
                    Email = b.Email,
                    PhoneNumber = b.PhoneNumber,
                    ScreeningId = b.ScreeningId,
                    MovieTitle = movieTitle,
                    HallName = hallName,
                    ScreeningDate = screeningDate,
                    ScreeningStart = screeningStart
                });
            }
        }

        public void EditSelectedBooking()
        {
            if (SelectedBooking == null) return;

            var allBookings = _bookingRepository.LoadBookings();
            int screeningId = SelectedBooking.ScreeningId;

            // find screening to get hall id
            var cinemas = _cinemaRepository.LoadCinemas();
            Screening? screening = null;
            foreach (var c in cinemas)
            {
                screening = c.Screenings.FirstOrDefault(s => s.Id == screeningId);
                if (screening != null) break;
            }

            if (screening == null)
            {
                MessageBox.Show("Forestillingen kunne ikke findes.", "Fejl", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var halls = _hallRepository.LoadHalls();
            var hall = halls.FirstOrDefault(h => h.Id == screening.HallId);
            if (hall == null)
            {
                MessageBox.Show("Sal kunne ikke findes.", "Fejl", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            int alreadyBookedExcludingThis = allBookings
                .Where(b => b.ScreeningId == screeningId && b.Id != SelectedBooking.Id)
                .Sum(b => b.BookingAmount);

            int seatsLeft = hall.Capacity - alreadyBookedExcludingThis;

            // Open dialog to edit
            var vm = new BookingViewModel();
            vm.SeatsLeft = seatsLeft;
            vm.BookingAmount = SelectedBooking.BookingAmount.ToString();
            vm.Email = SelectedBooking.Email;
            vm.PhoneNumber = SelectedBooking.PhoneNumber;

            var dialog = new Views.BookingDialog();
            dialog.DataContext = vm;
            dialog.Owner = Application.Current?.MainWindow;
            bool? result = dialog.ShowDialog();
            if (result == true)
            {
                // re-load bookings and re-check capacity
                var bookings = _bookingRepository.LoadBookings();
                var target = bookings.FirstOrDefault(x => x.Id == SelectedBooking.Id);
                if (target != null)
                {
                    try
                    {
                        var newBooking = vm.ToBooking();

                        int already = bookings.Where(b => b.ScreeningId == screeningId && b.Id != target.Id).Sum(b => b.BookingAmount);
                        int seatsNowLeft = hall.Capacity - already;
                        if (newBooking.BookingAmount > seatsNowLeft)
                        {
                            MessageBox.Show(seatsNowLeft > 0
                                ? $"Kan ikke opdatere. Der er kun {seatsNowLeft} pladser tilbage for denne forestilling."
                                : "Kan ikke opdatere. Salen er udsolgt for denne forestilling.",
                                "Ikke nok pladser", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                        }

                        target.BookingAmount = newBooking.BookingAmount;
                        target.Email = newBooking.Email;
                        target.PhoneNumber = newBooking.PhoneNumber;
                        _bookingRepository.SaveBookings(bookings);
                        Load();

                        if (_bookingRepository is TheMovies.Core.Repositories.FileBookingRepository fileRepo)
                        {
                            MessageBox.Show($"Booking gemt i: {fileRepo.FilePath}", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                    catch (System.FormatException fx)
                    {
                        MessageBox.Show(fx.Message, "Ugyldige data", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                    catch (System.Exception)
                    {
                        MessageBox.Show("Der opstod en fejl ved opdatering af bookingen.", "Fejl", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }
            }
        }

        public void DeleteSelectedBooking()
        {
            if (SelectedBooking == null) return;

            var confirm = MessageBox.Show("Er du sikker på du vil slette denne booking?", "Bekræft", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (confirm != MessageBoxResult.Yes) return;

            var bookings = _bookingRepository.LoadBookings();
            var target = bookings.FirstOrDefault(x => x.Id == SelectedBooking.Id);
            if (target != null)
            {
                bookings.Remove(target);
                _bookingRepository.SaveBookings(bookings);
                Load();

                if (_bookingRepository is TheMovies.Core.Repositories.FileBookingRepository fileRepo)
                {
                    MessageBox.Show($"Booking slettet. Fil: {fileRepo.FilePath}", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }
    }
}
