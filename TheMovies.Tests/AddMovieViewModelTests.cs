using TheMovies.Core.Repositories;
using TheMovies.WPF.ViewModels;


namespace TheMovies.Tests
{


    [TestClass]
    public class AddMovieViewModelTests
    {
        [TestMethod]
        public void AddMovie_WithValidInput_AddsMovie()
        {

            // Arrange

            string testFilePath = "test_movies_add.json";
            if (File.Exists(testFilePath))
                File.Delete(testFilePath);

            FileMovieRepository repository = new FileMovieRepository(testFilePath);

            MovieViewModel viewModel = new MovieViewModel(repository);

            viewModel.Title = "Interstellar";
            viewModel.Duration = "169";
            viewModel.Genre = "Sci-Fi";

            int movieCountBefore = viewModel.Movies.Count;


            // Act

            viewModel.RegisterMovieCommand.Execute(null);

            // Assert   
            Assert.AreEqual(movieCountBefore + 1, viewModel.Movies.Count);

            // Clean up
            if (File.Exists(testFilePath))
                File.Delete(testFilePath);
        }

        [TestMethod]
        public void AddMovie_WithInvalidDuration_DoesNotAddMovie()
        {
            // Arrange

            string testFilePath = "test_movies_invalid.json";
            if (File.Exists(testFilePath))
                File.Delete(testFilePath);

            FileMovieRepository repository = new FileMovieRepository(testFilePath);

            MovieViewModel viewModel = new MovieViewModel(repository);

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

            if (File.Exists(testFilePath))
                File.Delete(testFilePath);

            FileMovieRepository repository = new FileMovieRepository(testFilePath);

            MovieViewModel viewModel = new MovieViewModel(repository);

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

            if (File.Exists(testFilePath))
                File.Delete(testFilePath);

            FileMovieRepository repository = new FileMovieRepository(testFilePath);

            MovieViewModel viewModel = new MovieViewModel(repository);

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