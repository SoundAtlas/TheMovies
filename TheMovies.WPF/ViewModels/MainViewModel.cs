using TheMovies.Core.Repositories;

namespace TheMovies.WPF.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        public MovieViewModel AddMovieViewModel { get; }

        public MainViewModel()
        {
            FileMovieRepository repository = new FileMovieRepository();
            AddMovieViewModel = new MovieViewModel(repository);
        }

    }
}
