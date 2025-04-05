using MessagePack;
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
    [MessagePackObject]
    public struct SideData
    {
        [Key(0)] public float X;
        [Key(1)] public float Y;
        [Key(2)] public float Z;

        [Key(3)] public string MaterialName;
        [Key(4)] public int ID;
    }

    [MessagePackObject]
    public struct EntityLumpData
    {
        [Key(0)] public string EntityString;
    }
}
