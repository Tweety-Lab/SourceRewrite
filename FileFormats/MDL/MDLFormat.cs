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
        public List<string> TextureNames = new List<string>();
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

                // Read Model Flags
                Header.Flags = (MDLFlags)reader.ReadUInt32();

                // Read Offsets
                Header.BoneCount = reader.ReadInt32();
                Header.BoneOffset = reader.ReadInt32();

                Header.BoneControllerCount = reader.ReadInt32();
                Header.BoneControllerOffset = reader.ReadInt32();

                Header.HitboxCount = reader.ReadInt32();
                Header.HitboxOffset = reader.ReadInt32();

                Header.LocalAnimCount = reader.ReadInt32();
                Header.LocalAnimOffset = reader.ReadInt32();

                Header.LocalSequenceCount = reader.ReadInt32();
                Header.LocalSequenceOffset = reader.ReadInt32();

                Header.ActivityListVersion = reader.ReadInt32();
                Header.EventsIndexed = reader.ReadInt32();

                Header.TextureCount = reader.ReadInt32();
                Header.TextureOffset = reader.ReadInt32();

                // This Offset Points to a series of ints
                // Each int value, in turn, is an offset relative to the start of the file
                // at which there is a null-terminated string
                Header.TextureDirCount = reader.ReadInt32();
                Header.TextureDirOffset = reader.ReadInt32();

                Header.SkinReferenceCount = reader.ReadInt32();
                Header.SkinFamilyCount = reader.ReadInt32();
                Header.SkinReferenceIndex = reader.ReadInt32();

                Header.BodyPartCount = reader.ReadInt32();
                Header.BodyPartOffset = reader.ReadInt32();

                Header.AttachmentCount = reader.ReadInt32();
                Header.AttachmentOffset = reader.ReadInt32();
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
