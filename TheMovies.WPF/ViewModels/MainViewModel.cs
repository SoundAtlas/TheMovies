using TheMovies.Core.Repositories;

namespace TheMovies.WPF.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        public MovieViewModel MovieViewModel { get; }
        public CinemaViewModel CinemaViewModel { get; }

        public MainViewModel()
        {
            FileMovieRepository movieRepository = new FileMovieRepository();
            FileCinemaRepository cinemaRepository = new FileCinemaRepository();

            MovieViewModel = new MovieViewModel(movieRepository);
            CinemaViewModel = new CinemaViewModel(cinemaRepository);

        }

    }
}
