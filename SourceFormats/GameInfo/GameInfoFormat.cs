using SourceFormats.KeyValues;

namespace SourceFormats.GameInfo
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

            // Populate GameInfo-Specific Data
            GameName = (string)KeyValues.ParentKeys[0].GetKeyValue("game").Value;
            SteamAppID = (int)KeyValues.ParentKeys[0].GetKeyValue("SteamAppId").Value;
        }
    }
}
