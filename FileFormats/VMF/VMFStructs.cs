using FileFormats.KeyValues;
using System.Numerics;

namespace FileFormats.VMF
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
        public List<Solid> Solids;
    }

    // Solid Brush
    public struct Solid
    {
        public int ID;
        public List<Side> Sides;
    }

    // Solid Brush Side
    public struct Side
    {
        public int ID;
        public Plane Plane;
        public string Material;

        public UVAxis UAxis;
        public UVAxis VAxis;
    }

    // Side UV Axis
    public struct UVAxis
    {
        public Vector4 UVDir;
        public float UVScale;
    }

    // Solid Brush Side Plane
    public struct Plane
    {
        public Vector3 Corner1;
        public Vector3 Corner2;
        public Vector3 Corner3;
    }

    // Entity
    public struct Entity
    {
        public int ID;
        public string ClassName;
        public Vector3 Origin;
        public List<KeyValue> Properties;
    }
}
