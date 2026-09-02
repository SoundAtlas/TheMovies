namespace TheMovies.WPF.DisplayModels
{
    public class ScreeningDisplay
    {
        public int ScreeningId { get; set; }
        public int MovieId { get; set; }
        public int HallId { get; set; }
        public DateOnly Date { get; set; }
        public TimeOnly StartTime { get; set; }

        public string TimeSlot { get; set; }
        public string MovieTitle { get; set; }
        public string HallName { get; set; }
        public bool IsPremiere { get; set; }
        // Number of seats left for this screening (calculated at creation)
        public int SeatsLeft { get; set; }
    }
}