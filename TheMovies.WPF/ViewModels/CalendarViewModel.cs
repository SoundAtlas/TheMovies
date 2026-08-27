using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using TheMovies.Core.Models;
using TheMovies.Core.Repositories;

namespace TheMovies.WPF.ViewModels
{
    // Ét felt i kalendergitteret — én dag. 42 stk (6 rækker × 7 kolonner).
    public class DayDisplay
    {
        public DateOnly Date { get; set; }
        public bool HasScreenings { get; set; }     // Prik i UI hvis true altså så hvis der er forestillinger på den dag, kommer der en prik i UI
        public bool IsInCurrentMonth { get; set; }  // Nedtonet hvis false så hvis dagen ikke er i den måned der vises i kalenderen, skal den være nedtonet
                                                    // så det tydeligt fremgår at det er en dag i en anden måned, men stadig en del af kalendergitter
                                                    // (det er en layoutmæssighed).
    }

    // Én linje i dagsoversigten til højre for kalendergitteret.
    // simpel og med de tre ting vi skal vise: tidspunkt, filmtitel og biografsal
    public class ScreeningDisplay
    {
        public string TimeSlot { get; set; }   // fx "18:00"
        public string MovieTitle { get; set; }
        public string HallName { get; set; }
        public bool IsPremiere { get; set; }
    }

    public class CalendarViewModel : INotifyPropertyChanged // bruges til at binde data til UI i WPF.
                                                            // INotifyPropertyChanged interface gør det muligt for ViewModel
                                                            // at notificere UI når en property ændres, så UI kan opdatere sig selv.
    {
        private readonly FileCinemaRepository _cinemaRepository;
        private readonly FileMovieRepository _movieRepository;
        private readonly FileHallRepository _hallRepository;

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
        private ObservableCollection<Cinema> _cinemas;
        public ObservableCollection<Cinema> Cinemas
        {
            get => _cinemas;
            set { _cinemas = value; OnPropertyChanged(); }
        }

        // Den biograf brugeren har valgt
        private Cinema _selectedCinema;
        public Cinema SelectedCinema
        {
            get => _selectedCinema;
            set
            {
                _selectedCinema = value;
                OnPropertyChanged();
                // Skift af biograf og skift af måned skal begge opdatere kalendergitteret, så vi kalder BuildCalendar() her.
                BuildCalendar();
            }
        }

        private ObservableCollection<DayDisplay> _days;
        public ObservableCollection<DayDisplay> Days
        {
            get => _days;
            set { _days = value; OnPropertyChanged(); }
        }

        private DayDisplay _selectedDay;
        public DayDisplay SelectedDay
        {
            get => _selectedDay;
            set
            {
                _selectedDay = value;
                OnPropertyChanged();

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

        public string MonthLabel => $"{MonthNames[_month - 1]} {_year}";

        // Dagsoversigten: forestillinger for valgt dag i valgt biograf
        private ObservableCollection<ScreeningDisplay> _screenings;
        public ObservableCollection<ScreeningDisplay> Screenings
        {
            get => _screenings;
            set { _screenings = value; OnPropertyChanged(); }
        }

        public ICommand PreviousMonthCommand { get; }
        public ICommand NextMonthCommand { get; }

        public CalendarViewModel(FileCinemaRepository cinemaRepository, FileMovieRepository movieRepository, FileHallRepository hallRepository) // her injecter vi repository'et ind i viewmodel'en via konstruktøren
        {
            _cinemaRepository = cinemaRepository;
            _movieRepository = movieRepository;
            _hallRepository = hallRepository;

            _year = DateTime.Today.Year;
            _month = DateTime.Today.Month;

            PreviousMonthCommand = new RelayCommand(PreviousMonth);
            NextMonthCommand = new RelayCommand(NextMonth);

            Screenings = new ObservableCollection<ScreeningDisplay>();

            List<Cinema> loadedCinemas = _cinemaRepository.LoadCinemas();
            Cinemas = new ObservableCollection<Cinema>(loadedCinemas);

            // Sæt feltet direkte (ikke via property), så vi ikke bygger kalenderen to gange. Hvis vi sætter via property
            // kaldes den to gange: én gang her og én gang i SelectedCinema's setter.,
            // men ved at sætte direkte i feltet når vi bygger kalenderen her, så kaldes den kun én gang.
            _selectedCinema = Cinemas.FirstOrDefault();

            BuildCalendar();
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
                    TimeSlot = $"{screening.StartTime:HH\\:mm}",
                    MovieTitle = movie?.Title ?? "Ukendt film",
                    HallName = hall?.Name ?? "Ukendt sal"
                });
            }

            displays = displays
                .OrderBy(d => d.TimeSlot)
                .ToList();

            Screenings = new ObservableCollection<ScreeningDisplay>(displays);
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
