using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileFormats.BSP.IO
{
    public class BSPReader
    {
        public BSPFormat BSP { get; private set; }

        public BSPReader(string path)
        {
            BSP = new BSPFormat();

            // Read the Binary file
            using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read))
            using (var reader = new BinaryReader(stream, Encoding.UTF8))
            {
                BSP.Header = ReadHeader(reader);
            }
        }

        /// <summary>
        /// Reads the BSP header.
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        private BSPHeader ReadHeader(BinaryReader reader)
        {
            BSPHeader header = new BSPHeader();
            header.Identifier = Encoding.ASCII.GetString(reader.ReadBytes(4)).TrimEnd('\0');
            header.Version = reader.ReadInt32();

            return header;
        }
    }
}
