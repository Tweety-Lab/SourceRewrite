using System.Text;

namespace FileFormats.BSP
{
    /// <summary>
    /// Modified version of Valve's Source 1 .bsp map format.
    /// </summary>
    public class BSPFormat
    {
        public BSPHeader Header;

        public BSPFormat()
        {
            Header = new BSPHeader();

            int ident = ('P' << 24) | ('S' << 16) | ('B' << 8) | 'V'; // VBSP Identifier

            Header.ident = ident;
            Header.version = 26;
            Header.mapRevision = 1;

            // Initialize all 64 lumps as empty first
            Header.lumps = new Lump[64];
            for (int i = 0; i < 64; i++)
            {
                Header.lumps[i] = new Lump { Version = 0 };
            }

            // Create all our lump definitions
            foreach (LumpDefinition definition in LumpDefinitions.Definitions)
            {
                Header.lumps[(int)definition.Type] = definition.Lump;
            }
        }

        /// <summary>
        /// Set Lump Data.
        /// </summary>
        public void SetLumpData(LumpType type, object data)
        {
            // Get the lump index
            int index = (int)type;

            // Set the data
            Header.lumps[index].Data = data;

            // Recalculate the file length based on the new data
            if (data != null)
            {
                int calculatedSize = Header.lumps[index].CalculateDataSize(data);
                Header.lumps[index].FileLength = calculatedSize;

                // Debug output
                Console.WriteLine($"Set lump data for type {type} to: {data}. Calculated size: {calculatedSize} bytes");
            }
            else
            {
                Header.lumps[index].FileLength = 0;
            }
        }

        public void RecalculateLumpOffsets()
        {
            int currentOffset = sizeof(int) * 4 + (64 * 16); // Start after header + lump directory

            for (int i = 0; i < Header.lumps.Length; i++)
            {
                if (Header.lumps[i].FileLength > 0 && Header.lumps[i].Data != null)
                {
                    // Align offset to 4-byte boundary
                    currentOffset = (currentOffset + 3) & ~3;

                    // Create a temporary copy with the updated offset
                    Lump updatedLump = Header.lumps[i];
                    updatedLump.FileOffset = currentOffset;

                    // Replace the original lump with the updated one
                    Header.lumps[i] = updatedLump;

                    // Move to the next position
                    currentOffset += Header.lumps[i].FileLength;
                }
            }
        }
    }



    /// <summary>
    /// BSP Header, stores information about the .bsp.
    /// </summary>
    public struct BSPHeader
    {
        public int ident; // BSP file identifier
        public int version; // BSP file version
        public Lump[] lumps; // Lump array
        public int mapRevision; // The map's revision (iteration, version) number
    }

    /// <summary>
    /// Definition for a Lump in a BSP.
    /// </summary>
    public struct Lump
    {
        private static int curOffset = sizeof(int) * 4 + (64 * 16); // Start after header + lump directory

        public int FileOffset;
        public int FileLength;
        public int Version;
        public char[] FourCC;
        public object Data;

        public Type DataType;

        public Lump(Type dataType, object data)
        {
            // Align offset to 4-byte boundary
            curOffset = (curOffset + 3) & ~3;
            FileOffset = curOffset;

            // Assign the Lump's Data Type
            DataType = dataType;

            // Calculate actual data size
            FileLength = CalculateDataSize(data);
            Data = data;
            Version = 0;

            FourCC = new char[] { '\0', '\0', '\0', '\0' };

            curOffset += FileLength;
        }

        public int CalculateDataSize(object data)
        {
            if (data == null) return 0;

            return data switch
            {
                byte[] byteArray => byteArray.Length,
                int[] intArray => intArray.Length * sizeof(int),
                float[] floatArray => floatArray.Length * sizeof(float),
                uint[] uintArray => uintArray.Length * sizeof(uint),
                string[] stringArray => stringArray.Sum(str =>
                    str != null ? Encoding.ASCII.GetByteCount(str) + 1 : 1), // +1 for each null terminator
                string stringData => Encoding.ASCII.GetByteCount(stringData) + 1, // +1 for null terminator
                _ => throw new InvalidOperationException($"Unsupported data type: {data.GetType().Name}")
            };
        }



    }
}
