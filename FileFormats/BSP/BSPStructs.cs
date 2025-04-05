using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace FileFormats.BSP
{
    public enum BSPLumpType
    {
        LUMP_SIDES = 0,
        LUMP_ENTITIES = 1,
    }

    public struct BSPHeader
    {
        public string Identifier; // "VBSP"
        public int Version; // BSP File Version
        public BSPLump[] Lumps; // Lump Dictionary
        public int MapRevision; // Iteration Number
    }

    public struct BSPLump
    {
        public int Offset;
        public int Length;
        public BSPLumpType Type;
        public object Data;
    }
}
