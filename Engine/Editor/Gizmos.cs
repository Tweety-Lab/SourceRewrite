#if EDITOR
using SourceRewrite.AssetTypes;
using SourceRewrite.Entities;
using SourceRewrite.Files;
using SourceRewrite.Windowing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SourceRewrite.Editor
{
    /// <summary>
    /// Manage Editor Gizmos
    /// </summary>
    public static class Gizmos
    {
        /// <summary>
        /// Determines what Color to render new Gizmos with
        /// </summary>
        public static Vector4 Color = new Vector4(1, 1, 1, 1);

        /// <summary>
        /// Draws a Cube
        /// </summary>
        /// <param name="position">Center position of the cube</param>
        /// <param name="size">Size of the cube in each dimension</param>
        public static void DrawCube(Vector3 position, Vector3 size)
        {
            // Calculate half size for offsetting vertices from center
            Vector3 halfSize = new Vector3(size.X / 2, size.Y / 2, size.Z / 2);

            // Create a Mesh
            Mesh mesh = new Mesh();
            mesh.Material = FileSystem.GetMaterial("dev/gizmo");
            mesh.Vertices = new float[24]; // 8 vertices, each with 3 components (X, Y, Z)
            mesh.Indices = new uint[36]; // 12 triangles, 3 indices per triangle

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

            // Create a Mesh Entity
            MeshEntity meshEntity = new MeshEntity();
            meshEntity.Mesh = mesh;
        }
    }
}

#endif
