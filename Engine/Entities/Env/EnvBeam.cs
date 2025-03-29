using SourceRewrite.AssetTypes;
using SourceRewrite.Attributes;
using SourceRewrite.Files;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SourceRewrite.Entities.Env
{
    // Line Renderer
    [AlwaysExecute]
    [Entity("env_beam")]
    public class EnvBeam : MeshEntity
    {
        [EntityProperty("texture")]
        public string TexturePath { get; set; }

        [EntityProperty("Radius")]
        public float Radius { get; set; } = 256.0f;

        [EntityProperty("targetpoint")]
        public Vector3 TargetPoint { get; set; }

        public override void Start()
        {
            Mesh newMesh = new Mesh();
            newMesh.Material = FileSystem.GetMaterial(TexturePath.Replace(".vmt", ""));

            // Calculate the direction from Position to TargetPoint
            Vector3 direction = TargetPoint - Transform.Position;

            // Calculate the horizontal distance
            float horizontalDistance = new Vector2(direction.X, direction.Z).Length();

            // Create the mesh with Radius as the vertical height
            newMesh.Vertices = new float[] {
                // Vertex 1: bottom-left
                -0.5f * horizontalDistance, 0,  0.5f * Radius,
                // Vertex 2: top-left
                -0.5f * horizontalDistance, 0, -0.5f * Radius,
                // Vertex 3: top-right
                 0.5f * horizontalDistance, 0, -0.5f * Radius,
                // Vertex 4: bottom-right
                 0.5f * horizontalDistance, 0,  0.5f * Radius
            };

            newMesh.Indices = new uint[] { 0, 1, 2, 0, 2, 3 };

            for (int i = 0; i < newMesh.Vertices.Length; i += 3)
            {
                // Adjust the vertices based on the center offset
                newMesh.Vertices[i] += Transform.Position.X;
                newMesh.Vertices[i + 1] += Transform.Position.Y;
                newMesh.Vertices[i + 2] += Transform.Position.Z;
            }

            // Set the Mesh object
            Mesh = newMesh;
        }
    }
}
