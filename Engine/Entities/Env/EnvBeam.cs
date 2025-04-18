using SourceRewrite.AssetTypes;
using SourceRewrite.Attributes;
using SourceRewrite.Files;
using System.Numerics;

namespace SourceRewrite.Entities.Env
{
    // Line Renderer
    [AlwaysExecute]
    [Entity("env_beam")]
    public class EnvBeam : MeshEntity
    {
        [EntityProperty("texture")]
        public string TexturePath { get; set; }

        [EntityProperty("radius")]
        public float Radius { get; set; } = 16.0f;

        [EntityProperty("targetpoint")]
        public Vector3 TargetPoint { get; set; }

        public override void Start()
        {
            Mesh newMesh = new Mesh();
            newMesh.Material = FileSystem.GetMaterial(TexturePath.Replace(".vmt", ""));

            Vector3 start = Transform.Position;
            Vector3 end = TargetPoint;

            // Direction vector from start to end
            Vector3 direction = Vector3.Normalize(end - start);
            float length = Vector3.Distance(start, end);

            // Find a perpendicular vector to direction
            Vector3 up = Vector3.UnitZ; // Z-up world
            if (Vector3.Dot(direction, up) > 0.99f) // if they're parallel
                up = Vector3.UnitX; // pick another axis

            Vector3 right = Vector3.Normalize(Vector3.Cross(direction, up)) * Radius;
            Vector3 actualUp = Vector3.Normalize(Vector3.Cross(right, direction)) * Radius;

            // Define vertices
            Vector3 v0 = start - right;           // bottom-left
            Vector3 v1 = start + right;           // bottom-right
            Vector3 v2 = end + right;             // top-right
            Vector3 v3 = end - right;             // top-left

            newMesh.Vertices = new float[]
            {
                v0.X, v0.Y, v0.Z,
                v1.X, v1.Y, v1.Z,
                v2.X, v2.Y, v2.Z,
                v3.X, v3.Y, v3.Z
            };

            newMesh.Indices = new uint[]
            {
                0, 1, 2, 0, 2, 3
            };

            // UVs based on length
            newMesh.UVs = new float[]
            {
                0, 0,
                1, 0,
                1, length / Radius,
                0, length / Radius
            };

            Mesh = newMesh;
        }
    }
}
