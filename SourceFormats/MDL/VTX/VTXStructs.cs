namespace SourceFormats.MDL.VTX
{
    public struct VTXHeader
    {
        // File Version
        public int Version;

        // Hardware Params that affect how the model is optimized
        public int VertCacheSize;
        public short MaxBonesPerStrip;
        public short MaxBonesPerTri;
        public int MaxBonesPerVert;

        // Checksum
        public int Checksum;

        public int NumLODs;

        // Offset to MaterialReplacementList array, one for each lod (8 by default).
        public int MaterialReplacementListOffset;

        // Size and location of the body part array
        public int NumBodyParts;
        public int BodyPartOffset;
    }

    public struct VTXBodyPart
    {
        // Model Array
        public int NumModels;
        public int ModelOffset;
    }

    public struct VTXModel
    {
        // LOD Mesh Array
        public int NumLODs;
        public int LODOffset;
    }

    public struct VTXModelLOD
    {
        // Mesh Array
        public int NumMeshes;
        public int MeshOffset;

        public float SwitchPoint; // TODO: Figure out what this does
    }

    [Flags]
    public enum StripGroupFlags
    {
        STRIPGROUP_IS_FLEXED = 0x01,
        STRIPGROUP_IS_HWSKINNED = 0x02,
        STRIPGROUP_IS_DELTA_FLEXED = 0x04,
        STRIPGROUP_SUPPRESS_HW_MORPH = 0x08
    }

    public struct VTXMesh
    {
        // Strip Group Array
        public int NumStripGroups;
        public int StripGroupHeaderOffset;

        public StripGroupFlags Flags;
    }

    public struct VTXStripGroup
    {
        // These are the arrays of all verts and indices for this mesh, strips index into this.
        public int NumVerts;
        public int VertOffset;

        public int NumIndices;
        public int IndexOffset;

        // Strip Array
        public int NumStrips;
        public int StripOffset;

        public char Flags; // Does this use StripGroupFlags?

        // V49 Stuff
        // Points to an array of unsigned shorts (16 bits each)
        public int NumTopologyIndices;
        public int TopologyIndexOffset;

        // NEW STUFF:
        public byte[] IndexData;  // Raw index buffer data
        public byte[] VertexData; // Raw vertex buffer data
    }

    public struct VTXStrip
    {
        // Vertices and Arrays to index into the StripGroup
        public int NumIndices;
        public int IndexOffset;

        public int NumVerts;
        public int VertOffset;

        public short NumBones;

        public char Flags;

        public int NumBoneStateChanges;
        public int BoneStateChangeOffset;

        // V49 Stuff
        public int NumTopologyIndices;
        public int TopologyIndexOffset;
    }
}