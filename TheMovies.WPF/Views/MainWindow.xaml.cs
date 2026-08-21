using System.Windows;
using TheMovies.WPF.ViewModels;


namespace TheMovies.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            MainViewModel mainViewModel = new MainViewModel();
            DataContext = mainViewModel;
            mainViewModel.AddMovieViewModel.ShowMessageRequested += ShowMessage;
        }

        private void ShowMessage(string title, string message)
        {
            MessageBox.Show(
                message,
                title,
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }
    }
}