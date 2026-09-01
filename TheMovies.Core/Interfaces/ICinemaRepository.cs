using TheMovies.Core.Models;

namespace TheMovies.Core.Interfaces
{
    public interface ICinemaRepository
    {
        List<Cinema> LoadCinemas();
        void SaveCinemas(List<Cinema> cinemas);
    }
}
