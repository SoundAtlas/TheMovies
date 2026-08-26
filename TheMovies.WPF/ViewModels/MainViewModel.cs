using System.Windows.Input;
using TheMovies.Core.Repositories;

namespace TheMovies.WPF.ViewModels
{
	public class MainViewModel : ViewModelBase
	{
		public AddMovieViewModel AddMovieViewModel { get; }

		private readonly FileCinemaRepository _cinemaRepository;

		public ICommand OpenCalendarCommand { get; }

		// MainWindow (View) "lytter" på dette event og åbner selve vinduet.
		// ViewModel må ikke selv oprette et Window — det ville bryde MVVM-adskillelsen.
		public event Action? OpenCalendarRequested;

		public MainViewModel()
		{
			FileMovieRepository movieRepository = new FileMovieRepository();
			AddMovieViewModel = new AddMovieViewModel(movieRepository);

			_cinemaRepository = new FileCinemaRepository();
			OpenCalendarCommand = new RelayCommand(() => OpenCalendarRequested?.Invoke());
		}

		public CalendarViewModel CreateCalendarViewModel()
		{
			return new CalendarViewModel(_cinemaRepository);
		}
	}
}
