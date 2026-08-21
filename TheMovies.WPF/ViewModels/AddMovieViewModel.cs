using System.Collections.ObjectModel;
using System.Windows.Input;
using TheMovies.Core.Models;
using TheMovies.Core.Repositories;


namespace TheMovies.WPF.ViewModels
{
    public class AddMovieViewModel : ViewModelBase
    {

        private readonly FileMovieRepository _repository;

        private string _title;
        private string _duration;
        private string _genre;

        public string Title
        {
            get => _title;
            set
            {
                _title = value;
                OnPropertyChanged();
            }
        }

        public string Duration
        {
            get => _duration;
            set
            {
                _duration = value;
                OnPropertyChanged();
            }
        }
        public string Genre
        {
            get => _genre;
            set
            {
                _genre = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Movie> Movies { get; set; }

        public ICommand RegisterMovieCommand { get; }

        public AddMovieViewModel(FileMovieRepository repository)
        {
            _repository = repository;

            List<Movie> loadedMovies = _repository.LoadMovies();

            Movies = new ObservableCollection<Movie>(loadedMovies);

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

            _repository.SaveMovies(Movies.ToList());
        }

    }
}
