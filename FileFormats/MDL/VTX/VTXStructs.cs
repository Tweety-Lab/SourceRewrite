using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

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

    public class VTXBodyPart
    {
        public int NumModels;
        public int ModelOffset;
        public List<VTXModel> Models = new List<VTXModel>();
    }

    public class VTXModel
    {
        public int NumLODs;
        public int LodOffset;
        public List<VTXModelLOD> LODs = new List<VTXModelLOD>();
    }

    public class VTXModelLOD
    {
        public int NumMeshes;
        public int MeshOffset;
        public float SwitchPoint;
        public List<VTXMesh> Meshes = new List<VTXMesh>();
    }

    [Flags]
    public enum StripGroupFlags
    {
        STRIPGROUP_IS_FLEXED = 0x01,
        STRIPGROUP_IS_HWSKINNED = 0x02,
        STRIPGROUP_IS_DELTA_FLEXED = 0x04,
        STRIPGROUP_SUPPRESS_HW_MORPH = 0x08
    }

    public class VTXMesh
    {
        public int NumStripGroups;
        public int StripGroupHeaderOffset;
        public StripGroupFlags Flags;
        public List<VTXStripGroup> StripGroups = new List<VTXStripGroup>();
    }

    public class VTXStripGroup
    {
        public int NumVerts;
        public int VertOffset;
        public int NumIndices;
        public int IndexOffset;
        public int NumStrips;
        public int StripOffset;
        public StripGroupFlags Flags;
        public int NumTopologyIndices;
        public int TopologyOffset;

        public List<VTXVertex> Vertices = new List<VTXVertex>();
        public List<ushort> Indices = new List<ushort>();
        public List<VTXStrip> Strips = new List<VTXStrip>();
    }

    [Flags]
    public enum StripFlags
    {
        IS_TRIFAN = 0x01,
        IS_TRISTRIP = 0x02
    }

    public class VTXStrip
    {
        public int NumIndices;
        public int IndexOffset;
        public int NumVerts;
        public int VertOffset;
        public short NumBones;
        public StripFlags Flags;
        public int NumBoneStateChanges;
        public int BoneStateChangeOffset;
        public int NumTopologyIndices;
        public int TopologyOffset;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct VTXVertex
    {
        public byte BoneWeightIndex;
        public byte NumBones;
        public ushort OriginalMeshVertexID;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public byte[] BoneID;
    }

    public class VTXFile
    {
        public VTXHeader Header;
        public List<VTXBodyPart> BodyParts = new List<VTXBodyPart>();
    }
}