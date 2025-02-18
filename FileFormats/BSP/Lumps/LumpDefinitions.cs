using Sledge.Formats.Texture.Wad;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileFormats.BSP
{
    public static class LumpDefinitions
    {
        ////////////////////////////////////////////////////////
        // LUMP DEFINITONS
        ////////////////////////////////////////////////////////
        public static LumpDefinition[] Definitions = {

            //                 Type            Data          Lump Type
            new LumpDefinition(typeof(string), "bricks.vmt", Lump.LumpType.LUMP_MATERIAL), // Materials
            new LumpDefinition(typeof(float[]), new float[] {     // Front face
                -0.5f, -0.5f, -0.5f, 0.0f, 0.0f,  // Bottom-left
                0.5f, -0.5f, -0.5f, 1.0f, 0.0f,  // Bottom-right
                0.5f, 0.5f, -0.5f, 1.0f, 1.0f,  // Top-right
                -0.5f, 0.5f, -0.5f, 0.0f, 1.0f,  // Top-left

                // Back face
                -0.5f, -0.5f, 0.5f, 0.0f, 0.0f,  // Bottom-left
                0.5f, -0.5f, 0.5f, 1.0f, 0.0f,  // Bottom-right
                0.5f, 0.5f, 0.5f, 1.0f, 1.0f,  // Top-right
                -0.5f, 0.5f, 0.5f, 0.0f, 1.0f,  // Top-left

                // Left face
                -0.5f, -0.5f, -0.5f, 0.0f, 0.0f,  // Bottom-left
                -0.5f, 0.5f, -0.5f, 1.0f, 0.0f,  // Top-left
                -0.5f, 0.5f, 0.5f, 1.0f, 1.0f,  // Top-right
                -0.5f, -0.5f, 0.5f, 0.0f, 1.0f,  // Bottom-right

                // Right face
                0.5f, -0.5f, -0.5f, 0.0f, 0.0f,  // Bottom-left
                0.5f, 0.5f, -0.5f, 1.0f, 0.0f,  // Top-left
                0.5f, 0.5f, 0.5f, 1.0f, 1.0f,  // Top-right
                0.5f, -0.5f, 0.5f, 0.0f, 1.0f,  // Bottom-right

                // Top face
                -0.5f, 0.5f, -0.5f, 0.0f, 0.0f,  // Bottom-left
                0.5f, 0.5f, -0.5f, 1.0f, 0.0f,  // Bottom-right
                0.5f, 0.5f, 0.5f, 1.0f, 1.0f,  // Top-right
                -0.5f, 0.5f, 0.5f, 0.0f, 1.0f,  // Top-left

                // Bottom face
                -0.5f, -0.5f, -0.5f, 0.0f, 0.0f,  // Bottom-left
                0.5f, -0.5f, -0.5f, 1.0f, 0.0f,  // Bottom-right
                0.5f, -0.5f, 0.5f, 1.0f, 1.0f,  // Top-right
                -0.5f, -0.5f, 0.5f, 0.0f, 1.0f   // Top-left }, Lump.LumpType.LUMP_VERTEXES), // Vertices

            }, Lump.LumpType.LUMP_VERTEXES), // Vertices
            new LumpDefinition(typeof(uint[]), new uint[] {     // Front face
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
    20, 21, 22,  20, 22, 23 }, Lump.LumpType.LUMP_INDICES) // Indices

        };
    };

    // Used to define Lumps
    public class LumpDefinition
    {
        public Lump Lump;
        public Lump.LumpType Type;

        public LumpDefinition(Type type, object data, Lump.LumpType lumpType)
        {
            Lump = new Lump(type, data);
            Type = lumpType;
        }

    }
}
