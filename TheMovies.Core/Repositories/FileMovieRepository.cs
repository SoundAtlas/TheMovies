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

            // Læser JSON-data fra filen
            string json = File.ReadAllText(_filePath);

            // Returnerer en ny liste hvis JSON-dataen er tom
            if (string.IsNullOrWhiteSpace(json))
            {
                return new List<Movie>();
            }

            // Omdanner json til en liste af Movie-objekter
            List<Movie>? movies = JsonSerializer.Deserialize<List<Movie>>(json);

            // Returner listen eller en tom liste hvis deserialiseringen mislykkes
            return movies ?? new List<Movie>();
        }
    }
}
