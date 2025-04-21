using System.Numerics;
using System.Text;
using SourceFormats.MDL.PHY;
using SourceFormats.MDL.VTX;
using SourceFormats.MDL.VVD;

namespace SourceFormats.MDL
{
    /// <summary>
    /// Valve's Source 1 .mdl model format.
    /// </summary>
    public class MDLFormat
    {
        /// <summary>
        /// Name of the Model.
        /// </summary>
        public string Name => Header.Name;

        /// <summary>
        /// Version of the MDL.
        /// </summary>
        public int Version => Header.Version;

        /// <summary>
        /// Mass of the model in kilograms.
        /// </summary>
        public float Mass => Header.Mass;

        /// <summary>
        /// Indices of the model. Clockwise winding order.
        /// </summary>
        public uint[] MeshIndices => GetMeshIndices(MDLWindingOrder.Clockwise);

        /// <summary>
        /// Vertices of the model.
        /// </summary>
        public VVDVertex[] Vertices => VVD.Vertices;

        /// <summary>
        /// Names of requested .VMT Textures.
        /// </summary>
        public List<string> TextureNames { get; private set; }

        /// <summary>
        /// Paths of requested .VMT Textures, does not include Texture Name.
        /// </summary>
        public List<string> TexturePaths { get; private set; }

        /// <summary>
        /// Model Header.
        /// </summary>
        public MDLHeader Header;

        // Other Files
        public readonly VTXFormat VTX;
        public readonly VVDFormat VVD;
        public readonly PHYFormat PHY;

        public MDLFormat(string path)
        {

            // Start reading the file
            using (var reader = new BinaryReader(File.Open(path, FileMode.Open)))
            {
                // Load the Model
                Header = new MDLHeader(reader);

                TextureNames = GetTextureNames(reader);
                TexturePaths = GetTextureDirs(reader);
            }

            // Load other Files
            string vvdPath = path.Replace(".mdl", ".vvd");
            VVD = new VVDFormat(vvdPath);

            string vtxPath = path.Replace(".mdl", ".vtx");
            VTX = new VTXFormat(vtxPath);

            string phyPath = path.Replace(".mdl", ".phy");

            // Only read .PHY if the file exists
            if (File.Exists(phyPath))
                PHY = new PHYFormat(phyPath);
        }

        // Convert The indices to be usable
        // Help from: https://github.com/gkjohnson/source-engine-model-loader/
        public uint[] GetMeshIndices(MDLWindingOrder windingOrder)
        {
            if (VTX == null)
                return new uint[0];

            // Get the strip group and strip data
            var stripGroup = VTX.StripGroup;
            var strip = VTX.Strip;

            // Create an array to store the final indices
            uint[] meshIndices = new uint[strip.NumIndices];

            // Read the vertex data from the strip group
            for (int i = 0; i < strip.NumIndices; i++)
            {
                // Step 1: Get the raw index from the strip
                int rawIndex = strip.IndexOffset + i;

                // Step 2: Read the index from the strip group's index data
                int index2 = BitConverter.ToUInt16(stripGroup.IndexData, rawIndex * 2);

                // Step 3: Read the vertex index from the strip group's vertex data
                int index3 = BitConverter.ToUInt16(stripGroup.VertexData, index2 * 9 + 4);

                // Step 4: Apply mesh vertex offset
                int index4 = VTX.Strip.VertOffset + index3;

                // Step 5: Apply model vertex offset
                int index5 = index4 + (int)(VTX.Strip.VertOffset / 48);

                meshIndices[i] = (uint)index5;
            }

            // Reverse the array for winding order
            if (windingOrder == MDLWindingOrder.CounterClockwise)
                Array.Reverse(meshIndices);

            return meshIndices;
        }

        private List<string> GetTextureNames(BinaryReader reader)
        {
            List<string> textureNames = new List<string>();

            // Go to offset
            int currentOffset = (int)reader.BaseStream.Position;
            reader.BaseStream.Seek(Header.TextureOffset, SeekOrigin.Begin);

            // Read the texture offset
            int textureOffset = reader.ReadInt32();

            // Goto the texture offset accounting for the extra 4 bytes
            reader.BaseStream.Seek(textureOffset - 4, SeekOrigin.Current);

            // Read texture Name
            string textureName = MDLHeader.ReadNullTerminatedString(reader, 100);
            textureNames.Add(textureName);

            // Return to original position
            reader.BaseStream.Seek(currentOffset, SeekOrigin.Begin);

            return textureNames;
        }

        private List<string> GetTextureDirs(BinaryReader reader)
        {
            List<string> textureDirs = new List<string>();

            // Go to offset
            int currentOffset = (int)reader.BaseStream.Position;
            reader.BaseStream.Seek(Header.TextureDirOffset, SeekOrigin.Begin);

            // Read the texture offset
            int textureDirOffset = reader.ReadInt32();

            // Goto the texture offset accounting for the extra 4 bytes
            reader.BaseStream.Seek(textureDirOffset, SeekOrigin.Begin);

            // Read texture dirs
            string textureDir = MDLHeader.ReadNullTerminatedString(reader, 100);

            if (textureDir.EndsWith(".mdl"))
            {
                textureDirs.Add("");
            } else
            {
                textureDirs.Add(textureDir);
            }

            // Return to original position
            reader.BaseStream.Seek(currentOffset, SeekOrigin.Begin);

            return textureDirs;
        }

    }
}
