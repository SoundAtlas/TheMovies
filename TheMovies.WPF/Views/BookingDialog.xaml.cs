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
            // Validate email contains '@' and phone contains only digits before closing
            if (DataContext is BookingViewModel vm)
            {
                if (string.IsNullOrWhiteSpace(vm.Email) || !vm.Email.Contains("@"))
                {
                    MessageBox.Show("Indtast en gyldig emailadresse som indeholder '@'.", "Ugyldig email", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(vm.PhoneNumber) || !Regex.IsMatch(vm.PhoneNumber, "^[0-9]+$"))
                {
                    MessageBox.Show("Telefonnummer må kun indeholde tal.", "Ugyldigt telefonnummer", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }

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
