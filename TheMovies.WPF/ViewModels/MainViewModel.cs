using TheMovies.Core.Repositories;

namespace TheMovies.WPF.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        public AddMovieViewModel AddMovieViewModel { get; }

        public MainViewModel()
        {
            FileMovieRepository repository = new FileMovieRepository();
            AddMovieViewModel = new AddMovieViewModel(repository);
        }

    }
}
