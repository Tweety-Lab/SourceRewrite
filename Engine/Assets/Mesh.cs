using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Silk.NET.Assimp;
using SourceRewrite.Rendering;
using SourceRewrite.Windowing;

namespace SourceRewrite.Assets
{
    public class Mesh
    {
        public Rendering.Texture Texture;
        public Shader Shader;

        public unsafe Mesh(string filePath, Material material)
        {
            Texture = material.Texture;
            Shader = material.Shader;

            // PLACEHOLDER: Use Assimp to load OBJ
            var assimp = Assimp.GetApi();

            Scene* scene = assimp.ImportFile(filePath, (uint)PostProcessSteps.Triangulate);

            if (scene == null || scene->MFlags == Silk.NET.Assimp.Assimp.SceneFlagsIncomplete || scene->MRootNode == null)
            {
                var error = assimp.GetErrorStringS();
                throw new Exception(error);
            }

            // Get the first mesh
            var mesh = scene->MMeshes[0];

            // Extract vertices
            List<float> vertexData = new();
            for (uint i = 0; i < mesh->MNumVertices; i++)
            {
                var vertexPosition = mesh->MVertices[i];

                // Add X, Y, Z components
                vertexData.Add(vertexPosition.X);
                vertexData.Add(vertexPosition.Y);
                vertexData.Add(vertexPosition.Z);
            }

            // Extract indices
            List<uint> indexData = new();
            for (uint i = 0; i < mesh->MNumFaces; i++)
            {
                var face = mesh->MFaces[i];
                // Each face is guaranteed to be a triangle due to Triangulate flag
                for (uint j = 0; j < face.MNumIndices; j++)
                {
                    indexData.Add(face.MIndices[j]);
                }
            }

            // Convert Lists to arrays
            Vertices = vertexData.ToArray();
            Indices = indexData.ToArray();

            assimp.FreeScene(scene); // Cleanup
        }

        // Array of Vertex positions
        public float[] Vertices =
        {
        };

        // Array of Indices
        public uint[] Indices =
        {
        };
    }
}
