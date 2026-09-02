using System.Windows;
using TheMovies.WPF.ViewModels;

namespace TheMovies.WPF.Views
{
    public partial class BookingsView : Window
    {
        public BookingsView()
        {
            InitializeComponent();
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is BookingsViewModel vm && vm.SelectedBooking != null)
            {
                vm.EditSelectedBooking();
            }
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is BookingsViewModel vm && vm.SelectedBooking != null)
            {
                vm.DeleteSelectedBooking();
            }
        }
    }
}
