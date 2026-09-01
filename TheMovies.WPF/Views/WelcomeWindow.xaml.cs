using System.Windows;
using TheMovies.Core.Models;
using TheMovies.Core.Repositories;
using TheMovies.WPF.ViewModels;

namespace TheMovies.WPF.Views
{
    // Det første vindue appen viser (sat op i App.xaml). Her vælger man biograf og
    // kommer videre til StartWindow - eller går til biograf-administration.
    public partial class WelcomeWindow : Window
    {
        public WelcomeWindow()
        {
            InitializeComponent();

            // Ingen fælles dependency injection i appen, så vinduet opretter selv sit repository.
            FileCinemaRepository cinemaRepository = new FileCinemaRepository();

            WelcomeViewModel welcomeViewModel = new WelcomeViewModel(cinemaRepository);
            DataContext = welcomeViewModel;

            welcomeViewModel.CinemaSelected += OnCinemaSelected;
            welcomeViewModel.ManageCinemasRequested += OnManageCinemasRequested;
        }

        // En biograf blev valgt: åbner dens startskærm og lukker velkomstskærmen.
        private void OnCinemaSelected(Cinema selectedCinema)
        {
            StartWindow startWindow = new StartWindow(selectedCinema);
            startWindow.Show();

            Close();
        }

        // "Administrer biografer" blev trykket: åbner den side, og lukker velkomstskærmen
        // imens - ManageCinemasWindow sender én tilbage hertil igen via "Tilbage".
        private void OnManageCinemasRequested()
        {
            ManageCinemasWindow manageCinemasWindow = new ManageCinemasWindow();
            manageCinemasWindow.Show();

            Close();
        }
    }
}
