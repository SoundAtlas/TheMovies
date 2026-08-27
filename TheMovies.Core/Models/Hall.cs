using System.Text.Json.Serialization;

namespace TheMovies.Core.Models
{
    public class Hall
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int CinemaId { get; set; }

        // This property is not serialized to JSON, but it can be used in the application to display the cinema name associated with the hall.
        [JsonIgnore]
        public string CinemaName { get; set; }

    }
}
