using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SourceRewrite.Files;

namespace SourceRewrite.AssetTypes
{
    /// <summary>
    /// Base Mesh, contains Vertices, Indices and a Material.
    /// </summary>
    public class MeshAsset
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
        public Material Material = FileSystem.GetMaterial("dev/error.vmt");
    }
}
