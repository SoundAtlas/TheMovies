namespace TheMovies.Core.Models
{
    public class Screening
    {
        public int Id { get; set; }

        // Vi gemmer filmens titel som tekst i stedet for en reference til Movie-objektet.
        // Det holder JSON-filen simpel og uden krydsreferencer mellem filer
        // lidt i tvivl om dette er den bedste løsning, men det virker og det var sådan min tidligere gruppe gjorde det :P

        public int MovieId { get; set; }
        public int HallId { get; set; }

        public DateOnly Date { get; set; } // noget med .net6 versus 7 og serilization med json vi lige skal tjekke tror jeg. husker det som at det kunne give lidt bøvl
        public TimeOnly StartTime { get; set; }


    }
}
