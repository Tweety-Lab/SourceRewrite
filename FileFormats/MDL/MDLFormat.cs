using FileFormats.MDL.PHY;
using FileFormats.MDL.VTX;
using FileFormats.MDL.VVD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace FileFormats.MDL
{
    /// <summary>
    /// Valve's Source 1 .mdl model format.
    /// </summary>
    public class MDLFormat
    {
        public MDLHeader Header;

        public VTXFormat VTX;
        public VVDFormat VVD;
        public PHYFormat PHY;

        public MDLFormat(string path)
        {
            // Load the Model
            Header = new MDLHeader();

            // Start reading the file
            using (var reader = new BinaryReader(File.Open(path, FileMode.Open)))
            {
                // Read the header
                Header.ID = reader.ReadInt32();
                Header.Version = reader.ReadInt32();
                Header.Checksum = reader.ReadInt32();

                // Read the name
                Header.Name = new string(reader.ReadChars(64)).Trim('\0');
                Header.DataLength = reader.ReadInt32();

                // Read Vectors, three 4-byte floats in a row
                Header.EyePosition = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                Header.IllumPosition = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                Header.HullMin = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                Header.HullMax = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                Header.ViewBBMin = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                Header.ViewBBMax = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());

                // Binary flags in little-endian order
                // ex (0x010000C0) means flags for position 0, 30, and 31 are set
                Header.Flags = (MDLFlags)reader.ReadUInt32();
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
    }
}
