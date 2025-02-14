using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VBSP
{
    /// <summary>
    /// Definition for all Lumps in a BSP.
    /// </summary>
    public static class Lumps
    {
        public static float[] LUMP_VERTEXES = [    
            // Positions            // Texture Coordinates (u, v)
        -0.5f, -0.5f, -0.5f,   0.0f, 0.0f,  // Front-bottom-left
        0.5f, -0.5f, -0.5f,   1.0f, 0.0f,  // Front-bottom-right
        0.5f,  0.5f, -0.5f,   1.0f, 1.0f,  // Front-top-right
        -0.5f,  0.5f, -0.5f,   0.0f, 1.0f,  // Front-top-left
        -0.5f, -0.5f,  0.5f,   0.0f, 0.0f,  // Back-bottom-left
        0.5f, -0.5f,  0.5f,   1.0f, 0.0f,  // Back-bottom-right
        0.5f,  0.5f,  0.5f,   1.0f, 1.0f,  // Back-top-right
        -0.5f,  0.5f,  0.5f,   0.0f, 1.0f
            ];
    }
}
