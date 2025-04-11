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

    // Strip Group Flags Flags
    // We map these to the bits that define them in the .VTX File.
    [Flags]
    public enum StripGroupFlags
    {
        STRIPGROUP_IS_FLEXED = 0x01,
        STRIPGROUP_IS_HWSKINNED = 0x02,
        STRIPGROUP_IS_DELTA_FLEXED = 0x04,
        STRIPGROUP_SUPPRESS_HW_MORPH = 0x08
    }

    public struct VTXMeshHeader
    {
        public int NumStripGroups;
        public int StripGroupHeaderOffset;

        public StripGroupFlags Flags;
    }

    public struct VTXStripGroupHeader
    {
        public int NumVerts;
        public int VertOffset;

        public int NumIndices;
        public int IndexOffset;

        public int NumStrips;
        public int StripOffset;

        public StripGroupFlags Flags;

        // V49 stuff
        public int NumTopologyIndices;
        public int TopologyOffset;
    }
}
