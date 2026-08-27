namespace TheMovies.WPF.DisplayModels
{
    // Ét felt i kalendergitteret — én dag. 42 stk (6 rækker × 7 kolonner).
    public class DayDisplay
    {
        public DateOnly Date { get; set; }
        public bool HasScreenings { get; set; }     // Prik i UI hvis true altså så hvis der er forestillinger på den dag, kommer der en prik i UI
        public bool IsInCurrentMonth { get; set; }  // Nedtonet hvis false så hvis dagen ikke er i den måned der vises i kalenderen, skal den være nedtonet
                                                    // så det tydeligt fremgår at det er en dag i en anden måned, men stadig en del af kalendergitter
                                                    // (det er en layoutmæssighed).
    }
}
