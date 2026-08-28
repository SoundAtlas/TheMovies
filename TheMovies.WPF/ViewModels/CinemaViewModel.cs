using System.Collections.ObjectModel;
using System.Windows.Input;
using TheMovies.Core.Models;
using TheMovies.Core.Repositories;

namespace TheMovies.WPF.ViewModels
{
    public class CinemaViewModel : ViewModelBase
    {
        private readonly FileCinemaRepository _repository;
        private readonly FileHallRepository _hallRepository;

        private string _name;

        public string Name
        {
            get { return _name; }
            set { _name = value; OnPropertyChanged(); }
        }


        private string _statusMessage;

        public string StatusMessage
        {
            get { return _statusMessage; }
            set
            {
                _statusMessage = value;
                OnPropertyChanged();
            }
        }

        private Cinema? _selectedCinema;

        public Cinema? SelectedCinema
        {
            get { return _selectedCinema; }
            set
            {
                _selectedCinema = value;
                OnPropertyChanged();

                if (_selectedCinema == null)
                    return;

                Name = _selectedCinema.Name;
            }
        }


        public ObservableCollection<Cinema> Cinemas { get; set; }

        public ICommand RegisterCinemaCommand { get; }
        public ICommand DeleteCinemaCommand { get; }
        public ICommand SaveCinemaChangesCommand { get; }

        public CinemaViewModel(FileCinemaRepository repository, FileHallRepository hallRepository)
        {
            _repository = repository;
            _hallRepository = hallRepository;

            // Load cinemas from the repository and initialize the ObservableCollection
            List<Cinema> loadedCinemas = _repository.LoadCinemas();
            Cinemas = new ObservableCollection<Cinema>(loadedCinemas);

            RegisterCinemaCommand = new RelayCommand(RegisterCinema);
            DeleteCinemaCommand = new RelayCommand(DeleteCinema, CanDeleteCinema);
            SaveCinemaChangesCommand = new RelayCommand(SaveCinemaChanges, CanSaveCinemaChanges);
        }

        public void RegisterCinema(object paramter)
        {
            if (string.IsNullOrWhiteSpace(Name))
            {
                ShowMessage("Fejl", "Du skal indtaste et navn for biografen.");
                return;
            }


            int newId = 1;

            // Ensure the new ID is unique by checking existing cinemas
            foreach (var existingCinema in Cinemas)
            {
                if (existingCinema.Id >= newId)
                {
                    newId = existingCinema.Id + 1;
                }
            }

            Cinema cinema = new Cinema()
            {
                Id = newId,
                Name = Name,
            };

            Cinemas.Add(cinema);
            _repository.SaveCinemas(Cinemas.ToList());

            StatusMessage = $"Biograf '{Name}' er blevet registreret.";

            Name = ""; // Clear the input field after saving

        }

        public void DeleteCinema(object parameter)
        {
            if (SelectedCinema == null)
            {
                return;
            }

            List<Hall> halls = _hallRepository.LoadHalls();

            foreach (Hall hall in halls)
            {
                if (hall.CinemaId == SelectedCinema.Id)
                {
                    ShowMessage(
                        "Kan ikke slette biograf",
                        "Biografen kan ikke slettes, fordi den stadig har registrerede sale.");

                    return;
                }
            }

            Cinema cinemaToDelete = SelectedCinema;

            bool confirmed = ConfirmDelete("Bekræft sletning",
                $"Er du sikker på, at du vil slette biografen '{cinemaToDelete.Name}'?");


            if (!confirmed)
            {
                return;
            }

            Cinemas.Remove(cinemaToDelete);
            _repository.SaveCinemas(Cinemas.ToList());
            ShowMessage($"{cinemaToDelete.Name} slettet", "Biografen blev slettet.");

            Name = ""; // Clear the input field after deletion
            SelectedCinema = null; // Clear the selection after deletion

        }

        public void SaveCinemaChanges(object parameter)
        {
            if (SelectedCinema == null)
                return;

            if (string.IsNullOrWhiteSpace(Name))
                return;

            int index = Cinemas.IndexOf(SelectedCinema);

            Cinema updatedCinema = new Cinema
            {
                Id = SelectedCinema.Id,
                Name = Name,
                Screenings = SelectedCinema.Screenings
            };

            Cinemas[index] = updatedCinema;

            _repository.SaveCinemas(Cinemas.ToList());

            StatusMessage = $"Biograf '{Name}' er blevet opdateret.";
            // Return to default values after saving changes
            Name = "";
            SelectedCinema = null;
        }

        // Command CanExecute methods
        private bool CanDeleteCinema(object? parameter)
        {
            return SelectedCinema != null;
        }

        private bool CanSaveCinemaChanges(object? parameter)
        {
            return SelectedCinema != null;
        }


        // Events for showing messages and confirming deletions
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

    }
}
