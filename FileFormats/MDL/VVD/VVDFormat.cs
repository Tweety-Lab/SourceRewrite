namespace SourceFormats.MDL.VVD
{
    /// <summary>
    /// Valve's Source 1 .vvd vertex data format.
    /// </summary>
    public class VVDFormat
    {
        // Determine how many LODs are expected to be in the vvd
        public const int MAX_NUM_LODS = 8;

        public VVDHeader Header;
        public VVDVertex[] Vertices;

        public List<VVDFixupTable> VVDFixupTables;

        public VVDFormat(string filePath)
        {
            // Load the VVD
            Header = new VVDHeader();

            // Start reading the file
            using (var reader = new BinaryReader(File.Open(filePath, FileMode.Open)))
            {
                // ====== HEADER ======= //
                Header.ID = reader.ReadInt32();
                Header.Version = reader.ReadInt32();
                Header.Checksum = reader.ReadInt32();


                Header.NumLOD = reader.ReadInt32();

                // Read LOD Vertices for every potential LOD
                Header.NumLODVertices = new int[MAX_NUM_LODS];
                for (int i = 0; i < MAX_NUM_LODS; i++)
                {
                    Header.NumLODVertices[i] = reader.ReadInt32();
                }

                Header.NumFixups = reader.ReadInt32();

                Header.FixupTableStart = reader.ReadInt32();
                Header.VertexDataStart = reader.ReadInt32();
                Header.TangentDataStart = reader.ReadInt32();

                // ====== FIXUP TABLE ======= //

                // Read the fixup tables
                VVDFixupTables = new List<VVDFixupTable>();

                for (int i = 0; i < Header.NumFixups; i++)
                {
                    VVDFixupTable fixupTable = new VVDFixupTable();

                    fixupTable.LOD = reader.ReadInt32();
                    fixupTable.SourceVertexID = reader.ReadInt32();
                    fixupTable.NumVertices = reader.ReadInt32();

                    // Add the fixup table to the list
                    VVDFixupTables.Add(fixupTable);
                }

                // ====== VERTEX DATA ======= //

                int vertexCount;

                // Calculate total vertex count from all fixup tables
                if (Header.NumFixups > 0)
                {
                    vertexCount = 0;
                    foreach (var fixup in VVDFixupTables)
                    {
                        vertexCount += fixup.NumVertices;
                    }
                }
                else
                {
                    vertexCount = Header.NumLODVertices[0];
                }

                // Read the vertices for the top level LOD
                Vertices = new VVDVertex[vertexCount];

                // Move the reader to the vertex data start
                reader.BaseStream.Seek(Header.VertexDataStart, SeekOrigin.Begin);

                for (int i = 0; i < vertexCount; i++)
                {
                    VVDVertex vertex = new VVDVertex();

                    // Bone Weights
                    vertex.BoneWeights.Weight = new float[3];
                    vertex.BoneWeights.Bone = new byte[3];

                    for (int j = 0; j < 3; j++)
                    {
                        vertex.BoneWeights.Weight[j] = reader.ReadSingle();
                        vertex.BoneWeights.Bone[j] = reader.ReadByte();
                    }

                    vertex.BoneWeights.NumBones = reader.ReadByte(); // 1 byte

                    // Read the position
                    vertex.Position.X = reader.ReadSingle();
                    vertex.Position.Y = reader.ReadSingle();
                    vertex.Position.Z = reader.ReadSingle();

                    // Read the normal
                    vertex.Normal.X = reader.ReadSingle();
                    vertex.Normal.Y = reader.ReadSingle();
                    vertex.Normal.Z = reader.ReadSingle();

                    // Read the texture coordinates
                    vertex.TextureCoordinate.X = reader.ReadSingle();
                    vertex.TextureCoordinate.Y = reader.ReadSingle();

                    // Add the vertex to the array
                    Vertices[i] = vertex;
                }
            }
        }
    }
}
