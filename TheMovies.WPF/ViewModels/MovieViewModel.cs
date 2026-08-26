using System.Collections.ObjectModel;
using System.Windows.Input;
using TheMovies.Core.Models;
using TheMovies.Core.Repositories;


namespace TheMovies.WPF.ViewModels
{
    public class MovieViewModel : ViewModelBase
    {

        private readonly FileMovieRepository _repository;

        private string _title;
        private string _duration;
        private string _genre;
        private string _director;
        private DateTime _releasedate = DateTime.Today;
        private string _statusMessage;

        private Movie? _selectedMovie;

        public Movie? SelectedMovie
        {
            get => _selectedMovie;
            set
            {
                _selectedMovie = value;
                OnPropertyChanged();
            }
        }


        public string StatusMessage
        {
            get => _statusMessage;
            set
            {
                _statusMessage = value;
                OnPropertyChanged();
            }
        }
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

        public string Director
        {
            get => _director;
            set
            {
                _director = value;
                OnPropertyChanged();
            }
        }

        public DateTime ReleaseDate
        {
            get => _releasedate;
            set
            {
                _releasedate = value;
                OnPropertyChanged();
            }
        }
        public ObservableCollection<Movie> Movies { get; set; }


        public MovieViewModel(FileMovieRepository repository)
        {

            _repository = repository;

            List<Movie> loadedMovies = _repository.LoadMovies();

            Movies = new ObservableCollection<Movie>(loadedMovies);

            RegisterMovieCommand = new RelayCommand(RegisterMovie);
            DeleteMovieCommand = new RelayCommand(DeleteMovie);

        }
        public ICommand RegisterMovieCommand { get; }
        public ICommand DeleteMovieCommand { get; }


        private void RegisterMovie(object parameter)
        {

            if (string.IsNullOrWhiteSpace(Title))
            {
                ShowMessage("Fejl", "Du skal indtaste en titel.");
                return;
            }

            if (!int.TryParse(Duration, out int duration) || duration <= 0)
            {
                ShowMessage("Fejl", "Varighed skal angives som et heltal tal i minutter.");
                return;
            }

            if (string.IsNullOrWhiteSpace(Genre))
            {
                ShowMessage("Fejl", "Du skal indtaste en genre.");
                return;
            }

            Movie movie = new Movie
            {
                Title = Title,
                Duration = duration,
                Genre = Genre,
                Director = Director,
                ReleaseDate = ReleaseDate
            };

            Movies.Add(movie);

            _repository.SaveMovies(Movies.ToList());

            ShowMessage($"{Title} registreret", "Filmen blev registreret.");

            // Return to default values after registration
            Title = "";
            Duration = "";
            Genre = "";
            Director = "";
            ReleaseDate = DateTime.Now;
        }

        private void DeleteMovie(object parameter)
        {
            if (SelectedMovie == null)
                return;

            Movie movieToDelete = SelectedMovie;

            Movies.Remove(movieToDelete);

            _repository.SaveMovies(Movies.ToList());
            ShowMessage($"{movieToDelete.Title} slettet", "Filmen blev slettet.");
            SelectedMovie = null; // Reset the selected movie after deletion
        }

        public event Action<string, string>? ShowMessageRequested;

        private void ShowMessage(string title, string message)
        {
            ShowMessageRequested?.Invoke(title, message);
        }
    }
}
