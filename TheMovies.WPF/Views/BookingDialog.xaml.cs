using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using TheMovies.WPF.ViewModels;

namespace TheMovies.WPF.Views
{
    public partial class BookingDialog : Window
    {
        public BookingDialog()
        {
            InitializeComponent();
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void BookingAmount_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // allow only digits
            e.Handled = !Regex.IsMatch(e.Text, "^[0-9]+$");
        }
    }
}
