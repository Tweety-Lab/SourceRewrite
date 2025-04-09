using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileFormats.MDL.VTX
{
    public struct VTXHeader
    {
        public int Version;
        public int VertCacheSize;

        public short MaxBonesPerStrip;
        public short MaxBonesPerTri;
        public int MaxBonesPerVert;

        public int Checksum;

        public int NumLODs;

        public int MaterialReplacementListOffset;

        public int NumBodyParts;
        public int BodyPartOffset;
    }


    public struct VTXBodyPartHeader
    {
        public int NumModels;
        public int ModelOffset;
    }

    public struct VTXModelHeader
    {
        public int NumLODs;
        public int LodOffset;
    }

    public struct VTXModelLODHeader
    {
        public int NumMeshes;
        public int MeshOffset;

        public float SwitchPoint;
    }

    public struct VTXMeshHeader
    {
        public int NumStripGroups;
        public int StripGroupHeaderOffset;

        public byte Flags;
    }

    public struct VTXStripGroupHeader
    {
        public int NumVerts;
        public int VertOffset;

        public int NumIndices;
        public int IndexOffset;

        public int NumStrips;
        public int StripOffset;

        public byte Flags;

        // V49 stuff
        public int NumTopologyIndices;
        public int TopologyOffset;
    }
}
