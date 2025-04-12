using FileFormats.MDL;
using FileFormats.MDL.VVD;
using Silk.NET.Assimp;
using SourceRewrite.Files;
using System.Numerics;
using System.Security.Cryptography;

namespace SourceRewrite.AssetTypes
{
    /// <summary>
    /// Mesh Class, Constructed from filePath to model.
    /// </summary>
    public class Model : Mesh
    {
        // Create a Model from file path
        public Model(string filePath, Material material = null)
        {
            CreateModel(filePath, material);
        }

        private void CreateModel(string filePath, Material material)
        {
            // If Mesh cant be found, set it to ERROR
            if (!System.IO.File.Exists(filePath))
            {
                DeveloperConsole.Warning($"Could not find model at '{filePath}'");
                filePath = FileSystem.GetModelPath("dev/error.model");
                material = FileSystem.GetMaterial("dev/error");
            }

            if (filePath.EndsWith(".mdl"))
            {
                // Extract vertices, normals, and texture coordinates as separate arrays
                List<float> vertexData = new();
                List<float> normalData = new();
                List<float> uvData = new();

                MDLFormat MDL = new MDLFormat(filePath);

                // For an MDL quad, ensure consistent coordinate system and proper winding
                foreach (VVDVertex vertex in MDL.VVD.Vertices)
                { 
                    
                    vertexData.Add(vertex.Position.X);
                    vertexData.Add(vertex.Position.Y);
                    vertexData.Add(vertex.Position.Z);

                    // normals
                    normalData.Add(vertex.Normal.X);
                    normalData.Add(vertex.Normal.Y);
                    normalData.Add(vertex.Normal.Z);

                    uvData.Add(vertex.TextureCoordinate.X);
                    uvData.Add(vertex.TextureCoordinate.Y);
                }

                // Set all the data
                Vertices = vertexData.ToArray();

                Normals = normalData.ToArray();

                Indices = MDL.GetMeshIndices();

                UVs = uvData.ToArray();

                if (material != null)
                {
                    Material = material;
                }
                else
                {
                    // MDL Models define their own materials
                    Material = FileSystem.GetMaterial(MDL.TexturePaths[0]);
                }
            }
            else
            {
                DeveloperConsole.Warning("Tried to load a non-MDL model");
            }
        }

    }
}