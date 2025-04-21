using System.Numerics;

namespace SourceFormats.MDL.VVD
{
    public struct VVDHeader
    {
        public int ID; // FourCC (IDSV)
        public int Version;
        public int Checksum;

        public int NumLOD; // Number of Valid LOD Models
        public int[] NumLODVertices; // Number of Vertices in the LOD
        public int NumFixups; // Number of Fixup tables

        public int FixupTableStart; // Offset to the Fixup table
        public int VertexDataStart; // Offset to the Vertices
        public int TangentDataStart; // Offset to the Tangent data
    }

    public struct VVDFixupTable
    {
        public int LOD;
        public int SourceVertexID;
        public int NumVertices;
    }

    // A Vertex stored in VVD
    // 48 BYTES
    public struct VVDVertex
    {
        public VVDBoneWeight BoneWeights;
        public Vector3 Position;
        public Vector3 Normal;
        public Vector2 TextureCoordinate;
    }

    // A bone weight stored in VVD
    // 16 BYTES
    public struct VVDBoneWeight
    {
        public float[] Weight;
        public byte[] Bone;
        public byte NumBones;
    }
}
