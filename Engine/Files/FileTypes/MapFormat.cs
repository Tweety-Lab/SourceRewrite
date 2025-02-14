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
        public BSPHeader Header;

        public MapFormat()
        {
            Header = new BSPHeader();

            int ident = ('V' << 24) | ('B' << 16) | ('S' << 8) | 'P'; // VBSP Identifier

            // Adjust for little-endian systems
            if (BitConverter.IsLittleEndian)
            {
                ident = BitConverter.ToInt32(BitConverter.GetBytes(ident).Reverse().ToArray(), 0);
            }

            Header.ident = ident;

            Header.version = 26; // Set the unique version (we use 26 for now)

            Header.mapRevision = 1;

            // Create our lumps TODO: Automate this
            Header.lumps = [
                new Lump{version = 1},
                new Lump{version = 1}
            ];
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
        private static int curOffset = 0;

        public int fileofs;      // offset into file (bytes)
        public int filelen;      // length of lump (bytes)
        public int version;      // lump format version
        char[] fourCC; // lump ident code


        public object Data; // Holds lump-specific data (could be vertices, textures, etc.)

        // Static List to store all Lumps.
        public static List<Lump> Lumps = new List<Lump>();

        // Static method to add a Lump to the list
        public static void AddLump(Lump lump)
        {
            Lumps.Add(lump);
        }

        // Constructor for Lump
        public Lump(object data)
        {
            fileofs = curOffset;
            filelen = 256;
            Data = data;

            // Give every lump 256 space
            curOffset += 256;

            fourCC = new char[4];    // lump ident code

            // Add this lump to the static list
            AddLump(this);

        }

        // Lump to represent vertices
        public static Lump LUMP_VERTEXES = new Lump(new float[]
        {
            // Positions            // Texture Coordinates (u, v)
            -0.5f, -0.5f, -0.5f,   0.0f, 0.0f,  // Front-bottom-left
             0.5f, -0.5f, -0.5f,   1.0f, 0.0f,  // Front-bottom-right
             0.5f,  0.5f, -0.5f,   1.0f, 1.0f,  // Front-top-right
            -0.5f,  0.5f, -0.5f,   0.0f, 1.0f,  // Front-top-left
            -0.5f, -0.5f,  0.5f,   0.0f, 0.0f,  // Back-bottom-left
             0.5f, -0.5f,  0.5f,   1.0f, 0.0f,  // Back-bottom-right
             0.5f,  0.5f,  0.5f,   1.0f, 1.0f,  // Back-top-right
            -0.5f,  0.5f,  0.5f,   0.0f, 1.0f   // Back-top-left
        });

        // Lump to represent indices ( not in bsps under v26 )
        public static Lump LUMP_INDICES = new Lump(new uint[]
        {
    0, 1, 2,  0, 2, 3,  // Front face
    4, 5, 6,  4, 6, 7,  // Back face
    0, 1, 5,  0, 5, 4,  // Bottom face
    2, 3, 7,  2, 7, 6,  // Top face
    0, 3, 7,  0, 7, 4,  // Left face
    1, 2, 6,  1, 6, 5   // Right face
        });
    }

}
