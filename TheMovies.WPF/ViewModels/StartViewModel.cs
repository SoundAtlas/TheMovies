using System.Collections.ObjectModel;
using System.Windows.Input;
using TheMovies.Core.Interfaces;
using TheMovies.Core.Models;
using TheMovies.WPF.DisplayModels;

namespace TheMovies.WPF.ViewModels
{
    // ViewModel til startskærmen for én biograf. Viser dagens forestillinger og styrer
    // knapperne til kalender, opret film og skift biograf.
    public class StartViewModel : ViewModelBase
    {
        private readonly ICinemaRepository _cinemaRepository;
        private readonly IMovieRepository _movieRepository;
        private readonly IHallRepository _hallRepository;

        private static readonly string[] DayNames =
        {
            "Mandag", "Tirsdag", "Onsdag", "Torsdag", "Fredag", "Lørdag", "Søndag"
        };
        private static readonly string[] MonthNames =
        {
            "januar", "februar", "marts", "april", "maj", "juni",
            "juli", "august", "september", "oktober", "november", "december"
        };

        public Cinema SelectedCinema { get; }

        public string TodayLabel { get; }

        // Dagens forestillinger, sorteret efter tid. RefreshTodaysScreenings() opdaterer
        // denne liste i stedet for at lave en ny, så XAML'ens binding ikke skal genoprettes.
        public ObservableCollection<ScreeningDisplay> TodaysScreenings { get; }

        // Til XAML: vis enten listen eller "ingen forestillinger i dag"-beskeden
        // Ligger ikke bare på TodaysScreenings.Count, fordi vi skal kunne bruge PropertyChanged
        // manuelt for dem, når RefreshTodaysScreenings() opdaterer listens indhold.
        public bool HasScreeningsToday => TodaysScreenings.Count > 0;
        public bool HasNoScreeningsToday => TodaysScreenings.Count == 0;

        public ICommand OpenCalendarCommand { get; }
        public ICommand CreateMovieCommand { get; }
        public ICommand ChangeCinemaCommand { get; }

        public event Action? OpenCalendarRequested;
        public event Action? CreateMovieRequested;
        public event Action? ChangeCinemaRequested;

        public StartViewModel(
            Cinema selectedCinema,
            ICinemaRepository cinemaRepository,
            IMovieRepository movieRepository,
            IHallRepository hallRepository)
        {
            SelectedCinema = selectedCinema;

            _cinemaRepository = cinemaRepository;
            _movieRepository = movieRepository;
            _hallRepository = hallRepository;

            TodayLabel = BuildTodayLabel();
            TodaysScreenings = new ObservableCollection<ScreeningDisplay>(LoadTodaysScreenings(SelectedCinema));

            OpenCalendarCommand = new RelayCommand(() => OpenCalendarRequested?.Invoke());
            CreateMovieCommand = new RelayCommand(() => CreateMovieRequested?.Invoke());
            ChangeCinemaCommand = new RelayCommand(() => ChangeCinemaRequested?.Invoke());
        }

        private string BuildTodayLabel()
        {
            DateOnly today = DateOnly.FromDateTime(DateTime.Today);

            // DayOfWeek starter på søndag (0) 
            int dayIndex = ((int)today.DayOfWeek == 0) ? 6 : (int)today.DayOfWeek - 1;

            return $"{DayNames[dayIndex]} d. {today.Day}. {MonthNames[today.Month - 1]}";
        }

        // Tager biografen som parameter (i stedet for bare at bruge SelectedCinema), så samme
        // metode kan bruges både ved opstart og ved en frisk genindlæsning fra disk (så den opdatere ved tilbageklik).
        private List<ScreeningDisplay> LoadTodaysScreenings(Cinema cinema)
        {
            DateOnly today = DateOnly.FromDateTime(DateTime.Today);

            List<Movie> movies = _movieRepository.LoadMovies();
            List<Hall> halls = _hallRepository.LoadHalls();

            List<ScreeningDisplay> result = new List<ScreeningDisplay>();

            foreach (Screening screening in cinema.Screenings)
            {
                if (screening.Date != today)
                    continue;

                Movie? movie = movies.FirstOrDefault(m => m.Id == screening.MovieId);
                Hall? hall = halls.FirstOrDefault(h => h.Id == screening.HallId);

                result.Add(new ScreeningDisplay
                {
                    ScreeningId = screening.Id,
                    MovieId = screening.MovieId,
                    HallId = screening.HallId,
                    Date = screening.Date,
                    StartTime = screening.StartTime,

                    TimeSlot = $"{screening.StartTime:HH\\:mm}",
                    MovieTitle = movie?.Title ?? "Ukendt film",
                    HallName = hall?.Name ?? "Ukendt sal",

                    IsPremiere = movie != null &&
                                 screening.Date == DateOnly.FromDateTime(movie.ReleaseDate)
                });
            }

            return result.OrderBy(s => s.StartTime).ToList();
        }

        // Genindlæser dagens forestillinger disk. Nødvendig fordi "Opret film" og
        // "Se kalender" åbner deres eget repository-objekt og gemmer via det - så denne
        // ViewModel's SelectedCinema.Screenings (indlæst da man gik ind på den) opdateres
        // ikke automatisk, så selvom man lige har oprettet en forestilling til den nuværende dag
        // viste den det først nrå man gik helt ud af dne valgte biograf og ind igen.
        // StartWindow kalder den her, når man går tilbage til vinduet.
        public void RefreshTodaysScreenings()
        {
            List<Cinema> currentCinemas = _cinemaRepository.LoadCinemas();
            Cinema? refreshedCinema = currentCinemas.FirstOrDefault(c => c.Id == SelectedCinema.Id);

            if (refreshedCinema == null)
                return;

            List<ScreeningDisplay> refreshed = LoadTodaysScreenings(refreshedCinema);

            TodaysScreenings.Clear();
            foreach (ScreeningDisplay screening in refreshed)
                TodaysScreenings.Add(screening);

            // TodaysScreenings er en ObservableCollection, så listen i XAML opdaterer sig selv.
            // HasScreeningsToday/HasNoScreeningsToday er derimod almindelige properties, så de
            // skal have besked eksplicit for at vise/skjule "ingen forestillinger i dag"-teksten korrekt.
            OnPropertyChanged(nameof(HasScreeningsToday));
            OnPropertyChanged(nameof(HasNoScreeningsToday));
        }

        // Bygger kalenderen for hele The Movies, men starter med den biograf man kom fra.
        public CalendarViewModel CreateCalendarViewModel()
        {
            CalendarViewModel calendarViewModel = new CalendarViewModel(
                _cinemaRepository,
                _movieRepository,
                _hallRepository);

            Cinema? matchingCinema = calendarViewModel.Cinemas
                .FirstOrDefault(c => c.Id == SelectedCinema.Id);

            if (matchingCinema != null)
                calendarViewModel.SelectedCinema = matchingCinema;

            return calendarViewModel;
        }
    }
}
