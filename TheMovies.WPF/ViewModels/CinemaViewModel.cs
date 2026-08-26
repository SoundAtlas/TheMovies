using System.Collections.ObjectModel;
using System.Windows.Input;
using TheMovies.Core.Models;
using TheMovies.Core.Repositories;

namespace TheMovies.WPF.ViewModels
{
    public class CinemaViewModel : ViewModelBase
    {
        private readonly FileCinemaRepository _repository;

        private string _name;

        public string Name
        {
            get { return _name; }
            set { _name = value; OnPropertyChanged(); }
        }

        public ObservableCollection<Cinema> Cinemas { get; set; }

        public ICommand RegisterCinemaCommand { get; }

        public CinemaViewModel(FileCinemaRepository repository)
        {
            _repository = repository;

            // Load cinemas from the repository and initialize the ObservableCollection
            List<Cinema> loadedCinemas = _repository.LoadCinemas();
            Cinemas = new ObservableCollection<Cinema>(loadedCinemas);

            RegisterCinemaCommand = new RelayCommand(RegisterCinema);
        }

        public void RegisterCinema(object paramter)
        {
            if (string.IsNullOrWhiteSpace(Name))
            {
                ShowMessage("Fejl", "Du skal indtaste et navn for biografen.");
                return;
            }

            Cinema cinema = new Cinema()
            {
                Id = Cinemas.Count + 1,
                Name = Name,
            };

            Cinemas.Add(cinema);
            _repository.SaveCinemas(Cinemas.ToList());
            Name = ""; // Clear the input field after saving

        }

        public event Action<string, string>? ShowMessageRequested;

        private void ShowMessage(string title, string message)
        {
            ShowMessageRequested?.Invoke(title, message);
        }

    }
}
