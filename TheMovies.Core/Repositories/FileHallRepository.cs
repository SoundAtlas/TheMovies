using System.Text.Json;
using TheMovies.Core.Models;

namespace TheMovies.Core.Repositories
{
    public class FileHallRepository
    {

        private readonly string _filePath;

        // AppContext.BaseDirectory er mappen .exe'en rent faktisk kører fra - virker uanset
        // om man starter via Visual Studio, "dotnet run" eller den byggede .exe direkte.
        public FileHallRepository(string? filePath = null)
        {
            _filePath = filePath ?? Path.Combine(AppContext.BaseDirectory, "Data", "halls.json");

            // Data-mappen findes ikke nødvendigvis endnu - uden denne linje fejler File.Create
            // nedenfor, fordi mappen mangler.
            Directory.CreateDirectory(Path.GetDirectoryName(_filePath)!);
            if (!File.Exists(_filePath))
                File.Create(_filePath).Close();
        }

        public void SaveHalls(List<Hall> halls)
        {

            string json = JsonSerializer.Serialize(halls, new JsonSerializerOptions { WriteIndented = true });

            File.WriteAllText(_filePath, json);
        }
        public List<Hall> LoadHalls()
        {

            if (!File.Exists(_filePath))
            {
                return new List<Hall>();
            }

            // Read the JSON data from the file
            string json = File.ReadAllText(_filePath);

            // Check if the JSON string is empty or null
            if (string.IsNullOrWhiteSpace(json))
            {
                return new List<Hall>();
            }

            // Deserialize the JSON data into a list of Hall objects
            List<Hall>? halls = JsonSerializer.Deserialize<List<Hall>>(json);

            // Return the list of halls, or an empty list if deserialization failed
            return halls ?? new List<Hall>();
        }
    }
}
