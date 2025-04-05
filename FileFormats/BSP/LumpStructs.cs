using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace FileFormats.BSP
{
    // Lump-specific structs
    public struct SideData
    {
        public float X;
        public float Y;
        public float Z;

        public string MaterialName;
        public int ID;
    }

    public struct EntityLumpData
    {
        public string EntityString;
    }
}
