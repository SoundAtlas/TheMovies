using System.Text.Json;
using TheMovies.Core.Models;
using TheMovies.Core.Repositories;

namespace TheMovies.Tests;

[TestClass]
public class FileMovieRepositoryTests
{
    [TestMethod]
    public void LoadMovies_EmptyFile_ReturnsEmptyList()
    {

        // Arrange
        string testFilePath = "test_movies_empty.json";
        if (File.Exists(testFilePath))
            File.Delete(testFilePath);

        File.Create(testFilePath).Close();
        FileMovieRepository repository = new FileMovieRepository(Path.GetFullPath(testFilePath));

        // Act
        List<Movie> movies = repository.LoadMovies();

        // Assert
        Assert.AreEqual(0, movies.Count);

        // Clean up
        if (File.Exists(testFilePath))
            File.Delete(testFilePath);

    }

    [TestMethod]
    public void LoadMovies_WithSavedMovies_LoadsMoviesCorrectly()
    {
        // Arrange
        string testFilePath = "test_movies_load.json";
        if (File.Exists(testFilePath))
            File.Delete(testFilePath);
        FileMovieRepository repository = new FileMovieRepository(Path.GetFullPath(testFilePath));
        List<Movie> moviesToSave = new List<Movie>
        {
            new Movie { Title = "Inception", Duration = 148, Genre = "Sci-Fi" },
            new Movie { Title = "The Matrix", Duration = 136, Genre = "Action" }
        };

        repository.SaveMovies(moviesToSave);

        // Act
        List<Movie> loadedMovies = repository.LoadMovies();

        // Assert
        Assert.AreEqual(2, loadedMovies.Count);
        Assert.AreEqual("Inception", loadedMovies[0].Title);
        Assert.AreEqual(148, loadedMovies[0].Duration);
        Assert.AreEqual("Sci-Fi", loadedMovies[0].Genre);
        Assert.AreEqual("The Matrix", loadedMovies[1].Title);
        Assert.AreEqual(136, loadedMovies[1].Duration);
        Assert.AreEqual("Action", loadedMovies[1].Genre);

        // Clean up
        if (File.Exists(testFilePath))
            File.Delete(testFilePath);
    }

    [TestMethod]
    public void SaveMovies_SavesMovieToFile()
    {
        // Arrange
        string testFilePath = "test_movies_save.json";
        if (File.Exists(testFilePath))
            File.Delete(testFilePath);
        FileMovieRepository repository = new FileMovieRepository(Path.GetFullPath(testFilePath));
        List<Movie> moviesToSave = new List<Movie>
        {
            new Movie { Title = "Inception", Duration = 148, Genre = "Sci-Fi" }
        };

        // Act
        repository.SaveMovies(moviesToSave);

        // Assert
        Assert.IsTrue(File.Exists(testFilePath));
        string jsonFromFile = File.ReadAllText(testFilePath);
        List<Movie>? moviesFromFile = JsonSerializer.Deserialize<List<Movie>>(jsonFromFile);
        Assert.IsNotNull(moviesFromFile);
        Assert.AreEqual(1, moviesFromFile.Count);
        Assert.AreEqual("Inception", moviesFromFile[0].Title);
        Assert.AreEqual(148, moviesFromFile[0].Duration);
        Assert.AreEqual("Sci-Fi", moviesFromFile[0].Genre);

        // Clean up
        if (File.Exists(testFilePath))
            File.Delete(testFilePath);
    }



}
