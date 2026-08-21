using System.Windows.Input;
using TheMovies.Core.Models;


namespace TheMovies.WPF.ViewModels
{
    public class AddMovieViewModel : ViewModelBase
    {



        private string _title;

        public string Title
        {
            get => _title;
            set
            {
                _title = value;
                OnPropertyChanged();
            }
        }

        private string _duration;
        public string Duration
        {
            get => _duration;
            set
            {
                _duration = value;
                OnPropertyChanged();
            }
        }

        private string _genre;
        public string Genre
        {
            get => _genre;
            set
            {
                _genre = value;
                OnPropertyChanged();
            }
        }

        public List<Movie> Movies { get; set; }

        public ICommand RegisterMovieCommand { get; }

        public AddMovieViewModel()
        {
            Movies = new List<Movie>();

            RegisterMovieCommand = new RelayCommand(RegisterMovie);
        }


        private void RegisterMovie(object parameter)
        {
            Movie movie = new Movie
            {
                Title = Title,
                Duration = int.Parse(Duration),
                Genre = Genre
            };

            Movies.Add(movie);
        }

    }
}
