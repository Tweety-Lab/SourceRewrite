using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SourceRewrite.Objects;
using Silk.NET.Assimp;
using SourceRewrite.Rendering;
using SourceRewrite.Windowing;
using Silk.NET.Maths;

namespace SourceRewrite.Components
{
    public class MeshRenderer : GameComponent
    {

        public Rendering.Texture texture;
        public Shader shader;
        public unsafe MeshRenderer(string filePath, Rendering.Texture inputTexture, Shader inputShader)
        {
            shader = inputShader;
            texture = inputTexture;

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

            GameWindow.CurrentWindow. Renderer.InitMesh(this); // Render our mesh
            assimp.FreeScene(scene); // Cleanup
        }
        // Placeholder Array of vertex positions
        public float[] Vertices =
        {
        };

        // Placeholder Array of indices
        public uint[] Indices =
        {
        };

    }
}
