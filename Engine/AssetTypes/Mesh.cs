using SourceRewrite.Files;

namespace SourceRewrite.AssetTypes
{
    /// <summary>
    /// Base Mesh, contains Vertices, Indices and a Material.
    /// </summary>
    public class Mesh
    {
        // Array of Vertex positions
        public float[] Vertices =
        {
        };

        // Array of Indices
        public uint[] Indices =
        {
        };

        // Material
        public Material Material = FileSystem.GetMaterial("dev/error");
    }
}
