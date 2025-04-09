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
            }
        }
    }
}
