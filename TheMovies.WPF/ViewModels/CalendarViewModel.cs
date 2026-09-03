using System.Collections.ObjectModel;
using System.Windows.Input;
using TheMovies.Core.Interfaces;
using TheMovies.Core.Models;
using TheMovies.Core.Services;
using TheMovies.WPF.DisplayModels;
using TheMovies.WPF.Views;

namespace TheMovies.WPF.ViewModels
{




    public class CalendarViewModel : ViewModelBase
    {
        private readonly ICinemaRepository _cinemaRepository;
        private readonly IMovieRepository _movieRepository;
        private readonly IHallRepository _hallRepository;
        private readonly IBookingRepository _bookingRepository;
        private readonly BookingService _bookingService;
        private int _year;
        private int _month;

        private static readonly string[] MonthNames =
        {
            "Januar", "Februar", "Marts", "April", "Maj", "Juni",
            "Juli", "August", "September", "Oktober", "November", "December"
        };
        private static readonly string[] DayNames =
        {
            "Mandag", "Tirsdag", "Onsdag", "Torsdag", "Fredag", "Lørdag", "Søndag"
        };

        // Alle biografer — fylder ComboBox'en øverst
        private ObservableCollection<Cinema> _cinemas = new();
        public ObservableCollection<Cinema> Cinemas
        {
            get => _cinemas;
            set { _cinemas = value; OnPropertyChanged(); }
        }

        private void ViewBookings()
        {
            int? filterScreeningId = SelectedScreening?.ScreeningId;
            string? screeningDescription = SelectedScreening == null
                ? null
                : $"{SelectedScreening.MovieTitle} – {SelectedScreening.Date:dd-MM-yyyy} kl. {SelectedScreening.StartTime:HH\\:mm}";
            var vm = new BookingsViewModel(
                _bookingRepository,
                _cinemaRepository,
                _hallRepository,
                _movieRepository,
                filterScreeningId,
                screeningDescription);
            var view = new Views.BookingsView();
            view.DataContext = vm;
            view.Owner = System.Windows.Application.Current?.MainWindow;
            view.ShowDialog();
        }

        // Den biograf brugeren har valgt
        private Cinema? _selectedCinema;
        public Cinema? SelectedCinema
        {
            get => _selectedCinema;
            set
            {
                _selectedCinema = value;
                OnPropertyChanged();
                LoadAvailableHalls();
                // Skift af biograf og skift af måned skal begge opdatere kalendergitteret, så vi kalder BuildCalendar() her.
                BuildCalendar();
            }
        }

        private ObservableCollection<DayDisplay> _days = new();
        public ObservableCollection<DayDisplay> Days
        {
            get => _days;
            set { _days = value; OnPropertyChanged(); }
        }

        private DayDisplay? _selectedDay;
        public DayDisplay? SelectedDay
        {
            get => _selectedDay;
            set
            {
                _selectedDay = value;
                OnPropertyChanged();
                SelectedScreening = null;

                if (value != null && SelectedCinema != null)
                {
                    int dayIndex = ((int)value.Date.DayOfWeek == 0) ? 6 : (int)value.Date.DayOfWeek - 1;
                    SelectedDateLabel = $"{DayNames[dayIndex]} d. {value.Date.Day}. {MonthNames[value.Date.Month - 1].ToLower()}";
                    LoadScreenings(value.Date);
                }
                else
                {
                    SelectedDateLabel = "Vælg en dag i kalenderen";
                    Screenings = new ObservableCollection<ScreeningDisplay>();
                }
            }
        }

        private string _selectedDateLabel = "Vælg en dag i kalenderen";
        public string SelectedDateLabel
        {
            get => _selectedDateLabel;
            set { _selectedDateLabel = value; OnPropertyChanged(); }
        }

        public string BookingsFilterText => SelectedScreening == null
            ? "Bookinger: Alle forestillinger"
            : $"Bookinger: {SelectedScreening.MovieTitle} kl. {SelectedScreening.StartTime:HH\\:mm}";

        public string ViewBookingsButtonText => SelectedScreening == null
            ? "Se alle bookinger"
            : "Se valgte bookinger";

        private Movie? _selectedMovie;
        public Movie? SelectedMovie
        {
            get => _selectedMovie;
            set
            {
                _selectedMovie = value;
                OnPropertyChanged();
            }
        }

        private Hall? _selectedHall;
        public Hall? SelectedHall
        {
            get => _selectedHall;
            set
            {
                _selectedHall = value;
                OnPropertyChanged();
            }
        }

        private DateTime _screeningDate = DateTime.Today;
        public DateTime ScreeningDate
        {
            get => _screeningDate;
            set
            {
                _screeningDate = value;
                OnPropertyChanged();
            }
        }

        private string _screeningStartTime = "";
        public string ScreeningStartTime
        {
            get => _screeningStartTime;
            set
            {
                _screeningStartTime = value;
                OnPropertyChanged();
            }
        }

        private ScreeningDisplay? _selectedScreening;

        public ScreeningDisplay? SelectedScreening
        {
            get => _selectedScreening;
            set
            {
                _selectedScreening = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(BookingsFilterText));
                OnPropertyChanged(nameof(ViewBookingsButtonText));

                if (_selectedScreening == null)
                    return;

                SelectedMovie = Movies.FirstOrDefault(
                    m => m.Id == _selectedScreening.MovieId);

                SelectedHall = AvailableHalls.FirstOrDefault(
                    h => h.Id == _selectedScreening.HallId);

                ScreeningDate =
                    _selectedScreening.Date.ToDateTime(TimeOnly.MinValue);

                ScreeningStartTime =
                    $"{_selectedScreening.StartTime:HH\\:mm}";
            }
        }

        private string _statusMessage = "";
        public string StatusMessage
        {
            get => _statusMessage;
            set
            {
                _statusMessage = value;
                OnPropertyChanged();
            }
        }

        public string MonthLabel => $"{MonthNames[_month - 1]} {_year}";

        // Dagsoversigten: forestillinger for valgt dag i valgt biograf
        private ObservableCollection<ScreeningDisplay> _screenings = new();
        public ObservableCollection<ScreeningDisplay> Screenings
        {
            get => _screenings;
            set { _screenings = value; OnPropertyChanged(); }
        }

        public ObservableCollection<Movie> Movies { get; set; }

        public ObservableCollection<Hall> Halls { get; set; }

        private ObservableCollection<Hall> _availableHalls = new ObservableCollection<Hall>();

        public ObservableCollection<Hall> AvailableHalls
        {
            get => _availableHalls;
            set
            {
                _availableHalls = value;
                OnPropertyChanged();
            }
        }

        public ICommand PreviousMonthCommand { get; }
        public ICommand NextMonthCommand { get; }
        public ICommand RegisterScreeningCommand { get; }
        public ICommand DeleteScreeningCommand { get; }
        public ICommand SaveScreeningChangesCommand { get; }
        public ICommand BookScreeningCommand { get; }
        public ICommand ViewBookingsCommand { get; }

        public CalendarViewModel(
            ICinemaRepository cinemaRepository,
            IMovieRepository movieRepository,
            IHallRepository hallRepository,
            IBookingRepository bookingRepository) // her injecter vi repositories ind i viewmodel'en via konstruktøren
        {
            _cinemaRepository = cinemaRepository;
            _movieRepository = movieRepository;
            _hallRepository = hallRepository;
            _bookingRepository = bookingRepository;
            _bookingService = new BookingService(bookingRepository);

            _year = DateTime.Today.Year;
            _month = DateTime.Today.Month;

            PreviousMonthCommand = new RelayCommand(PreviousMonth);
            NextMonthCommand = new RelayCommand(NextMonth);
            RegisterScreeningCommand = new RelayCommand(RegisterScreening);
            DeleteScreeningCommand = new RelayCommand(DeleteScreening);
            SaveScreeningChangesCommand = new RelayCommand(SaveScreeningChanges, CanSaveScreeningChanges);
            BookScreeningCommand = new RelayCommand(BookScreening, () => SelectedScreening != null);
            ViewBookingsCommand = new RelayCommand(ViewBookings);

            Screenings = new ObservableCollection<ScreeningDisplay>();

            List<Cinema> loadedCinemas = _cinemaRepository.LoadCinemas();
            Cinemas = new ObservableCollection<Cinema>(loadedCinemas);

            Movies = new ObservableCollection<Movie>(_movieRepository.LoadMovies());
            Halls = new ObservableCollection<Hall>(_hallRepository.LoadHalls());


            SelectedCinema = Cinemas.FirstOrDefault();

        }

        private void PreviousMonth() // Skifter til forrige måned og opdaterer kalendergitteret
        {
            _month--;
            if (_month < 1) { _month = 12; _year--; }
            OnPropertyChanged(nameof(MonthLabel));
            BuildCalendar();
        }

        private void NextMonth() // Skifter til næste måned og opdaterer kalendergitteret
        {
            _month++;
            if (_month > 12) { _month = 1; _year++; }
            OnPropertyChanged(nameof(MonthLabel));
            BuildCalendar();
        }

        private void BuildCalendar() // Bygger kalendergitteret med 42 felter (6 rækker × 7 kolonner)
        {
            var days = new ObservableCollection<DayDisplay>();

            var firstDay = new DateOnly(_year, _month, 1);
            int weekday = (int)firstDay.DayOfWeek;
            int startOffset = (weekday == 0) ? 6 : weekday - 1;
            var startDate = firstDay.AddDays(-startOffset);

            for (int i = 0; i < 42; i++)
            {
                var date = startDate.AddDays(i);
                days.Add(new DayDisplay
                {
                    Date = date,
                    IsInCurrentMonth = date.Month == _month && date.Year == _year,
                    HasScreenings = DayHasScreenings(date)
                });
            }

            Days = days;
            SelectedDay = null;
        }

        private bool DayHasScreenings(DateOnly date) // Tjekker om der er forestillinger for den valgte biograf på den givne dato
        {
            if (SelectedCinema == null) return false;
            return SelectedCinema.Screenings.Any(s => s.Date == date);
        }

        private void LoadScreenings(DateOnly date)
        {
            if (SelectedCinema == null)
            {
                Screenings = new ObservableCollection<ScreeningDisplay>();
                return;
            }

            List<Movie> movies = _movieRepository.LoadMovies();
            List<Hall> halls = _hallRepository.LoadHalls();

            var displays = new List<ScreeningDisplay>();

            foreach (Screening screening in SelectedCinema.Screenings)
            {
                if (screening.Date != date)
                    continue;

                Movie? movie = movies.FirstOrDefault(m => m.Id == screening.MovieId);
                Hall? hall = halls.FirstOrDefault(h => h.Id == screening.HallId);

                displays.Add(new ScreeningDisplay
                {
                    ScreeningId = screening.Id,
                    MovieId = screening.MovieId,
                    HallId = screening.HallId,
                    Date = screening.Date,
                    StartTime = screening.StartTime,

                    TimeSlot = $"{screening.StartTime:HH\\:mm}",
                    MovieTitle = movie?.Title ?? "Ukendt film",
                    HallName = hall?.Name ?? "Ukendt sal",

                    // check if the screening is a premiere by comparing the screening date with the movie's release date
                    IsPremiere = movie != null &&
                                  screening.Date == DateOnly.FromDateTime(movie.ReleaseDate)
                });
            }

            displays = displays
                .OrderBy(d => d.TimeSlot)
                .ToList();

            Screenings = new ObservableCollection<ScreeningDisplay>(displays);
        }

        private void RegisterScreening()
        {
            if (SelectedCinema == null)
            {
                StatusMessage = "Vælg en biograf.";
                return;
            }

            if (SelectedMovie == null)
            {
                StatusMessage = "Vælg en film.";
                return;
            }

            if (SelectedHall == null)
            {
                StatusMessage = "Vælg en sal.";
                return;
            }

            if (!TimeOnly.TryParse(ScreeningStartTime, out TimeOnly startTime))
            {
                StatusMessage = "Indtast et gyldigt tidspunkt, f.eks. 18:30.";
                return;
            }


            DateOnly screeningDate = DateOnly.FromDateTime(ScreeningDate);

            // Check if the screening date is before the movie's release date
            DateOnly releaseDate = DateOnly.FromDateTime(SelectedMovie.ReleaseDate);

            if (screeningDate < releaseDate)
            {
                StatusMessage =
                    $"Filmen kan ikke vises før premieredatoen " +
                    $"{releaseDate:dd/MM/yyyy}.";

                return;
            }

            // Check for screening conflicts in the selected hall on the selected date

            if (HasScreeningConflict(
                    SelectedHall.Id,
                    screeningDate,
                    startTime,
                    SelectedMovie.Duration))
            {
                TimeOnly nextAvailableTime = GetNextAvailableTime(
                    SelectedHall.Id,
                    screeningDate,
                    startTime,
                    SelectedMovie.Duration);

                StatusMessage =
                    $"Der er allerede en forestilling i salen på dette tidspunkt. " +
                    $"Næste ledige tidspunkt er kl. {nextAvailableTime:HH\\:mm}.";

                return;
            }



            // Generate a new unique ID for the screening

            int newId = 1;

            foreach (Cinema cinema in Cinemas)
            {
                foreach (Screening existingScreening in cinema.Screenings)
                {
                    if (existingScreening.Id >= newId)
                    {
                        newId = existingScreening.Id + 1;
                    }
                }
            }

            // Create a new Screening object and add it to the selected cinema's screenings

            Screening screening = new Screening
            {
                Id = newId,
                MovieId = SelectedMovie.Id,
                HallId = SelectedHall.Id,
                Date = screeningDate,
                StartTime = startTime
            };

            SelectedCinema.Screenings.Add(screening);

            _cinemaRepository.SaveCinemas(Cinemas.ToList());

            StatusMessage = "Forestillingen blev registreret.";

            ScreeningStartTime = "";

            BuildCalendar();
        }

        private void DeleteScreening()
        {
            if (SelectedCinema == null || SelectedScreening == null)
            {
                StatusMessage = "Vælg en forestilling, der skal slettes.";
                return;
            }

            Screening? screeningToDelete = SelectedCinema.Screenings
                .FirstOrDefault(s => s.Id == SelectedScreening.ScreeningId);

            if (screeningToDelete == null)
            {
                StatusMessage = "Forestillingen kunne ikke findes.";
                return;
            }

            SelectedCinema.Screenings.Remove(screeningToDelete);

            _cinemaRepository.SaveCinemas(Cinemas.ToList());

            StatusMessage = "Forestillingen blev slettet.";

            DateOnly deletedDate = screeningToDelete.Date;

            BuildCalendar();

            // Vis dagsoversigten igen efter kalenderen er blevet genopbygget
            LoadScreenings(deletedDate);

            SelectedScreening = null;
        }

        private void SaveScreeningChanges()
        {
            if (SelectedCinema == null ||
                SelectedScreening == null ||
                SelectedMovie == null ||
                SelectedHall == null)
            {
                StatusMessage = "Vælg en forestilling, der skal redigeres.";
                return;
            }

            if (!TimeOnly.TryParse(ScreeningStartTime, out TimeOnly startTime))
            {
                StatusMessage = "Indtast et gyldigt tidspunkt, f.eks. 18:30.";
                return;
            }

            DateOnly screeningDate = DateOnly.FromDateTime(ScreeningDate);

            // Check if the screening date is before the movie's release date
            DateOnly releaseDate = DateOnly.FromDateTime(SelectedMovie.ReleaseDate);

            if (screeningDate < releaseDate)
            {
                StatusMessage =
                    $"Filmen kan ikke vises før premieredatoen " +
                    $"{releaseDate:dd/MM/yyyy}.";

                return;
            }

            // Find the screening to update in the selected cinema's screenings
            Screening? screeningToUpdate =
                SelectedCinema.Screenings.FirstOrDefault(
                    s => s.Id == SelectedScreening.ScreeningId);

            if (screeningToUpdate == null)
            {
                StatusMessage = "Forestillingen kunne ikke findes.";
                return;
            }

            bool hasConflict = HasScreeningConflictExceptCurrent(
                SelectedHall.Id,
                screeningDate,
                startTime,
                SelectedMovie.Duration,
                screeningToUpdate.Id);

            if (hasConflict)
            {
                TimeOnly nextAvailableTime =
                    GetNextAvailableTimeExceptCurrent(
                        SelectedHall.Id,
                        screeningDate,
                        startTime,
                        SelectedMovie.Duration,
                        screeningToUpdate.Id);

                StatusMessage =
                    $"Ændringen giver en konflikt med en anden forestilling i salen. " +
                    $"Næste ledige tidspunkt er kl. {nextAvailableTime:HH\\:mm}.";

                return;
            }

            screeningToUpdate.MovieId = SelectedMovie.Id;
            screeningToUpdate.HallId = SelectedHall.Id;
            screeningToUpdate.Date = screeningDate;
            screeningToUpdate.StartTime = startTime;

            _cinemaRepository.SaveCinemas(Cinemas.ToList());

            StatusMessage = "Forestillingen blev opdateret.";

            BuildCalendar();

            SelectedScreening = null;
            SelectedMovie = null;
            SelectedHall = null;
            ScreeningStartTime = "";
        }

        private bool HasScreeningConflict(
                        int hallId,
                        DateOnly date,
                        TimeOnly newStartTime,
                        int newMovieDuration)
        {
            foreach (Cinema cinema in Cinemas)
            {
                foreach (Screening existingScreening in cinema.Screenings)
                {
                    if (existingScreening.HallId != hallId)
                        continue;

                    if (existingScreening.Date != date)
                        continue;

                    Movie? existingMovie =
                        Movies.FirstOrDefault(m => m.Id == existingScreening.MovieId);

                    if (existingMovie == null)
                        continue;

                    DateTime existingStart =
                        existingScreening.Date.ToDateTime(existingScreening.StartTime);

                    DateTime existingEnd =
                        existingStart.AddMinutes(existingMovie.Duration + 30);

                    DateTime newStart =
                        date.ToDateTime(newStartTime);

                    DateTime newEnd =
                        newStart.AddMinutes(newMovieDuration + 30);

                    if (newStart < existingEnd &&
                        newEnd > existingStart)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private bool HasScreeningConflictExceptCurrent(
                        int hallId,
                        DateOnly date,
                        TimeOnly newStartTime,
                        int newMovieDuration,
                        int screeningIdToIgnore)
        {
            foreach (Cinema cinema in Cinemas)
            {
                foreach (Screening existingScreening in cinema.Screenings)
                {
                    if (existingScreening.Id == screeningIdToIgnore)
                        continue;

                    if (existingScreening.HallId != hallId)
                        continue;

                    if (existingScreening.Date != date)
                        continue;

                    Movie? existingMovie =
                        Movies.FirstOrDefault(
                            m => m.Id == existingScreening.MovieId);

                    if (existingMovie == null)
                        continue;

                    DateTime existingStart =
                        existingScreening.Date.ToDateTime(
                            existingScreening.StartTime);

                    DateTime existingEnd =
                        existingStart.AddMinutes(
                            existingMovie.Duration + 30);

                    DateTime newStart =
                        date.ToDateTime(newStartTime);

                    DateTime newEnd =
                        newStart.AddMinutes(newMovieDuration + 30);

                    if (newStart < existingEnd &&
                        newEnd > existingStart)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private TimeOnly GetNextAvailableTime(
                            int hallId,
                            DateOnly date,
                            TimeOnly requestedStartTime,
                            int newMovieDuration)
        {
            DateTime candidateStart = date.ToDateTime(requestedStartTime);

            // Find alle forestillinger i den valgte sal på den valgte dato
            List<Screening> screeningsForHall = new List<Screening>();

            foreach (Cinema cinema in Cinemas)
            {
                foreach (Screening screening in cinema.Screenings)
                {
                    if (screening.HallId == hallId &&
                        screening.Date == date)
                    {
                        screeningsForHall.Add(screening);
                    }
                }
            }

            // Sorter dem efter starttid
            screeningsForHall = screeningsForHall
                .OrderBy(s => s.StartTime)
                .ToList();

            foreach (Screening existingScreening in screeningsForHall)
            {
                Movie? existingMovie =
                    Movies.FirstOrDefault(
                        m => m.Id == existingScreening.MovieId);

                if (existingMovie == null)
                    continue;

                DateTime existingStart =
                    date.ToDateTime(existingScreening.StartTime);

                DateTime existingEnd =
                    existingStart.AddMinutes(
                        existingMovie.Duration + 30);

                DateTime candidateEnd =
                    candidateStart.AddMinutes(
                        newMovieDuration + 30);

                // Den nye forestilling overlapper den eksisterende
                if (candidateStart < existingEnd &&
                    candidateEnd > existingStart)
                {
                    // Prøv igen lige efter den eksisterende forestilling
                    candidateStart = existingEnd;
                }
            }

            return TimeOnly.FromDateTime(candidateStart);
        }


        private TimeOnly GetNextAvailableTimeExceptCurrent(
                            int hallId,
                            DateOnly date,
                            TimeOnly requestedStartTime,
                            int newMovieDuration,
                            int screeningIdToIgnore)
        {
            DateTime candidateStart = date.ToDateTime(requestedStartTime);

            List<Screening> screeningsForHall = new List<Screening>();

            foreach (Cinema cinema in Cinemas)
            {
                foreach (Screening screening in cinema.Screenings)
                {
                    if (screening.Id == screeningIdToIgnore)
                        continue;

                    if (screening.HallId == hallId &&
                        screening.Date == date)
                    {
                        screeningsForHall.Add(screening);
                    }
                }
            }

            screeningsForHall = screeningsForHall
                .OrderBy(s => s.StartTime)
                .ToList();

            foreach (Screening existingScreening in screeningsForHall)
            {
                Movie? existingMovie =
                    Movies.FirstOrDefault(
                        m => m.Id == existingScreening.MovieId);

                if (existingMovie == null)
                    continue;

                DateTime existingStart =
                    date.ToDateTime(existingScreening.StartTime);

                DateTime existingEnd =
                    existingStart.AddMinutes(
                        existingMovie.Duration + 30);

                DateTime candidateEnd =
                    candidateStart.AddMinutes(
                        newMovieDuration + 30);

                if (candidateStart < existingEnd &&
                    candidateEnd > existingStart)
                {
                    candidateStart = existingEnd;
                }
            }

            return TimeOnly.FromDateTime(candidateStart);
        }

        private void LoadAvailableHalls()
        {
            AvailableHalls.Clear();

            if (SelectedCinema == null)
                return;

            foreach (Hall hall in Halls)
            {
                if (hall.CinemaId == SelectedCinema.Id)
                {
                    AvailableHalls.Add(hall);
                }
            }

            SelectedHall = null;
        }

        private bool CanSaveScreeningChanges()
        {
            return SelectedScreening != null;
        }

        private void BookScreening()
        {
            if (SelectedScreening == null)
            {
                StatusMessage = "Vælg en forestilling først.";
                return;
            }

            Hall? hall = Halls.FirstOrDefault(h => h.Id == SelectedScreening.HallId);
            if (hall == null)
            {
                StatusMessage = "Sal kunne ikke findes.";
                return;
            }

            int seatsLeft = _bookingService.GetSeatsLeft(
                SelectedScreening.ScreeningId,
                hall.Capacity);

            // Show booking dialog
            BookingDialog bookingView = new BookingDialog();
            var bookingVm = new BookingViewModel();
            bookingVm.SeatsLeft = seatsLeft;
            bookingView.DataContext = bookingVm;
            bookingView.Owner = System.Windows.Application.Current?.MainWindow;

            bool? result = bookingView.ShowDialog();
            if (result != true)
            {
                StatusMessage = "Booking annulleret.";
                return;
            }
            Booking newBooking;
            try
            {
                // additional safety checks and conversion
                if (string.IsNullOrWhiteSpace(bookingVm.Email) || !bookingVm.Email.Contains("@"))
                    throw new FormatException("Email skal indeholde '@'.");

                if (string.IsNullOrWhiteSpace(bookingVm.PhoneNumber) || !System.Text.RegularExpressions.Regex.IsMatch(bookingVm.PhoneNumber, "^[0-9]+$"))
                    throw new FormatException("Telefonnummer må kun indeholde tal.");

                newBooking = bookingVm.ToBooking();
            }
            catch (FormatException fx)
            {
                StatusMessage = fx.Message;
                return;
            }
            catch (Exception)
            {
                StatusMessage = "Der opstod en fejl ved oprettelse af booking.";
                return;
            }
            BookingCreationResult creationResult = _bookingService.CreateBooking(
                newBooking,
                SelectedScreening.ScreeningId,
                hall.Capacity);

            StatusMessage = creationResult.Message;
            if (!creationResult.IsSuccess)
                return;

            // Notify listeners so UI (StartWindow etc.) can refresh immediately
            TheMovies.WPF.Helpers.BookingNotifier.RaiseBookingChanged(newBooking.ScreeningId);
        }

    }
}
