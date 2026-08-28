using System.Windows;
using TheMovies.WPF.ViewModels;
using TheMovies.WPF.Views;


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
            mainViewModel.MovieViewModel.ShowMessageRequested += ShowMessage;
            mainViewModel.MovieViewModel.ConfirmDeleteRequested += ConfirmDelete;

            mainViewModel.CinemaViewModel.ShowMessageRequested += ShowMessage;
            mainViewModel.CinemaViewModel.ConfirmDeleteRequested += ConfirmDelete;

            mainViewModel.HallViewModel.ConfirmDeleteRequested += ConfirmDelete;
            mainViewModel.OpenCalendarRequested += OpenCalendar;
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
        private void OpenCalendar()
        {
            MainViewModel vm = (MainViewModel)DataContext;
            CalendarView calendarView = new CalendarView(vm.CreateCalendarViewModel());
            calendarView.Show();
        }
    }
}