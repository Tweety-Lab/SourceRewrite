using FileFormats.MDL;
using FileFormats.MDL.VVD;
using Silk.NET.Assimp;
using SourceRewrite.Files;

namespace SourceRewrite.AssetTypes
{
    /// <summary>
    /// Mesh Class, Constructed from filePath to model.
    /// </summary>
    public class Model : Mesh
    {
        public Texture Texture;
        public Shader Shader;

        // Create a Model from file path
        public unsafe Model(string filePath, Material material)
        {
            // If Mesh cant be found, set it to ERROR
            if (!System.IO.File.Exists(filePath))
            {
                DeveloperConsole.Warning($"Could not find model at '{filePath}'");
                filePath = FileSystem.GetModelPath("dev/error.model");
                material = FileSystem.GetMaterial("dev/error");
            }

            Material = material;
            Shader = Material.Shader;

            if (filePath.EndsWith(".mdl"))
            {
                // Extract vertices, normals, and texture coordinates as separate arrays
                List<float> vertexData = new();
                List<float> normalData = new();
                List<float> uvData = new();

                MDLFormat MDL = new MDLFormat(filePath);
                Console.WriteLine("MDL Data:");
                Console.WriteLine(MDL.Header.Version);
                Console.WriteLine(MDL.Header.Name);
                Console.WriteLine(MDL.Header.ID);
                Console.WriteLine(MDL.Header.Checksum);

                Console.WriteLine("VVD Data:");
                Console.WriteLine(MDL.VVD.Header.Version);
                Console.WriteLine(MDL.VVD.Header.ID);
                Console.WriteLine(MDL.VVD.Header.Checksum);

                // For an MDL quad, ensure consistent coordinate system and proper winding
                foreach (VVDVertex vertex in MDL.VVD.Vertices)
                {
                    // Add position
                    vertexData.Add(vertex.Position.X);
                    vertexData.Add(vertex.Position.Z); // Y up to Z up conversion
                    vertexData.Add(-vertex.Position.Y); // Negate Y for correct handedness

                    // normals
                    normalData.Add(vertex.Normal.X);
                    normalData.Add(vertex.Normal.Z);
                    normalData.Add(-vertex.Normal.Y);
                }

                // Set all the data
                Vertices = vertexData.ToArray();
                Normals = normalData.ToArray();
                UVs = uvData.ToArray();

                // EXAMPLE INDICES
                Indices = new uint[]
                {
                    0, 2, 1,  // First triangle (top-right, top-left, bottom-right)
                    1, 2, 3   // Second triangle (bottom-right, top-left, bottom-left)
                };
            }
            else
            {
                // Use Assimp to load generic mesh types
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
}