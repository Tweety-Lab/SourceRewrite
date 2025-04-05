using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileFormats.BSP
{
    // Represents the structure of a BSP file
    public class BSPFormat
    {
        /// <summary>
        /// The header of the BSP file.
        /// </summary>
        public BSPHeader Header;

        // Get a Lump
        public BSPLump GetLump(BSPLumpType type)
        {
            return Header.Lumps.FirstOrDefault(l => l.Type == type);
        }


        /// <summary>
        /// Sets the lump of the specified type. Creates the lump if it doesn't exist.
        /// </summary>
        public void SetLumpData<T>(BSPLumpType type, T data) where T : struct
        {
            var lump = GetLump(type);

            if (lump.Data == null)
            {
                // Create a new lump if it doesn't exist
                lump = new BSPLump
                {
                    Type = type,
                    Offset = 0,
                    Length = 0,
                    Data = data
                };

                Header.Lumps[(int)type] = lump;
            }

            lump.Data = data;
        }
    }
}
