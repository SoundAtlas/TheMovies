using System.Collections.ObjectModel;
using System.Windows.Input;
using TheMovies.Core.Interfaces;
using TheMovies.Core.Models;

namespace TheMovies.WPF.ViewModels
{
    public class HallViewModel : ViewModelBase
    {
        private readonly IHallRepository _hallRepository;
        private readonly ICinemaRepository _cinemaRepository;
        private string _Name = string.Empty;
        private int _Capacity;
        private Cinema? _selectedCinema;
        private Hall? _selectedHall;
        private string _statusMessage = string.Empty;

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

        public int Capacity
        {
            get { return _Capacity; }
            set { _Capacity = value; OnPropertyChanged(); }
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

        public HallViewModel(IHallRepository hallRepository, ICinemaRepository cinemaRepository,
            ObservableCollection<Cinema> cinemas)
        {
            _hallRepository = hallRepository;
            _cinemaRepository = cinemaRepository;
            Cinemas = cinemas;

            // Load halls from the repository and initialize the ObservableCollection
            List<Hall> loadedHalls = _hallRepository.LoadHalls();

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

        private void RegisterHall(object? parameter)
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
                CinemaName = SelectedCinema.Name,
                Capacity = Capacity

            };

            Halls.Add(hall);

            _hallRepository.SaveHalls(Halls.ToList());

            StatusMessage = $"Sal '{Name}' er blevet registreret.";

            Name = "";
            SelectedCinema = null;

        }

        private void DeleteHall(object? parameter)
        {
            if (SelectedHall == null)
                return;

            Hall hallToDelete = SelectedHall;

            bool confirmed = Confirm(
                "Slet sal",
                $"Er du sikker på, at du vil slette {hallToDelete.Name} i {hallToDelete.CinemaName}? " +
                "Alle forestillinger i salen bliver også slettet.");

            if (!confirmed)
                return;

            List<Cinema> cinemas = _cinemaRepository.LoadCinemas();
            // Remove all screenings of the hall from all cinemas
            foreach (Cinema cinema in cinemas)
            {
                cinema.Screenings.RemoveAll(
                    screening => screening.HallId == hallToDelete.Id);
            }

            _cinemaRepository.SaveCinemas(cinemas);

            Halls.Remove(hallToDelete);

            _hallRepository.SaveHalls(Halls.ToList());

            StatusMessage = $"Sal '{hallToDelete.Name}' og dens forestillinger blev slettet.";

            Name = "";
            SelectedCinema = null;
            SelectedHall = null;
        }


        private void SaveHallChanges(object? parameter)
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
                CinemaName = SelectedCinema.Name,
                Capacity = Capacity
            };

            Halls[index] = updatedHall;

            _hallRepository.SaveHalls(Halls.ToList());

            StatusMessage = $"Sal '{Name}' er blevet opdateret.";
            Name = "";
            SelectedCinema = null;
            SelectedHall = null;
            Capacity = 0;

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
