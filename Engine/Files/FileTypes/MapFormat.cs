using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SourceRewrite.Files.FileTypes
{
    /// <summary>
    /// Modified version of Valve's Source 1 .bsp map format.
    /// </summary>
    public class MapFormat
    {
        public BSPHeader_t Header;

        public MapFormat()
        {
            Header = new BSPHeader_t();

            int ident = ('V' << 24) | ('B' << 16) | ('S' << 8) | 'P'; // VBSP Identifier

            // Adjust for little-endian systems
            if (BitConverter.IsLittleEndian)
            {
                ident = BitConverter.ToInt32(BitConverter.GetBytes(ident).Reverse().ToArray(), 0);
            }

            Header.ident = ident;

            Header.version = 26; // Set the unique version (we use 26 for now)

            // Create our lumps TODO: Automate this
            Header.lumps = [
                new lump_t{version = 1}
            ];
        }
    }

    /// <summary>
    /// BSP Header, stores information about the .bsp.
    /// </summary>
    public struct BSPHeader_t
    {
        public int ident; // BSP file identifier
        public int version; // BSP file version
        public lump_t[] lumps; // Lump array
        public int mapRevision; // The map's revision (iteration, version) number
    }


    /// <summary>
    /// BSP Lump.
    /// </summary>
    public struct lump_t
    {
        private static int curOffset = 0;

        public int fileofs;      // offset into file (bytes)
        public int filelen;      // length of lump (bytes)
        public int version;      // lump format version
        char[] fourCC; // lump ident code
        public lump_t()
        {
            fileofs = curOffset;
            filelen = 1024;

            // Give every lump 1024 space
            curOffset += 1024;

            fourCC = new char[4];    // lump ident code
        }
    }

}
