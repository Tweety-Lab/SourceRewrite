using Steam;

namespace FileFormats.KeyValues.GameInfo
{
    // GameInfo is just abstracted KeyValues

    /// <summary>
    /// Holds data from gameinfo.txt.
    /// </summary>
    public class GameInfoFormat
    {
        public KeyValuesFormat KeyValues;
        
        // Data
        public string GameName;
        public int SteamAppID;

        public GameInfoFormat(string content)
        {
            // Load KeyValues
            KeyValues = new KeyValuesFormat(content);

            // Populate Data
            GameName = (string) KeyValues.GetKeyValue("game").Value;
            SteamAppID = (int) KeyValues.GetKeyValue("SteamAppId").Value;
        }

        /// <summary>
        /// Returns the paths to all mounted games defined in gameinfo.txt.
        /// </summary>
        public string[] GetMountedPaths()
        {
            List<string> paths = new List<string>();
            foreach (var searchPath in KeyValues.GetParentKey("GameInfo").GetChildParentKey("FileSystem").GetChildParentKey("SearchPaths").ChildKeyValues)
            {
                paths.Add($"{SteamPaths.GetGamePathFromAppId(SteamAppID)}\\{(string)searchPath.Value}");
            }

            return paths.ToArray();
        }
    }
}
