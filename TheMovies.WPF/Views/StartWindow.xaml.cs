using System.Windows;
using TheMovies.Core.Models;
using TheMovies.Core.Repositories;
using TheMovies.WPF.ViewModels;

namespace TheMovies.WPF.Views
{
    // Startskærmen for én biograf. Åbnes fra WelcomeWindow når man har valgt hvilken biograf
    // man arbejder i. Herfra kommer man videre til kalenderen, opret film, eller skift biograf.
    public partial class StartWindow : Window
    {
        public StartWindow(Cinema selectedCinema)
        {
            InitializeComponent();

            FileCinemaRepository cinemaRepository = new FileCinemaRepository();
            FileMovieRepository movieRepository = new FileMovieRepository();
            FileHallRepository hallRepository = new FileHallRepository();

            StartViewModel startViewModel = new StartViewModel(
                selectedCinema,
                cinemaRepository,
                movieRepository,
                hallRepository);

            DataContext = startViewModel;

            startViewModel.OpenCalendarRequested += OnOpenCalendarRequested;
            startViewModel.CreateMovieRequested += OnCreateMovieRequested;
            startViewModel.ChangeCinemaRequested += OnChangeCinemaRequested;

            // "Opret film" og "Se kalender" åbner deres eget vindue oven på dette, uden at
            // lukke det. Når man klikker tilbage til dette vindue, får det fokus igen - det
            // er signalet til at hente dagens forestillinger friskt, i tilfælde af at man lige
            // har oprettet en ny forestilling til i dag i et af de andre vinduer.
            Activated += (sender, e) => startViewModel.RefreshTodaysScreenings();
        }

        private void OnOpenCalendarRequested()
        {
            StartViewModel viewModel = (StartViewModel)DataContext;

            CalendarView calendarView = new CalendarView(viewModel.CreateCalendarViewModel());
            calendarView.Show();
        }

        private void OnCreateMovieRequested()
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
        }

        // Tilbage til velkomstskærmen, så man kan vælge en anden biograf. Lukker denne
        // startskærm, så vi ikke samler på åbne vinduer.
        private void OnChangeCinemaRequested()
        {
            WelcomeWindow welcomeWindow = new WelcomeWindow();
            welcomeWindow.Show();

            Close();
        }
    }
}
