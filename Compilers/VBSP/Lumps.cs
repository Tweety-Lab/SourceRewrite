using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VBSP
{
    /// <summary>
    /// Definition for a Lump in a BSP.
    /// </summary>
    public struct Lump
    {
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
            Data = data;

            // Add this lump to the static list
            AddLump(this);
        }

        // Example lump to represent vertices
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
    }
}
