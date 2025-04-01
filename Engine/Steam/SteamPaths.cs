using System.Text.RegularExpressions;

namespace SourceRewrite.Steam
{
    public static class SteamPaths
    {
        /// <summary>
        /// Gets the File Path to Steam install.
        /// </summary>
        public static string? GetSteamPath()
        {
            string? steamPath = null;
            if (OperatingSystem.IsWindows())
            {
                string regPath = @"HKEY_CURRENT_USER\Software\Valve\Steam";
                object? regValue = Microsoft.Win32.Registry.GetValue(regPath, "SteamPath", null);
                if (regValue != null)
                {
                    steamPath = regValue.ToString()?.Replace("/", "\\"); // Convert to Windows-style path
                }
            }
            else if (OperatingSystem.IsLinux())
            {
                steamPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), ".steam", "steam");
            }
            else if (OperatingSystem.IsMacOS())
            {
                steamPath = "/Library/Application Support/Steam";
            }

            return steamPath;
        }

        /// <summary>
        /// Gets the File Path to a Game from its appID.
        /// </summary>
        public static string? GetGamePathFromAppId(int appId)
        {
            string? steamPath = GetSteamPath();
            if (string.IsNullOrEmpty(steamPath))
            {
                Console.WriteLine("Steam is not installed or could not be found.");
                return null;
            }

            List<string> steamLibraries = GetSteamLibraryFolders(steamPath);
            foreach (string library in steamLibraries)
            {
                string manifestPath = Path.Combine(library, $"appmanifest_{appId}.acf");
                if (File.Exists(manifestPath))
                {
                    string manifestContent = File.ReadAllText(manifestPath);
                    Match match = Regex.Match(manifestContent, @"""installdir""\s*""([^""]+)""");
                    if (match.Success)
                    {
                        string installDir = match.Groups[1].Value;
                        return Path.Combine(library, "common", installDir);
                    }
                }
            }

            Console.WriteLine("Game not found.");
            return null;
        }

        /// <summary>
        /// Retrieves Steam library folders in a platform-agnostic way.
        /// </summary>
        private static List<string> GetSteamLibraryFolders(string steamPath)
        {
            List<string> libraryPaths = new() { Path.Combine(steamPath, "steamapps") };
            string vdfPath = Path.Combine(steamPath, "steamapps", "libraryfolders.vdf");

            if (File.Exists(vdfPath))
            {
                string vdfContent = File.ReadAllText(vdfPath);
                var matches = Regex.Matches(vdfContent, @"""path""\s*""([^""]+)""");

                foreach (Match match in matches)
                {
                    string libraryPath = match.Groups[1].Value.Replace("\\\\", "\\");
                    libraryPaths.Add(Path.Combine(libraryPath, "steamapps"));
                }
            }

            return libraryPaths;
        }
    }
}
