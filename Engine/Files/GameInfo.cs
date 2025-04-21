using Steam;
using SourceFormats.KeyValues;
using SourceRewrite.Windowing.Modules;

namespace SourceRewrite.Files
{
    /// <summary>
    /// GameInfo Abstraction.
    /// </summary>
    public static class GameInfo
    {
        /// <summary>
        /// Returns a GameInfo KeyValue from its Key.
        /// </summary>
        public static KeyValue GetKeyValue(string name)
        {
            return GameModules.GetModule<GameInfoModule>().GameInfo.KeyValues.GetKeyValue(name);
        }

        /// <summary>
        /// Returns a GameInfo ParentKey from its name.
        /// </summary>
        public static ParentKey GetParentKey(string name)
        {
            return GameModules.GetModule<GameInfoModule>().GameInfo.KeyValues.GetParentKey(name);
        }

        /// <summary>
        /// Returns the Game Name from the GameInfo
        /// </summary>
        public static string GetGameName()
        {
            return GameModules.GetModule<GameInfoModule>().GameInfo.GameName;
        }

        /// <summary>
        /// Returns the mounted SteamAppId defined in gameinfo.txt.
        /// </summary>
        public static int GetSteamAppID()
        {
            return GameModules.GetModule<GameInfoModule>().GameInfo.SteamAppID;
        }

        /// <summary>
        /// Returns the paths to all mounted games defined in gameinfo.txt.
        /// </summary>
        public static string[] GetMountedPaths()
        {
            List<string> paths = new List<string>();
            foreach (var searchPath in GetParentKey("GameInfo").GetChildParentKey("FileSystem").GetChildParentKey("SearchPaths").ChildKeyValues)
            {
                paths.Add($"{SteamPaths.GetGamePathFromAppId(GetSteamAppID())}\\{(string)searchPath.Value}");
            }

            return paths.ToArray();
        }
    }
}
