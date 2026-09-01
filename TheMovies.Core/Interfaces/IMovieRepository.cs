using TheMovies.Core.Models;

namespace TheMovies.Core.Interfaces
{
    public interface IMovieRepository
    {
        List<Movie> LoadMovies();
        void SaveMovies(List<Movie> movies);
    }
}
