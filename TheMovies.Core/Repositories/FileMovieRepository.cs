using System.Text.Json;
using TheMovies.Core.Interfaces;
using TheMovies.Core.Models;


namespace TheMovies.Core.Repositories
{
    public class FileMovieRepository : IMovieRepository
    {

        private readonly string _filePath;

        // AppContext.BaseDirectory er mappen .exe'en rent faktisk kører fra - virker uanset
        // om man starter via Visual Studio, "dotnet run" eller den byggede .exe direkte.
        public FileMovieRepository(string? filePath = null)
        {
            _filePath = filePath ?? DataFilePath.Get("movies.json");

            // Data-mappen findes ikke nødvendigvis endnu (fx første gang appen køres et nyt sted) -
            // uden denne linje fejler File.Create nedenfor, fordi mappen mangler.
            Directory.CreateDirectory(Path.GetDirectoryName(_filePath)!);
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
