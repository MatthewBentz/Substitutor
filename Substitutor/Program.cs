using System.Configuration;

internal class Program
{
    private static void Main(string[] args)
    {
        var moviesDir = ConfigurationManager.AppSettings["MoviesPath"] ?? throw new Exception("Movies directory is invalid");
        var tvShowsDir = ConfigurationManager.AppSettings["TVShowsPath"] ?? throw new Exception("TV Shows directory is invalid");
        var subPattern = ConfigurationManager.AppSettings["SubtitlePattern"] ?? throw new Exception("No subtitle pattern provided");

        // ----------------------------------
        //   Move the subtitles for Movies.
        // ----------------------------------

        var subFiles = Directory.GetFiles(moviesDir, subPattern, SearchOption.AllDirectories);

        foreach (var file in subFiles)
        {
            var fileName = Path.GetFileName(file);

            // -----------------------------------------------
            // Check that the file has not been moved already.

            if (Directory.GetParent(file)?.Name.ToLower() == "subs")
            {
                // --------------------------------
                // Get the directory for the movie.

                var movieFolder = Directory.GetParent(Directory.GetParent(file).FullName);
                var destinationFilePath = Path.Combine(movieFolder.FullName, $"{fileName[..2]}{movieFolder.Name}.en.srt");

                File.Move(file, destinationFilePath, true);

                Console.WriteLine($"{file} \n\t→ {destinationFilePath}\n");
            }
        }

        // -----------------------------------
        //   Move the subtitles for TVShows.
        // -----------------------------------

        subFiles = Directory.GetFiles(tvShowsDir, subPattern, SearchOption.AllDirectories);

        foreach (var file in subFiles)
        {
            // -----------------------------------------------
            // Check that the file has not been moved already.
            
            if (Directory.GetParent(Directory.GetParent(file).FullName).Name.ToLower() == "subs")
            {
                var episodeName = Directory.GetParent(file).Name;
                var seasonDirectory = Directory.GetParent(Directory.GetParent(Directory.GetParent(file).FullName).FullName).FullName;

                var destinationFilePath = Path.Combine(seasonDirectory, $"{episodeName}.en.srt");

                // -------------------------------------------
                // Only copy file if it is larger than current

                if (!File.Exists(destinationFilePath))
                {
                    File.Move(file, destinationFilePath, true);

                    Console.WriteLine($"{file} \n\t→ {destinationFilePath}\n");
                } 
                else if (new FileInfo(file).Length > new FileInfo(destinationFilePath).Length)
                {
                    File.Move(file, destinationFilePath, true);

                    Console.WriteLine($"{file} \n\t→ {destinationFilePath}\n");
                }
            }
        }
    }
}
