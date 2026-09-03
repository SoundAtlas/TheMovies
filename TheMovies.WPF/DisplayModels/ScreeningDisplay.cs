namespace TheMovies.WPF.DisplayModels
{
    public class ScreeningDisplay
    {
        public int ScreeningId { get; set; }
        public int MovieId { get; set; }
        public int HallId { get; set; }
        public DateOnly Date { get; set; }
        public TimeOnly StartTime { get; set; }

        public string TimeSlot { get; set; } = string.Empty;
        public string MovieTitle { get; set; } = string.Empty;
        public string HallName { get; set; } = string.Empty;
        public bool IsPremiere { get; set; }
        // Number of seats left for this screening (calculated at creation)
        public int SeatsLeft { get; set; }
    }
}