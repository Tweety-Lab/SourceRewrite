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
    public enum BSPLumpType
    {
        LUMP_SIDES = 0,
        LUMP_ENTITIES = 1
    }

    // Lump-specific structs


    [MessagePackObject]
    public struct BSPSide
    {
        [Key(0)] public List<Vector3> Vertices;
        [Key(1)] public List<int> Indices;

        [Key(2)] public string MaterialName;
        [Key(3)] public int ID;
    }

    [MessagePackObject]
    public struct BSPEntity
    {
        [Key(0)] public string KeyValuesString;
    }
}
