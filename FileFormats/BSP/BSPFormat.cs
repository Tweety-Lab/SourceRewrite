using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

            // LUMP DEFINITONS

            // Write Brush Material, placeholder
            Header.lumps[(int)Lump.LumpType.LUMP_MATERIAL] = new Lump(typeof(string), "bricks.vmt");

            // Write Vertices
            Header.lumps[(int)Lump.LumpType.LUMP_VERTEXES] = new Lump(typeof(float[]), new float[]
            {
    // Front face
    -0.5f, -0.5f, -0.5f,  0.0f, 0.0f,  // Bottom-left
     0.5f, -0.5f, -0.5f,  1.0f, 0.0f,  // Bottom-right
     0.5f,  0.5f, -0.5f,  1.0f, 1.0f,  // Top-right
    -0.5f,  0.5f, -0.5f,  0.0f, 1.0f,  // Top-left

    // Back face
    -0.5f, -0.5f,  0.5f,  0.0f, 0.0f,  // Bottom-left
     0.5f, -0.5f,  0.5f,  1.0f, 0.0f,  // Bottom-right
     0.5f,  0.5f,  0.5f,  1.0f, 1.0f,  // Top-right
    -0.5f,  0.5f,  0.5f,  0.0f, 1.0f,  // Top-left

    // Left face
    -0.5f, -0.5f, -0.5f,  0.0f, 0.0f,  // Bottom-left
    -0.5f,  0.5f, -0.5f,  1.0f, 0.0f,  // Top-left
    -0.5f,  0.5f,  0.5f,  1.0f, 1.0f,  // Top-right
    -0.5f, -0.5f,  0.5f,  0.0f, 1.0f,  // Bottom-right

    // Right face
     0.5f, -0.5f, -0.5f,  0.0f, 0.0f,  // Bottom-left
     0.5f,  0.5f, -0.5f,  1.0f, 0.0f,  // Top-left
     0.5f,  0.5f,  0.5f,  1.0f, 1.0f,  // Top-right
     0.5f, -0.5f,  0.5f,  0.0f, 1.0f,  // Bottom-right

    // Top face
    -0.5f,  0.5f, -0.5f,  0.0f, 0.0f,  // Bottom-left
     0.5f,  0.5f, -0.5f,  1.0f, 0.0f,  // Bottom-right
     0.5f,  0.5f,  0.5f,  1.0f, 1.0f,  // Top-right
    -0.5f,  0.5f,  0.5f,  0.0f, 1.0f,  // Top-left

    // Bottom face
    -0.5f, -0.5f, -0.5f,  0.0f, 0.0f,  // Bottom-left
     0.5f, -0.5f, -0.5f,  1.0f, 0.0f,  // Bottom-right
     0.5f, -0.5f,  0.5f,  1.0f, 1.0f,  // Top-right
    -0.5f, -0.5f,  0.5f,  0.0f, 1.0f   // Top-left
            });

            // Write Vertex Indices Lump (for rendering with indexed drawing)
            Header.lumps[(int)Lump.LumpType.LUMP_INDICES] = new Lump(typeof(uint[]), new uint[]
            {
    // Front face
    0, 1, 2,  0, 2, 3,

    // Back face
    4, 5, 6,  4, 6, 7,

    // Left face
    8, 9, 10,  8, 10, 11,

    // Right face
    12, 13, 14,  12, 14, 15,

    // Top face
    16, 17, 18,  16, 18, 19,

    // Bottom face
    20, 21, 22,  20, 22, 23
            });


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
        // Lump directory indices
        public enum LumpType
        {
            LUMP_MATERIAL,          // Brush Material Path ( not in < v26 )
            LUMP_VERTEXES,          // Brush Vertices
            LUMP_INDICES            // Brush Indices ( not in < v26 )
                                    // ... continue
        }

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

        private int CalculateDataSize(object data)
        {
            return data switch
            {
                byte[] byteArray => byteArray.Length,
                int[] intArray => intArray.Length * sizeof(int),
                float[] floatArray => floatArray.Length * sizeof(float),
                uint[] uintArray => uintArray.Length * sizeof(uint),
                string stringData => Encoding.UTF8.GetByteCount(stringData) + 1, // +1 for null terminator
                _ => throw new InvalidOperationException($"Unsupported data type: {data.GetType().Name}")
            };
        }
    }
}
