using FileFormats.BSP;
using SourceRewrite.Files;

namespace SourceRewrite.AssetTypes
{
    public class BSPMesh : MeshAsset
    {
        private BSPReader reader;

        public BSPMesh(string bspPath)
        {
            // Read the BSP
            reader = new BSPReader(bspPath);

            Vertices = reader.GetLumpData<float[]>(LumpType.LUMP_VERTEXES); // Load Vertices Lump
            Indices = reader.GetLumpData<uint[]>(LumpType.LUMP_INDICES); // Load Indices Lump

            string materialPath = reader.GetLumpData<string[]>(LumpType.LUMP_MATERIAL)[0]; // Load Material

            // Create a Material
            Material = FileSystem.GetMaterial(materialPath);

            // Free the BSP from memory
            reader.Dispose();
        }
    }
}
