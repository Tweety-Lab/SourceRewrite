using System.Numerics;
using SourceFormats.KeyValues;

namespace SourceFormats.VMF
{
    // Version information
    public struct VersionInfo
    {
        public int EditorVersion;
        public int EditorBuild;
        public int MapVersion;
        public int FormatVersion;
    }

    // World
    public struct World
    {
        public int ID;
        public int MapVersion;
        public List<VMFSolid> Solids;
    }

    // Solid Brush
    public struct VMFSolid
    {
        public int ID;
        public List<VMFSide> Sides;
    }

    // Solid Brush Side
    public struct VMFSide
    {
        public int ID;
        public VMFPlane Plane;
        public string Material;

        public VMFUVAxis UAxis;
        public VMFUVAxis VAxis;
    }

    // Solid Brush Side Plane
    public struct VMFPlane
    {
        public Vector3 Point1;
        public Vector3 Point2;
        public Vector3 Point3;
    }

    public struct VMFUVAxis
    {
        public Vector4 Vector;
        public float Scale;
    }

    // Entity
    public struct VMFEntity
    {
        public int ID;
        public string ClassName;
        public string TargetName;
        public Vector3 Origin;
        public Vector3 Angles;
        public List<KeyValue> Properties;
        public List<KeyValue> Connections;
        public List<VMFSolid> Solids; // Brush Entity Solids
    }
}
