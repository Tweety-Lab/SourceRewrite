using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileFormats.KeyValues.GameInfo
{
    // GameInfo is just abstracted KeyValues

    /// <summary>
    /// Holds KeyValue data from gameinfo.txt.
    /// </summary>
    public class GameInfoFormat
    {
        public KeyValuesFormat KeyValues;
        public GameInfoFormat(string content)
        {
            KeyValues = new KeyValuesFormat(content);
        }
    }
}
