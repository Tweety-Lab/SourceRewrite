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
            float[] vertices = (float[])bspMap.Header.lumps[0].Data;
            uint[] indices = (uint[])bspMap.Header.lumps[1].Data;

            // Create a Material
            Material material = FileSystem.GetMaterial("bricks.vmt");

            // Create a Mesh
            Mesh = new Mesh(vertices, indices, material);
        }
    }
}
