using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using TheMovies.Core.Models;
using TheMovies.Core.Interfaces;
using System.IO;

namespace TheMovies.Core.Repositories
{
    public class FileBookingRepository : IBookingRepository
    {
        private readonly string _filePath;

        // expose the file path for diagnostics
        public string FilePath => _filePath;

        // AppContext.BaseDirectory er mappen .exe'en rent faktisk kører fra - virker uanset
        // om man starter via Visual Studio, "dotnet run" eller den byggede .exe direkte.
        public FileBookingRepository(string? filePath = null)
        {
            // Keep behavior consistent with other file repositories (movies/halls):
            // use runtime output Data folder next to the executable by default.
            _filePath = filePath ?? Path.Combine(AppContext.BaseDirectory, "Data", "bookings.json");

            // Ensure directory exists and file is created
            Directory.CreateDirectory(Path.GetDirectoryName(_filePath)!);
            if (!File.Exists(_filePath))
                File.Create(_filePath).Close();
        }

        public void SaveBookings(List<Booking> bookings)
        {
            JsonSerializerOptions options = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            string json = JsonSerializer.Serialize(bookings, options);

            File.WriteAllText(_filePath, json);
        }

        public List<Booking> LoadBookings()
        {

            if (!File.Exists(_filePath))
            {
                return new List<Booking>();
            }

            // Read the JSON data from the file
            string json = File.ReadAllText(_filePath);

            // Check if the JSON string is empty or null
            if (string.IsNullOrWhiteSpace(json))
            {
                return new List<Booking>();
            }

            // Deserialize the JSON data into a list of Booking objects
            List<Booking>? bookings = JsonSerializer.Deserialize<List<Booking>>(json); 
            // Return the list of bookings, or an empty list if deserialization failed
            return bookings ?? new List<Booking>();
        }
    }
}
