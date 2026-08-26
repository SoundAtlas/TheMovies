using System.Text.Json;
using TheMovies.Core.Models;

namespace TheMovies.Core.Repositories
{
    public class FileHallRepository
    {

        private readonly string _filePath;

        public FileHallRepository(string filePath = @"..\..\..\..\TheMovies.Core\Data\halls.json")
        {
            _filePath = filePath;

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
