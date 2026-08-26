using TheMovies.Core.Repositories;

namespace TheMovies.WPF.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        public MovieViewModel MovieViewModel { get; }

        public MainViewModel()
        {
            FileMovieRepository movieRepository = new FileMovieRepository();
            MovieViewModel = new MovieViewModel(movieRepository);
        }

    }
}
