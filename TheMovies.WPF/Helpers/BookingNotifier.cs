namespace TheMovies.WPF.Helpers
{
    public static class BookingNotifier
    {
        public static event Action<int>? BookingChanged;

        public static void RaiseBookingChanged(int screeningId)
        {
            BookingChanged?.Invoke(screeningId);
        }
    }
}
