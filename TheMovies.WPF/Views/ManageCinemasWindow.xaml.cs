using System.Windows;
using TheMovies.Core.Repositories;
using TheMovies.WPF.ViewModels;

namespace TheMovies.WPF.Views
{
    // Side til at oprette, redigere og slette biografer og sale. Genbruger CinemaViewModel
    // og HallViewModel uændret - de var allerede fuldt implementeret i MainWindow, så her
    // er det kun selve skærmen omkring dem, der er ny.
    public partial class ManageCinemasWindow : Window
    {
        public ManageCinemasWindow()
        {
            InitializeComponent();

            FileCinemaRepository cinemaRepository = new FileCinemaRepository();
            FileHallRepository hallRepository = new FileHallRepository();

            ManageCinemasViewModel viewModel = new ManageCinemasViewModel(cinemaRepository, hallRepository);
            DataContext = viewModel;

            // Samme mønster som MainWindow: ViewModel'erne beder om en MessageBox via events.
            viewModel.CinemaViewModel.ShowMessageRequested += ShowMessage;
            viewModel.CinemaViewModel.ConfirmDeleteRequested += ConfirmDelete;
            viewModel.HallViewModel.ConfirmDeleteRequested += ConfirmDelete;
        }

        private void ShowMessage(string title, string message)
        {
            MessageBox.Show(
                message,
                title,
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        private bool ConfirmDelete(string title, string message)
        {
            MessageBoxResult result = MessageBox.Show(
                message,
                title,
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            return result == MessageBoxResult.Yes;
        }

        private void TilbageKnap_Click(object sender, RoutedEventArgs e)
        {
            WelcomeWindow welcomeWindow = new WelcomeWindow();
            welcomeWindow.Show();

            Close();
        }
    }
}
