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
        public void SetLumpData<T>(BSPLumpType type, T[] data) where T : struct
        {
            var lump = GetLump(type);

            if (lump.Data == null)
            {
                // Create a new lump if it doesn't exist
                lump = new BSPLump
                {
                    Type = type,
                    Offset = 0,
                    Length = data.Length * System.Runtime.InteropServices.Marshal.SizeOf(typeof(T)),
                    Data = data
                };

                Header.Lumps[(int)type] = lump;
            }

            lump.Data = data;
            lump.Length = data.Length * System.Runtime.InteropServices.Marshal.SizeOf(typeof(T));

            // Recalculate the offsets for all lumps
            UpdateLumpOffsets();
        }

        public static int CurrentOffset = 0;
        private void UpdateLumpOffsets()
        {
            // Loop through all lumps using an index to modify the original array
            for (int i = 0; i < Header.Lumps.Length; i++)
            {
                var lump = Header.Lumps[i];

                if (lump.Data != null)
                {
                    // Set the lump's offset directly through the array
                    lump.Offset = CurrentOffset;
                    CurrentOffset += lump.Length;

                    // Update the lump back into the array after modifying it
                    Header.Lumps[i] = lump;
                }
            }
        }
    }
}
