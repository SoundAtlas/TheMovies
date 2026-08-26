using System.Text.Json;
using TheMovies.Core.Models;


namespace TheMovies.Core.Repositories
{
    public class FileMovieRepository
    {

        private readonly string _filePath;

        public FileMovieRepository(string filePath = @"..\..\..\..\TheMovies.Core\Data\movies.json")
        {
            _filePath = filePath;
            if (!File.Exists(_filePath))
                File.Create(_filePath).Close();
        }

        public void SaveMovies(List<Movie> movies)
        {
            JsonSerializerOptions options = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            string json = JsonSerializer.Serialize(movies, options);

            File.WriteAllText(_filePath, json);
        }

        public List<Movie> LoadMovies()
        {

            if (!File.Exists(_filePath))
            {
                return new List<Movie>();
            }

            // Read the JSON data from the file
            string json = File.ReadAllText(_filePath);

            // Check if the JSON string is empty or null
            if (string.IsNullOrWhiteSpace(json))
            {
                return new List<Movie>();
            }

            // Deserialize the JSON data into a list of Movie objects
            List<Movie>? movies = JsonSerializer.Deserialize<List<Movie>>(json);

            // Return the list of movies, or an empty list if deserialization failed
            return movies ?? new List<Movie>();
        }
    }
}
