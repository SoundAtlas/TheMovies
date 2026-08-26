using System.Text.Json;
using TheMovies.Core.Models;

namespace TheMovies.Core.Repositories
{
	public class FileCinemaRepository
	{
		private readonly string _filePath;

		public FileCinemaRepository(string filePath = @"..\..\..\..\TheMovies.Core\Data\cinemas.json") // Default file path for the cinemas.json file
		{
			_filePath = filePath;
		}

		public void SaveCinemas(List<Cinema> cinemas)
		{
			JsonSerializerOptions options = new JsonSerializerOptions
			{
				WriteIndented = true // Gør det lidt mere læsbart
			};

			string json = JsonSerializer.Serialize(cinemas, options);
			File.WriteAllText(_filePath, json);
		}

		public List<Cinema> LoadCinemas()
		{
			if (!File.Exists(_filePath))
			{
				return new List<Cinema>();
			}

			string json = File.ReadAllText(_filePath);

			if (string.IsNullOrWhiteSpace(json))
			{
				return new List<Cinema>();
			}

			List<Cinema>? cinemas = JsonSerializer.Deserialize<List<Cinema>>(json);
			return cinemas ?? new List<Cinema>();
		}
	}
}