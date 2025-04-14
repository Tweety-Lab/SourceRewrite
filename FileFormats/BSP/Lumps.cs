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
        LUMP_ENTITIES = 0,
        LUMP_PLANES = 1
    }

    // Lump-specific structs


    [MessagePackObject]
    public struct BSPPlane
    {
        [Key(0)] public float[] Vertices;
        [Key(1)] public uint[] Indices;

        [Key(2)] public string MaterialName;
        [Key(3)] public int ID;

        [Key(4)] public BSPUVAxis UAxis;
        [Key(5)] public BSPUVAxis VAxis;
    }

    [MessagePackObject]
    public struct BSPEntity
    {
        [Key(0)] public string KeyValuesString;

        [Key(1)] public BSPPlane[] BrushSides;
    }

    [MessagePackObject]
    public struct BSPUVAxis
    {
        [Key(0)] public Vector4 Axis;
        [Key(1)] public float Scale;
    }
}
