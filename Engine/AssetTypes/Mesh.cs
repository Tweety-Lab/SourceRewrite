using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Silk.NET.Assimp;
using SourceRewrite.Rendering;
using SourceRewrite.Files;

namespace SourceRewrite.AssetTypes
{
    /// <summary>
    /// Mesh Class, Constructed from filePath to model.
    /// </summary>
    public class Mesh : MeshAsset
    {
        public Rendering.Texture Texture;
        public Shader Shader;

        // Create a Mesh from file path
        public unsafe Mesh(string filePath, Material material)
        {
            // If Mesh cant be found, set it to ERROR
            if (!System.IO.File.Exists(filePath))
            {
                Console.WriteLine($"Could not find model at '{filePath}'");
                filePath = FileSystem.GetModelPath("dev/error.model");
                material = FileSystem.GetMaterial("dev/error.vmt");
            }

            Material = material;

            Texture = Material.Texture;
            Shader = Material.Shader;

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

                // Add texture coordinates (U, V)
                if (mesh->MTextureCoords[0] != null)
                {
                    var texCoord = mesh->MTextureCoords[0][i];
                    vertexData.Add(texCoord.X);
                    vertexData.Add(texCoord.Y);
                }
                else
                {
                    // Default texture coordinates if none are provided
                    vertexData.Add(0.0f);
                    vertexData.Add(0.0f);
                }
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

        // Create a Mesh from vertices and indices
        public Mesh(float[] vertices, uint[] indices, Material material)
        {
            Texture = material.Texture;
            Shader = material.Shader;

            Vertices = vertices;
            Indices = indices;
        }
    }
}