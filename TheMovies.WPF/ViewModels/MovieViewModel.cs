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

        private string _director;
        public string Director
        {
            get => _director;
            set
            {
                _director = value;
                OnPropertyChanged();
            }
        }

        private DateTime _releasedate = DateTime.Today;
        public DateTime ReleaseDate
        {
            get => _releasedate;
            set
            {
                _releasedate = value;
                OnPropertyChanged();
            }
        }

        private string _statusMessage;
        public string StatusMessage
        {
            get => _statusMessage;
            set
            {
                _statusMessage = value;
                OnPropertyChanged();
            }
        }


        private Movie? _selectedMovie;
        public Movie? SelectedMovie
        {
            get => _selectedMovie;
            set
            {
                _selectedMovie = value;
                OnPropertyChanged();

                if (_selectedMovie == null)
                    return;

                Title = _selectedMovie.Title;
                Duration = _selectedMovie.Duration.ToString();
                Genre = _selectedMovie.Genre;
                Director = _selectedMovie.Director;
                ReleaseDate = _selectedMovie.ReleaseDate;

            }

        }


        public ObservableCollection<Movie> Movies { get; set; }

        public ICommand RegisterMovieCommand { get; }
        public ICommand DeleteMovieCommand { get; }
        public ICommand SaveMovieChangesCommand { get; }


        public MovieViewModel(FileMovieRepository repository)
        {

            _repository = repository;

            // Load movies from the repository and initialize the ObservableCollection
            List<Movie> loadedMovies = _repository.LoadMovies();
            Movies = new ObservableCollection<Movie>(loadedMovies);

            RegisterMovieCommand = new RelayCommand(RegisterMovie);
            DeleteMovieCommand = new RelayCommand(DeleteMovie, CanDeleteMovie);
            SaveMovieChangesCommand = new RelayCommand(SaveMovieChanges, CanSaveMovieChanges);

        }

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

            // Find the next available ID for the new movie
            int newId = 1;
            foreach (Movie existingMovie in Movies)
            {
                if (existingMovie.Id >= newId)
                {
                    newId = existingMovie.Id + 1;
                }
            }

            Movie movie = new Movie
            {
                Id = newId,
                Title = Title,
                Duration = duration,
                Genre = Genre,
                Director = Director,
                ReleaseDate = ReleaseDate
            };

            Movies.Add(movie);
            _repository.SaveMovies(Movies.ToList());

            StatusMessage = $"{Title} registreret";

            // Return to default values after registration
            ClearInputFields();

        }

        private void DeleteMovie(object parameter)
        {
            if (SelectedMovie == null)
                return;


            Movie movieToDelete = SelectedMovie;

            bool confirmed = ConfirmDelete("Bekræft sletning",
                $"Er du sikker på, at du vil slette filmen '{movieToDelete.Title}'?");

            if (!confirmed)
            {
                return;
            }

            Movies.Remove(movieToDelete);

            _repository.SaveMovies(Movies.ToList());
            ShowMessage($"{movieToDelete.Title} slettet", "Filmen blev slettet.");

            // Return to default values after deletion
            ClearInputFields();
            SelectedMovie = null; // Reset the selected movie after deletion

        }



        private void SaveMovieChanges(object parameter)
        {
            if (SelectedMovie == null)
                return;
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

            // Find the index of the selected movie in the collection
            int index = Movies.IndexOf(SelectedMovie);
            // Create a new Movie object with the updated values
            Movie updatedMovie = new Movie
            {
                Id = SelectedMovie.Id,
                Title = Title,
                Duration = duration,
                Genre = Genre,
                Director = Director,
                ReleaseDate = ReleaseDate
            };

            // Update the movie in the collection at the found index
            Movies[index] = updatedMovie;


            _repository.SaveMovies(Movies.ToList());
            StatusMessage = $"{Title} blev opdateret";
            // Reset the input fields after saving changes
            ClearInputFields();
            SelectedMovie = null; // Reset the selected movie after editing

        }

        // Command CanExecute methods
        private bool CanDeleteMovie(object? parameter)
        {
            return SelectedMovie != null;
        }

        private bool CanSaveMovieChanges(object? parameter)
        {
            return SelectedMovie != null;
        }

        public event Action<string, string>? ShowMessageRequested;

        private void ShowMessage(string title, string message)
        {
            ShowMessageRequested?.Invoke(title, message);
        }

        public event Func<string, string, bool>? ConfirmDeleteRequested;

        private bool ConfirmDelete(string title, string message)
        {
            if (ConfirmDeleteRequested != null)
            {
                return ConfirmDeleteRequested.Invoke(title, message);
            }
            return false;
        }

        private void ClearInputFields()
        {
            Title = "";
            Duration = "";
            Genre = "";
            Director = "";
            ReleaseDate = DateTime.Now;
        }
    }
}
