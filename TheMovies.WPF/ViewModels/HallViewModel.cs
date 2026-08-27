using System.Collections.ObjectModel;
using System.Windows.Input;
using TheMovies.Core.Models;
using TheMovies.Core.Repositories;

namespace TheMovies.WPF.ViewModels
{
    public class HallViewModel : ViewModelBase
    {
        private readonly FileHallRepository _repository;
        private string _Name;
        private Cinema? _selectedCinema;
        private Hall? _selectedHall;
        private string _statusMessage;

        public string StatusMessage
        {
            get { return _statusMessage; }
            set { _statusMessage = value; OnPropertyChanged(); }
        }


        public string Name
        {
            get { return _Name; }
            set { _Name = value; OnPropertyChanged(); }
        }

        public Hall? SelectedHall
        {
            get { return _selectedHall; }
            set
            {
                _selectedHall = value;
                OnPropertyChanged();

                if (_selectedHall == null)
                    return;

                Name = _selectedHall.Name;

                // Go through all cinemas to find the cinema that the selected hall belongs to
                foreach (Cinema cinema in Cinemas)
                {
                    // Compare the cinema's Id with the CinemaId stored on the selected hall
                    if (cinema.Id == _selectedHall.CinemaId)
                    {
                        // When the IDs match, select that cinema in the ComboBox
                        SelectedCinema = cinema;
                        break;
                    }
                }

            }
        }

        public Cinema? SelectedCinema
        {
            get { return _selectedCinema; }
            set
            {
                _selectedCinema = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Hall> Halls { get; set; }
        public ObservableCollection<Cinema> Cinemas { get; set; }

        public ICommand RegisterHallCommand { get; }
        public ICommand DeleteHallCommand { get; }
        public ICommand SaveHallChangesCommand { get; }

        public HallViewModel(FileHallRepository repository,
            ObservableCollection<Cinema> cinemas)
        {
            _repository = repository;
            Cinemas = cinemas;

            // Load halls from the repository and initialize the ObservableCollection
            List<Hall> loadedHalls = _repository.LoadHalls();

            // Go through each hall that was loaded from the JSON file
            foreach (Hall hall in loadedHalls)
            {
                // Go through each cinema to find the cinema that this hall belongs to
                foreach (Cinema cinema in Cinemas)
                {
                    // Compare the hall's CinemaId with the cinema's Id
                    if (hall.CinemaId == cinema.Id)
                    {
                        // When the IDs match, we have found the correct cinema
                        // Store its name on the hall so we can display it in the UI
                        hall.CinemaName = cinema.Name;
                        break; // Exit the inner loop since we found the matching cinema
                    }
                }
            }

            Halls = new ObservableCollection<Hall>(loadedHalls);

            RegisterHallCommand = new RelayCommand(RegisterHall);
            DeleteHallCommand = new RelayCommand(DeleteHall, CanDeleteHall);
            SaveHallChangesCommand = new RelayCommand(SaveHallChanges, CanSaveHallChanges);
        }

        private void RegisterHall(object paramter)
        {

            if (string.IsNullOrWhiteSpace(Name))
                return;

            if (SelectedCinema == null)
                return;

            int newId = 1;
            // Ensure the new ID is unique by checking existing halls
            foreach (Hall existingHall in Halls)
            {
                if (existingHall.Id >= newId)
                {
                    newId = existingHall.Id + 1;
                }
            }

            Hall hall = new Hall
            {
                Id = newId,
                Name = Name,
                CinemaId = SelectedCinema.Id,
                CinemaName = SelectedCinema.Name
            };

            Halls.Add(hall);

            _repository.SaveHalls(Halls.ToList());

            StatusMessage = $"Sal '{Name}' er blevet registreret.";

            Name = "";
            SelectedCinema = null;

        }

        private void DeleteHall(object parameter)
        {
            if (SelectedHall == null)
                return;

            Hall hallToDelete = SelectedHall;

            bool confirmed = Confirm(
                "Slet sal",
                $"Er du sikker på, at du vil slette {hallToDelete.Name} i {hallToDelete.CinemaName}?");

            if (!confirmed)
                return;

            Halls.Remove(hallToDelete);

            _repository.SaveHalls(Halls.ToList());

            SelectedHall = null;

        }


        private void SaveHallChanges(object parameter)
        {
            if (SelectedHall == null)
                return;

            if (string.IsNullOrWhiteSpace(Name))
                return;

            if (SelectedCinema == null)
                return;

            int index = Halls.IndexOf(SelectedHall);

            Hall updatedHall = new Hall
            {
                Id = SelectedHall.Id,
                Name = Name,
                CinemaId = SelectedCinema.Id,
                CinemaName = SelectedCinema.Name
            };

            Halls[index] = updatedHall;

            _repository.SaveHalls(Halls.ToList());

            StatusMessage = $"Sal '{Name}' er blevet opdateret.";
            Name = "";
            SelectedCinema = null;
            SelectedHall = null;

        }


        // Command CanExecute methods
        private bool CanDeleteHall(object? parameter)
        {
            return SelectedHall != null;
        }

        private bool CanSaveHallChanges(object? parameter)
        {
            return SelectedHall != null;
        }


        // Event to request confirmation for deletion
        public event Func<string, string, bool>? ConfirmDeleteRequested;

        private bool Confirm(string title, string message)
        {
            return ConfirmDeleteRequested?.Invoke(title, message) ?? false;
        }
    }

}
