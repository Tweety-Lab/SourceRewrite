using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileFormats.MDL.VTX
{
    /// <summary>
    /// Valve's Source 1 .vtx mesh strip format.
    /// </summary>
    public class VTXFormat
    {
        public VTXHeader Header;
        public VTXFormat(string path)
        {
            // Load the VTX
            Header = new VTXHeader();

            // Start reading the file
            using (var reader = new BinaryReader(File.Open(path, FileMode.Open)))
            {
                // Read the header
                Header.Version = reader.ReadInt32();

                Header.VertCacheSize = reader.ReadInt32();
                Header.MaxBonesPerStrip = reader.ReadInt16();
                Header.MaxBonesPerTri = reader.ReadInt16();
                Header.MaxBonesPerVert = reader.ReadInt32();

                Header.Checksum = reader.ReadInt32();

                Header.NumLODs = reader.ReadInt32();

                Header.MaterialReplacementListOffset = reader.ReadInt32();

                Header.NumBodyParts = reader.ReadInt32();
                Header.BodyPartOffset = reader.ReadInt32();
            }
        }
    }
}
