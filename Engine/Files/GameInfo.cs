using SourceRewrite.Files.FileTypes;
using SourceRewrite.Windowing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using SourceRewrite.Steam;

namespace SourceRewrite.Files
{
    /// <summary>
    /// Holds KeyValue data from gameinfo.txt.
    /// </summary>
    public class GameInfoContext
    {
        public KeyValuesFormat KeyValues;
        public GameInfoContext(string filePath)
        {
            try
            {
                string content = File.ReadAllText(filePath);

                KeyValues = new KeyValuesFormat(content);

                // Set the Title of the Game Window to as defined in Game Info
                GameWindow.CurrentWindow.GetSilkWindow().Title = KeyValues.GetKeyValue("game").Value.ToString();

                Console.WriteLine(KeyValues.GetKeyValue("Game").Value);
                Console.WriteLine(SteamPaths.GetGamePathFromAppId(620));

            }
            catch (Exception ex)
            {
                Console.WriteLine($"A GameInfo error occurred: {ex.Message}");
            }
        }
    }

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
            return GameWindow.CurrentWindow.GameInfo.KeyValues.GetKeyValue("game").Value.ToString();
        }

        /// <summary>
        /// Returns the path to all mounted games defined in gameinfo.txt.
        /// </summary>
        public static string GetMountedPaths()
        {
            return GameWindow.CurrentWindow.GameInfo.KeyValues.GetKeyValue("Game").Value.ToString();
        }
    }
}
