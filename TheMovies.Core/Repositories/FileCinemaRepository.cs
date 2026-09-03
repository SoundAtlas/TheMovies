using System.Text.Json;
using TheMovies.Core.Interfaces;
using TheMovies.Core.Models;

namespace TheMovies.Core.Repositories
{
    public class FileCinemaRepository : ICinemaRepository
    {
        private readonly string _filePath;

        // AppContext.BaseDirectory er mappen .exe'en rent faktisk kører fra - virker uanset
        // om man starter via Visual Studio, "dotnet run" eller den byggede .exe direkte.
        public FileCinemaRepository(string? filePath = null)
        {
            _filePath = filePath ?? DataFilePath.Get("cinemas.json");

            // Data-mappen findes ikke nødvendigvis endnu - uden denne linje fejler File.Create
            // nedenfor, fordi mappen mangler.
            Directory.CreateDirectory(Path.GetDirectoryName(_filePath)!);
            if (!File.Exists(_filePath))
                File.Create(_filePath).Close();
        }

        public void SaveCinemas(List<Cinema> cinemas)
        {

            string json = JsonSerializer.Serialize(cinemas, new JsonSerializerOptions { WriteIndented = true });

            File.WriteAllText(_filePath, json);
        }
        public List<Cinema> LoadCinemas()
        {

            if (!File.Exists(_filePath))
            {
                return new List<Cinema>();
            }

            // Read the JSON data from the file
            string json = File.ReadAllText(_filePath);

            // Check if the JSON string is empty or null
            if (string.IsNullOrWhiteSpace(json))
            {
                return new List<Cinema>();
            }

            // Deserialize the JSON data into a list of Cinema objects
            List<Cinema>? cinemas = JsonSerializer.Deserialize<List<Cinema>>(json);

            // Return the list of cinemas, or an empty list if deserialization failed
            return cinemas ?? new List<Cinema>();
        }
    }
}
