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

            BSPFormat bspMap = reader.ReadFromMap();
            float[] vertices = bspMap.GetLumpData<float[]>(Lump.LumpType.LUMP_VERTEXES); // Load Vertices Lump
            uint[] indices = bspMap.GetLumpData<uint[]>(Lump.LumpType.LUMP_INDICES); // Load Indices Lump

            // Create a Material
            Material material = FileSystem.GetMaterial("bricks.vmt");

            // Create a Mesh
            Mesh = new Mesh(vertices, indices, material);
        }
    }
}
