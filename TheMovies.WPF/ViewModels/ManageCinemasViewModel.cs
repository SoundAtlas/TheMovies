using TheMovies.Core.Interfaces;

namespace TheMovies.WPF.ViewModels
{
    // Samler CinemaViewModel og HallViewModel på én side, ligesom MainViewModel gør -
    // biografer og sale hænger sammen, så det giver mening at administrere dem samme sted.
    public class ManageCinemasViewModel
    {
        public CinemaViewModel CinemaViewModel { get; }
        public HallViewModel HallViewModel { get; }

        public ManageCinemasViewModel(
            ICinemaRepository cinemaRepository,
            IHallRepository hallRepository)
        {
            CinemaViewModel = new CinemaViewModel(cinemaRepository, hallRepository);

            // HallViewModel skal bruge den samme Cinemas-liste som CinemaViewModel, så
            // biograf-dropdownen i sal-sektionen matcher biograf-listen ovenover.
            HallViewModel = new HallViewModel(hallRepository, cinemaRepository, CinemaViewModel.Cinemas);
        }
    }
}
