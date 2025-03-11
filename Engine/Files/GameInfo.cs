using SourceRewrite.Windowing;
using SourceRewrite.Steam;
using FileFormats.KeyValues;

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
            return GameWindow.CurrentWindow.GameInfo.KeyValues.GetKeyValue(name);
        }

        /// <summary>
        /// Returns a GameInfo ParentKey from its name.
        /// </summary>
        public static ParentKey GetParentKey(string name)
        {
            return GameWindow.CurrentWindow.GameInfo.KeyValues.GetParentKey(name);
        }

        /// <summary>
        /// Returns the Game Name from the GameInfo
        /// </summary>
        public static string GetGameName()
        {
            return GameWindow.CurrentWindow.GameInfo.GameName;
        }

        /// <summary>
        /// Returns the mounted SteamAppId defined in gameinfo.txt.
        /// </summary>
        public static int GetSteamAppID()
        {
            return GameWindow.CurrentWindow.GameInfo.SteamAppID;
        }

        /// <summary>
        /// Returns the paths to all mounted games defined in gameinfo.txt.
        /// </summary>
        public static string GetMountedPaths()
        {
            return $"{SteamPaths.GetGamePathFromAppId(GetSteamAppID())}\\{(string)GetKeyValue("Game").Value}";
        }
    }
}
