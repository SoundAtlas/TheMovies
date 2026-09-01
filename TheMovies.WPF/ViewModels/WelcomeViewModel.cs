using System.Collections.ObjectModel;
using System.Windows.Input;
using TheMovies.Core.Interfaces;
using TheMovies.Core.Models;

namespace TheMovies.WPF.ViewModels
{
    // ViewModel til velkomstskærmen - viser de registrerede biografer, så man kan vælge én.
    public class WelcomeViewModel
    {
        private readonly ICinemaRepository _cinemaRepository;

        // Alle biografer man kan vælge imellem - én knap pr. biograf i WelcomeWindow.
        public ObservableCollection<Cinema> Cinemas { get; }

        public ICommand SelectCinemaCommand { get; }
        public ICommand ManageCinemasCommand { get; }

        // WelcomeWindow lytter på denne og åbner startskærmen for den valgte biograf.
        public event Action<Cinema>? CinemaSelected;

        // WelcomeWindow lytter på denne og åbner ManageCinemasWindow.
        public event Action? ManageCinemasRequested;

        public WelcomeViewModel(ICinemaRepository cinemaRepository)
        {
            _cinemaRepository = cinemaRepository;

            List<Cinema> loadedCinemas = _cinemaRepository.LoadCinemas();
            Cinemas = new ObservableCollection<Cinema>(loadedCinemas);

            SelectCinemaCommand = new RelayCommand(SelectCinema);
            ManageCinemasCommand = new RelayCommand(() => ManageCinemasRequested?.Invoke());
        }

        private void SelectCinema(object? parameter)
        {
            // Parameter kommer fra CommandParameter i XAML - selve Cinema-objektet knappen viser.
            if (parameter is Cinema selectedCinema)
            {
                CinemaSelected?.Invoke(selectedCinema);
            }
        }
    }
}
