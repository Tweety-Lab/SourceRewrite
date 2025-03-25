#if EDITOR
using SourceRewrite.AssetTypes;
using SourceRewrite.Entities;
using SourceRewrite.Files;
using System.Numerics;

namespace SourceRewrite.Editor
{
    /// <summary>
    /// Manage Editor Gizmos
    /// </summary>
    public static class Gizmos
    {
        /// <summary>
        /// Determines what Color to render new Gizmos with (0-255)
        /// </summary>
        public static Vector4 Color = new Vector4(255, 255, 255, 1);

        /// <summary>
        /// Draws a Cube and returns its MeshEntity
        /// </summary>
        /// <param name="position">Center position of the cube</param>
        /// <param name="size">Size of the cube in each dimension</param>
        public static MeshEntity DrawCube(Vector3 position, Vector3 size)
        {
            // Calculate a unique key for each mesh (e.g., based on position and size)
            int meshKey = position.GetHashCode() ^ size.GetHashCode();

            // Calculate half size for offsetting vertices from center
            Vector3 halfSize = new Vector3(size.X / 2, size.Y / 2, size.Z / 2);

            // Create a Mesh
            Mesh mesh = new Mesh();
            mesh.Material = FileSystem.GetMaterial("dev/gizmo");
            mesh.Material.Shader.SetParameter("tint", Color);

            mesh.Vertices = new float[24]; // 8 vertices, each with 3 components (X, Y, Z)
            mesh.Indices = new uint[36]; // 12 triangles, 3 indices per triangle

            #region PopulateVertices
            // Populate Vertices with Cube vertices, offsetting from center position
            // Vertex 0: front bottom left
            mesh.Vertices[0] = position.X - halfSize.X;
            mesh.Vertices[1] = position.Y - halfSize.Y;
            mesh.Vertices[2] = position.Z - halfSize.Z;

            // Vertex 1: front bottom right
            mesh.Vertices[3] = position.X + halfSize.X;
            mesh.Vertices[4] = position.Y - halfSize.Y;
            mesh.Vertices[5] = position.Z - halfSize.Z;

            // Vertex 2: front top right
            mesh.Vertices[6] = position.X + halfSize.X;
            mesh.Vertices[7] = position.Y + halfSize.Y;
            mesh.Vertices[8] = position.Z - halfSize.Z;

            // Vertex 3: front top left
            mesh.Vertices[9] = position.X - halfSize.X;
            mesh.Vertices[10] = position.Y + halfSize.Y;
            mesh.Vertices[11] = position.Z - halfSize.Z;

            // Vertex 4: back bottom left
            mesh.Vertices[12] = position.X - halfSize.X;
            mesh.Vertices[13] = position.Y - halfSize.Y;
            mesh.Vertices[14] = position.Z + halfSize.Z;

            // Vertex 5: back bottom right
            mesh.Vertices[15] = position.X + halfSize.X;
            mesh.Vertices[16] = position.Y - halfSize.Y;
            mesh.Vertices[17] = position.Z + halfSize.Z;

            // Vertex 6: back top right
            mesh.Vertices[18] = position.X + halfSize.X;
            mesh.Vertices[19] = position.Y + halfSize.Y;
            mesh.Vertices[20] = position.Z + halfSize.Z;

            // Vertex 7: back top left
            mesh.Vertices[21] = position.X - halfSize.X;
            mesh.Vertices[22] = position.Y + halfSize.Y;
            mesh.Vertices[23] = position.Z + halfSize.Z;

            #endregion

            #region PopulateIndices
            // Populate Indices for the 12 triangles
            mesh.Indices[0] = 0; mesh.Indices[1] = 2; mesh.Indices[2] = 1;  // Front face
            mesh.Indices[3] = 0; mesh.Indices[4] = 3; mesh.Indices[5] = 2;
            mesh.Indices[6] = 4; mesh.Indices[7] = 5; mesh.Indices[8] = 6;  // Back face
            mesh.Indices[9] = 4; mesh.Indices[10] = 6; mesh.Indices[11] = 7;
            mesh.Indices[12] = 0; mesh.Indices[13] = 1; mesh.Indices[14] = 5;  // Bottom face
            mesh.Indices[15] = 0; mesh.Indices[16] = 5; mesh.Indices[17] = 4;
            mesh.Indices[18] = 2; mesh.Indices[19] = 3; mesh.Indices[20] = 7;  // Top face
            mesh.Indices[21] = 2; mesh.Indices[22] = 7; mesh.Indices[23] = 6;
            mesh.Indices[24] = 0; mesh.Indices[25] = 4; mesh.Indices[26] = 7;  // Left face
            mesh.Indices[27] = 0; mesh.Indices[28] = 7; mesh.Indices[29] = 3;
            mesh.Indices[30] = 1; mesh.Indices[31] = 2; mesh.Indices[32] = 6;  // Right face
            mesh.Indices[33] = 1; mesh.Indices[34] = 6; mesh.Indices[35] = 5;
            #endregion

            // Create a MeshEntity
            MeshEntity meshEntity = new MeshEntity();
            meshEntity.Mesh = mesh;

            return meshEntity;
        }

        /// <summary>
        /// Draws a Sphere and returns its MeshEntity
        /// </summary>
        /// <param name="position">Center position of the sphere</param>
        /// <param name="radius">Radius of the sphere</param>
        /// <param name="segments">Number of segments (resolution of the sphere)</param>
        public static MeshEntity DrawSphere(Vector3 position, float radius, int segments = 16)
        {
            // Create a Mesh
            Mesh mesh = new Mesh();
            mesh.Material = FileSystem.GetMaterial("dev/gizmo");
            mesh.Material.Shader.SetParameter("tint", Color);

            // Add Empty UVs
            mesh.UVs = new float[1] { 0 };

            // Add Empty Normals
            mesh.Normals = new float[1] { 0 };

            // Calculate the number of vertices and indices needed
            int numVertices = (segments + 1) * (segments + 1);
            int numIndices = segments * segments * 6;

            mesh.Vertices = new float[numVertices * 3]; // 3 components (X, Y, Z) per vertex
            mesh.Indices = new uint[numIndices];

            #region PopulateVertices
            // Generate vertices for UV sphere
            int vertexIndex = 0;
            for (int y = 0; y <= segments; y++)
            {
                float yProgress = (float)y / segments; // 0.0 to 1.0
                float yAngle = yProgress * MathF.PI; // 0 to π (from top to bottom)

                for (int x = 0; x <= segments; x++)
                {
                    float xProgress = (float)x / segments; // 0.0 to 1.0
                    float xAngle = xProgress * MathF.PI * 2; // 0 to 2π (around the sphere)

                    // Calculate the point on a unit sphere
                    float xPos = MathF.Sin(yAngle) * MathF.Cos(xAngle);
                    float yPos = MathF.Cos(yAngle);
                    float zPos = MathF.Sin(yAngle) * MathF.Sin(xAngle);

                    // Scale by radius and add center position
                    mesh.Vertices[vertexIndex++] = position.X + xPos * radius;
                    mesh.Vertices[vertexIndex++] = position.Y + yPos * radius;
                    mesh.Vertices[vertexIndex++] = position.Z + zPos * radius;
                }
            }
            #endregion

            #region PopulateIndices
            // Generate indices for the triangles
            int indexIndex = 0;
            for (int y = 0; y < segments; y++)
            {
                for (int x = 0; x < segments; x++)
                {
                    // Calculate the indices of the quad vertices
                    uint i1 = (uint)(y * (segments + 1) + x);
                    uint i2 = (uint)(y * (segments + 1) + x + 1);
                    uint i3 = (uint)((y + 1) * (segments + 1) + x);
                    uint i4 = (uint)((y + 1) * (segments + 1) + x + 1);

                    // First triangle
                    mesh.Indices[indexIndex++] = i1;
                    mesh.Indices[indexIndex++] = i3;
                    mesh.Indices[indexIndex++] = i2;

                    // Second triangle
                    mesh.Indices[indexIndex++] = i2;
                    mesh.Indices[indexIndex++] = i3;
                    mesh.Indices[indexIndex++] = i4;
                }
            }
            #endregion

            // Create a MeshEntity
            MeshEntity meshEntity = new MeshEntity();
            meshEntity.Mesh = mesh;

            return meshEntity;
        }
    }
}

#endif
