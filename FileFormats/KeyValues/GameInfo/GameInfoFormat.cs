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
            KeyValues = new KeyValuesFormat(content);

            // Populate Data
            GameName = (string) KeyValues.GetKeyValue("game").Value;
            SteamAppID = (int) KeyValues.GetKeyValue("SteamAppId").Value;
        }
    }
}
