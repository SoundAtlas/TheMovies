using TheMovies.Core.Repositories;
using TheMovies.WPF.ViewModels;


namespace TheMovies.Tests
{


    [TestClass]
    public class MovieViewModelTests
    {
        // Helper method to create a MovieViewModel with a temporary file for testing
        private MovieViewModel CreateViewModel(
    string movieFilePath,
    string cinemaFilePath)
        {
            if (File.Exists(movieFilePath))
                File.Delete(movieFilePath);

            if (File.Exists(cinemaFilePath))
                File.Delete(cinemaFilePath);

            FileMovieRepository movieRepository =
                new FileMovieRepository(Path.GetFullPath(movieFilePath));

            FileCinemaRepository cinemaRepository =
                new FileCinemaRepository(Path.GetFullPath(cinemaFilePath));

            return new MovieViewModel(
                movieRepository,
                cinemaRepository);
        }

        [TestMethod]
        public void AddMovie_WithValidInput_AddsMovie()
        {
            // Arrange

            string testFilePath = "test_movies_add.json";
            string testCinemaFilePath = "test_cinemas_add.json";

            MovieViewModel viewModel =
                CreateViewModel(
                    testFilePath,
                    testCinemaFilePath);

            viewModel.Title = "Interstellar";
            viewModel.Duration = "169";
            viewModel.Genre = "Sci-Fi";

            int movieCountBefore = viewModel.Movies.Count;

            // Act

            viewModel.RegisterMovieCommand.Execute(null);

            // Assert

            Assert.AreEqual(
                movieCountBefore + 1,
                viewModel.Movies.Count);

            // Clean up

            if (File.Exists(testFilePath))
                File.Delete(testFilePath);

            if (File.Exists(testCinemaFilePath))
                File.Delete(testCinemaFilePath);
        }

        [TestMethod]
        public void AddMovie_WithInvalidDuration_DoesNotAddMovie()
        {
            // Arrange

            string testFilePath = "test_movies_invalid.json";
            string testCinemaFilePath = "test_cinemas_invalid.json";

            MovieViewModel viewModel =
                CreateViewModel(
                    testFilePath,
                    testCinemaFilePath);


            viewModel.Title = "Interstellar";
            viewModel.Duration = "invalid";
            viewModel.Genre = "Sci-Fi";

            int movieCountBefore = viewModel.Movies.Count;

            // Act

            viewModel.RegisterMovieCommand.Execute(null);

            // Assert

            Assert.AreEqual(movieCountBefore, viewModel.Movies.Count);

            // Clean up
            if (File.Exists(testFilePath))
                File.Delete(testFilePath);
        }

        [TestMethod]
        public void AddMovie_WithValidInput_ClearsInputFields()
        {
            // Arange 
            string testFilePath = "test_movies_clear.json";
            string testCinemaFilePath = "test_cinemas_clear.json";

            if (File.Exists(testFilePath))
                File.Delete(testFilePath);

            MovieViewModel viewModel =
                CreateViewModel(
                    testFilePath,
                    testCinemaFilePath);

            viewModel.Title = "Inception";
            viewModel.Duration = "148";
            viewModel.Genre = "Sci-Fi";

            // Act

            viewModel.RegisterMovieCommand.Execute(null);

            // Assert

            Assert.AreEqual("", viewModel.Title);
            Assert.AreEqual("", viewModel.Duration);
            Assert.AreEqual("", viewModel.Genre);

            // Clean up
            if (File.Exists(testFilePath))
                File.Delete(testFilePath);
        }


        [TestMethod]
        public void AddMovie_WithInvalidInput_DoesNotClearInputFields()
        {
            // Arrange

            string testFilePath = "test_movies_invalid_clear.json";
            string testCinemaFilePath = "test_cinemas_invalid_clear.json";

            if (File.Exists(testFilePath))
                File.Delete(testFilePath);

            MovieViewModel viewModel =
                CreateViewModel(
                    testFilePath,
                    testCinemaFilePath);



            viewModel.Title = "Inception";
            viewModel.Duration = "invalid";
            viewModel.Genre = "Sci-Fi";

            // Act

            viewModel.RegisterMovieCommand.Execute(null);

            // Assert
            Assert.AreEqual("Inception", viewModel.Title);
            Assert.AreEqual("invalid", viewModel.Duration);
            Assert.AreEqual("Sci-Fi", viewModel.Genre);

            // Clean up
            if (File.Exists(testFilePath))
                File.Delete(testFilePath);
        }

    }

}
