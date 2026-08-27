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

        public ICommand OpenCalendarCommand { get; }

        public event Action? OpenCalendarRequested;

        public MainViewModel()
        {
            FileMovieRepository movieRepository = new FileMovieRepository();
            _cinemaRepository = new FileCinemaRepository();
            FileHallRepository hallRepository = new FileHallRepository();

            MovieViewModel = new MovieViewModel(movieRepository);
            CinemaViewModel = new CinemaViewModel(cinemaRepository);
            HallViewModel = new HallViewModel(
                hallRepository,
                CinemaViewModel.Cinemas);

            OpenCalendarCommand = new RelayCommand(
                 => OpenCalendarRequested?.Invoke());
        }

        public CalendarViewModel CreateCalendarViewModel()
        {
            return new CalendarViewModel(_cinemaRepository);
        }
    }
}