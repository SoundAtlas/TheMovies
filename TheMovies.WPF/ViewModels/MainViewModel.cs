using System.Windows.Input;
using TheMovies.Core.Repositories;

namespace TheMovies.WPF.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        public MovieViewModel MovieViewModel { get; }
        public CinemaViewModel CinemaViewModel { get; }
        public HallViewModel HallViewModel { get; }

        private readonly FileCinemaRepository _cinemaRepository;
        private readonly FileMovieRepository _movieRepository;
        private readonly FileHallRepository _hallRepository;

        public ICommand OpenCalendarCommand { get; }

        public event Action? OpenCalendarRequested;

        public MainViewModel()
        {
            _movieRepository = new FileMovieRepository();
            _cinemaRepository = new FileCinemaRepository();
            _hallRepository = new FileHallRepository();

            MovieViewModel = new MovieViewModel(_movieRepository, _cinemaRepository);
            CinemaViewModel = new CinemaViewModel(_cinemaRepository, _hallRepository);

            HallViewModel = new HallViewModel(
                _hallRepository,
                _cinemaRepository,
                CinemaViewModel.Cinemas);

            OpenCalendarCommand = new RelayCommand(
                () => OpenCalendarRequested?.Invoke());
        }

        public CalendarViewModel CreateCalendarViewModel()
        {
            return new CalendarViewModel(
                _cinemaRepository,
                _movieRepository,
                _hallRepository);
        }
    }
}