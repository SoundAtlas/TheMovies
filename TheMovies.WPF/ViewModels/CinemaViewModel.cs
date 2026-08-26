using System.Collections.ObjectModel;
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

        public CinemaViewModel(FileCinemaRepository repository)
        {
            _repository = repository;

            // Load cinemas from the repository and initialize the ObservableCollection
            List<Cinema> loadedCinemas = _repository.LoadCinemas();
            Cinemas = new ObservableCollection<Cinema>(loadedCinemas);
        }

    }
}
