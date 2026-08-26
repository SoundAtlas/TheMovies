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
            mainViewModel.AddMovieViewModel.ShowMessageRequested += ShowMessage;
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

        private void OpenCalendar()
        {
            MainViewModel vm = (MainViewModel)DataContext;
            CalendarView calendarView = new CalendarView(vm.CreateCalendarViewModel());
            calendarView.Show();
        }
    }
}