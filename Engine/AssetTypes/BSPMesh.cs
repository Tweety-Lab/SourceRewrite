using FileFormats.BSP;
using SourceRewrite.Windowing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SourceRewrite.Files;

namespace SourceRewrite.AssetTypes
{
    public class BSPMesh
    {
        private BSPReader reader;

        public Mesh Mesh;
        public BSPMesh(string bspPath)
        {
            // Read the BSP
            reader = new BSPReader(bspPath);

            float[] vertices = reader.GetLumpData<float[]>(Lump.LumpType.LUMP_VERTEXES); // Load Vertices Lump
            uint[] indices = reader.GetLumpData<uint[]>(Lump.LumpType.LUMP_INDICES); // Load Indices Lump

            string materialPath = reader.GetLumpData<string>(Lump.LumpType.LUMP_MATERIAL); // Load Material

            Console.WriteLine(materialPath);

            // Create a Material
            Material material = FileSystem.GetMaterial(materialPath);

            // Create a Mesh
            Mesh = new Mesh(vertices, indices, material);
        }
    }
}
