using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileFormats.BSP.IO
{
    public class BSPReader
    {
        /// <summary>
        /// The Output BSP.
        /// </summary>
        public BSPFormat BSP { get; private set; }

        public BSPReader(string path)
        {
            BSP = new BSPFormat();

            // Read the Binary file
            using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read))
            using (var reader = new BinaryReader(stream, Encoding.UTF8))
            {
                BSP.Header = ReadHeader(reader);
                BSP.Header.Lumps = ReadLumps(reader);
            }
        }

        /// <summary>
        /// Reads the BSP header.
        /// </summary>
        /// <param name="reader"></param>
        private BSPHeader ReadHeader(BinaryReader reader)
        {
            BSPHeader header = new BSPHeader();
            header.Identifier = Encoding.ASCII.GetString(reader.ReadBytes(4)).TrimEnd('\0');
            header.Version = reader.ReadInt32();

            return header;
        }

        /// <summary>
        /// Reads the BSP lumps.
        /// </summary>
        /// <param name="reader"></param>
        private BSPLump[] ReadLumps(BinaryReader reader)
        {
            List<BSPLump> lumps = new List<BSPLump>();

            for (int i = 0; i < 64; i++)
            {
                BSPLump lump = new BSPLump();
                lump.Type = (BSPLumpType)i;
                lump.Offset = reader.ReadInt32();
                lump.Length = reader.ReadInt32();
                lumps.Add(lump);
            }

            return lumps.ToArray();
        }
    }
}
