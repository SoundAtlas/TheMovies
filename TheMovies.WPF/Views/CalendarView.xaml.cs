using System.Windows;
using TheMovies.WPF.ViewModels;

namespace TheMovies.WPF.Views
{
	public partial class CalendarView : Window
	{
		public CalendarView(CalendarViewModel viewModel)
		{
			InitializeComponent();
			DataContext = viewModel;
		}

		private void TilbageKnap_Click(object sender, RoutedEventArgs e)
		{
			this.Close();
		}
	}
}
