using TheMovies.Core.Models;
using TheMovies.Core.Repositories;
using TheMovies.WPF.ViewModels;

namespace TheMovies.Tests
{
    [TestClass]
    public class CalendarViewModelTests
    {
        [TestMethod]
        public void RegisterScreening_WithValidInput_AddsScreening()
        {
            // Arrange
            string moviePath = "test_screening_movies.json";
            string cinemaPath = "test_screening_cinemas.json";
            string hallPath = "test_screening_halls.json";
            string bookingPath = "test_screening_bookings.json";

            DeleteFiles(moviePath, cinemaPath, hallPath, bookingPath);

            FileMovieRepository movieRepository =
                new FileMovieRepository(Path.GetFullPath(moviePath));

            FileCinemaRepository cinemaRepository =
                new FileCinemaRepository(Path.GetFullPath(cinemaPath));

            FileHallRepository hallRepository =
                new FileHallRepository(Path.GetFullPath(hallPath));

            FileBookingRepository bookingRepository =
                new FileBookingRepository(Path.GetFullPath(bookingPath));

            Movie movie = new Movie
            {
                Id = 1,
                Title = "Interstellar",
                Duration = 120,
                Genre = "Sci-Fi",
                Director = "Christopher Nolan",
                ReleaseDate = new DateTime(2026, 8, 1)
            };

            Cinema cinema = new Cinema
            {
                Id = 1,
                Name = "Test Biograf"
            };

            Hall hall = new Hall
            {
                Id = 1,
                Name = "Sal 1",
                CinemaId = 1
            };

            movieRepository.SaveMovies(new List<Movie> { movie });
            cinemaRepository.SaveCinemas(new List<Cinema> { cinema });
            hallRepository.SaveHalls(new List<Hall> { hall });

            CalendarViewModel viewModel =
                new CalendarViewModel(
                    cinemaRepository,
                    movieRepository,
                    hallRepository,
                    bookingRepository
                    );

            viewModel.SelectedMovie = viewModel.Movies[0];
            viewModel.SelectedHall = viewModel.AvailableHalls[0];
            viewModel.ScreeningDate = new DateTime(2026, 8, 10);
            viewModel.ScreeningStartTime = "18:00";

            // Act
            viewModel.RegisterScreeningCommand.Execute(null);

            // Assert
            List<Cinema> savedCinemas = cinemaRepository.LoadCinemas();

            Assert.AreEqual(1, savedCinemas[0].Screenings.Count);
            Assert.AreEqual(1, savedCinemas[0].Screenings[0].MovieId);
            Assert.AreEqual(1, savedCinemas[0].Screenings[0].HallId);

            DeleteFiles(moviePath, cinemaPath, hallPath, bookingPath);
        }


        [TestMethod]
        public void RegisterScreening_WithOverlap_DoesNotAddScreening()
        {
            // Arrange
            string moviePath = "test_overlap_movies.json";
            string cinemaPath = "test_overlap_cinemas.json";
            string hallPath = "test_overlap_halls.json";
            string bookingPath = "test_overlap_bookings.json";

            DeleteFiles(moviePath, cinemaPath, hallPath, bookingPath);

            FileMovieRepository movieRepository =
                new FileMovieRepository(Path.GetFullPath(moviePath));

            FileCinemaRepository cinemaRepository =
                new FileCinemaRepository(Path.GetFullPath(cinemaPath));

            FileHallRepository hallRepository =
                new FileHallRepository(Path.GetFullPath(hallPath));

            FileBookingRepository bookingRepository =
                new FileBookingRepository(Path.GetFullPath(bookingPath));

            Movie movie = new Movie
            {
                Id = 1,
                Title = "Interstellar",
                Duration = 120,
                Genre = "Sci-Fi",
                Director = "Christopher Nolan",
                ReleaseDate = new DateTime(2026, 8, 1)
            };

            Hall hall = new Hall
            {
                Id = 1,
                Name = "Sal 1",
                CinemaId = 1
            };

            Cinema cinema = new Cinema
            {
                Id = 1,
                Name = "Test Biograf",
                Screenings = new List<Screening>
                {
                    new Screening
                    {
                        Id = 1,
                        MovieId = 1,
                        HallId = 1,
                        Date = new DateOnly(2026, 8, 10),
                        StartTime = new TimeOnly(18, 0)
                    }
                }
            };

            movieRepository.SaveMovies(new List<Movie> { movie });
            hallRepository.SaveHalls(new List<Hall> { hall });
            cinemaRepository.SaveCinemas(new List<Cinema> { cinema });

            CalendarViewModel viewModel =
                new CalendarViewModel(
                    cinemaRepository,
                    movieRepository,
                    hallRepository,
                    bookingRepository);

            viewModel.SelectedMovie = viewModel.Movies[0];
            viewModel.SelectedHall = viewModel.AvailableHalls[0];
            viewModel.ScreeningDate = new DateTime(2026, 8, 10);

            // Existing screening:
            // 18:00 + 120 min + 30 min = occupied until 20:30
            viewModel.ScreeningStartTime = "19:00";

            // Act
            viewModel.RegisterScreeningCommand.Execute(null);

            // Assert
            List<Cinema> savedCinemas = cinemaRepository.LoadCinemas();

            Assert.AreEqual(1, savedCinemas[0].Screenings.Count);

            StringAssert.Contains(
                viewModel.StatusMessage,
                "Næste ledige tidspunkt");

            DeleteFiles(moviePath, cinemaPath, hallPath, bookingPath);
        }


        [TestMethod]
        public void RegisterScreening_BeforeReleaseDate_DoesNotAddScreening()
        {
            // Arrange
            string moviePath = "test_release_movies.json";
            string cinemaPath = "test_release_cinemas.json";
            string hallPath = "test_release_halls.json";
            string bookingPath = "test_release_bookings.json";

            DeleteFiles(moviePath, cinemaPath, hallPath, bookingPath);

            FileMovieRepository movieRepository =
                new FileMovieRepository(Path.GetFullPath(moviePath));

            FileCinemaRepository cinemaRepository =
                new FileCinemaRepository(Path.GetFullPath(cinemaPath));

            FileHallRepository hallRepository =
                new FileHallRepository(Path.GetFullPath(hallPath));

            FileBookingRepository bookingRepository =
                new FileBookingRepository(Path.GetFullPath(bookingPath));

            Movie movie = new Movie
            {
                Id = 1,
                Title = "Ny Film",
                Duration = 100,
                Genre = "Drama",
                Director = "Test Director",
                ReleaseDate = new DateTime(2026, 8, 20)
            };

            Cinema cinema = new Cinema
            {
                Id = 1,
                Name = "Test Biograf"
            };

            Hall hall = new Hall
            {
                Id = 1,
                Name = "Sal 1",
                CinemaId = 1
            };

            movieRepository.SaveMovies(new List<Movie> { movie });
            cinemaRepository.SaveCinemas(new List<Cinema> { cinema });
            hallRepository.SaveHalls(new List<Hall> { hall });

            CalendarViewModel viewModel =
                new CalendarViewModel(
                    cinemaRepository,
                    movieRepository,
                    hallRepository,
                    bookingRepository);

            viewModel.SelectedMovie = viewModel.Movies[0];
            viewModel.SelectedHall = viewModel.AvailableHalls[0];

            // Film premieres 20/08, but screening is attempted 19/08
            viewModel.ScreeningDate = new DateTime(2026, 8, 19);
            viewModel.ScreeningStartTime = "18:00";

            // Act
            viewModel.RegisterScreeningCommand.Execute(null);

            // Assert
            List<Cinema> savedCinemas = cinemaRepository.LoadCinemas();

            Assert.AreEqual(0, savedCinemas[0].Screenings.Count);

            StringAssert.Contains(
                viewModel.StatusMessage,
                "kan ikke vises før premieredatoen");

            DeleteFiles(moviePath, cinemaPath, hallPath, bookingPath);
        }


        private void DeleteFiles(
            string moviePath,
            string cinemaPath,
            string hallPath,
            string bookingPath)
        {
            if (File.Exists(moviePath))
                File.Delete(moviePath);

            if (File.Exists(cinemaPath))
                File.Delete(cinemaPath);

            if (File.Exists(hallPath))
                File.Delete(hallPath);

            if (File.Exists(bookingPath))
                File.Delete(bookingPath);
        }
    }
}
