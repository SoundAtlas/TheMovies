using TheMovies.Core.Models;

namespace TheMovies.Core.Interfaces
{
    public interface IHallRepository
    {
        List<Hall> LoadHalls();
        void SaveHalls(List<Hall> halls);
    }
}
