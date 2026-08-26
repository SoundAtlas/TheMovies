using System.Collections.ObjectModel;
using TheMovies.Core.Models;
using TheMovies.Core.Repositories;

namespace TheMovies.WPF.ViewModels
{
    public class HallViewModel : ViewModelBase
    {
        private readonly FileHallRepository _repository;

        private string _Name;

        public string Name
        {
            get { return _Name; }
            set { _Name = value; OnPropertyChanged(); }
        }

        public ObservableCollection<Hall> Halls { get; set; }

        public HallViewModel(FileHallRepository repository)
        {
            _repository = repository;

            // Load halls from the repository and initialize the ObservableCollection
            List<Hall> loadedHalls = _repository.LoadHalls();
            Halls = new ObservableCollection<Hall>(loadedHalls);
        }


    }
}
