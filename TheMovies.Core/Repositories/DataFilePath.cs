namespace TheMovies.Core.Repositories;

internal static class DataFilePath
{
    public static string Get(string fileName)
    {
        DirectoryInfo? directory =
            new DirectoryInfo(AppContext.BaseDirectory);

        while (directory != null)
        {
            string dataDirectory =
                Path.Combine(directory.FullName, "TheMovies.Core", "Data");

            if (Directory.Exists(dataDirectory))
            {
                return Path.Combine(dataDirectory, fileName);
            }

            directory = directory.Parent;
        }

        throw new DirectoryNotFoundException(
            "Could not locate TheMovies.Core/Data.");
    }
}