using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace FileFormats.BSP
{
    // Lump-specific structures
    public struct SideData
    {
        public Vector3 FaceVertices;
        public string MaterialName;
        public int ID;
    }

    public struct EntityLumpData
    {
        public string EntityString;
    }
}
