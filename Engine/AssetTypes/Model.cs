using Silk.NET.Assimp;
using SourceRewrite.Rendering;
using SourceRewrite.Files;

namespace SourceRewrite.AssetTypes
{
    /// <summary>
    /// Mesh Class, Constructed from filePath to model.
    /// </summary>
    public class Model : Mesh
    {
        public Rendering.Texture Texture;
        public Shader Shader;

        // Create a Model from file path
        public unsafe Model(string filePath, Material material)
        {
            // If Mesh cant be found, set it to ERROR
            if (!System.IO.File.Exists(filePath))
            {
                Console.WriteLine($"Could not find model at '{filePath}'");
                filePath = FileSystem.GetModelPath("dev/error.model");
                material = FileSystem.GetMaterial("dev/error");
            }

            Material = material;
            Texture = Material.Textures[0];
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

            // Extract vertices, normals, and texture coordinates as separate arrays
            List<float> vertexData = new();
            List<float> normalData = new();
            List<float> uvData = new();

            for (uint i = 0; i < mesh->MNumVertices; i++)
            {
                var vertexPosition = mesh->MVertices[i];
                var vertexNormal = mesh->MNormals[i];

                // Add X, Y, Z components for position
                vertexData.Add(vertexPosition.X);
                vertexData.Add(vertexPosition.Y);
                vertexData.Add(vertexPosition.Z);

                // Add X, Y, Z components for normal
                normalData.Add(vertexNormal.X);
                normalData.Add(vertexNormal.Y);
                normalData.Add(vertexNormal.Z);

                // Add texture coordinates (U, V)
                if (mesh->MTextureCoords[0] != null)
                {
                    var texCoord = mesh->MTextureCoords[0][i];
                    uvData.Add(texCoord.X);
                    uvData.Add(texCoord.Y);
                }
                else
                {
                    // Default texture coordinates if none are provided
                    uvData.Add(0.0f);
                    uvData.Add(0.0f);
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
            Normals = normalData.ToArray();
            UVs = uvData.ToArray();
            Indices = indexData.ToArray();

            assimp.FreeScene(scene); // Cleanup
        }
    }
}