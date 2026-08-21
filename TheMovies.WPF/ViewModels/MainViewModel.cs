namespace TheMovies.WPF.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        public AddMovieViewModel AddMovieViewModel { get; }

        public MainViewModel()
        {
            AddMovieViewModel = new AddMovieViewModel();
        }

    }
}
