
namespace SourceFormats.KeyValues.GameInfo
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
    }
}
