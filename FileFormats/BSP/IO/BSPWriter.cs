using FileFormats.Binary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileFormats.BSP.IO
{
    public class BSPWriter
    {
        // Path to the output BSP file
        private string outputPath;

        // BSP Format
        private BSPFormat bsp;

        public BSPWriter(string outputPath, BSPFormat bsp)
        {
            this.outputPath = outputPath;
            this.bsp = bsp;
        }

        public void WriteToFile()
        {
            // Open the output stream
            using var stream = new FileStream(outputPath, FileMode.Create, FileAccess.Write);
            using var writer = new BinaryWriter(stream, Encoding.UTF8);

            WriteHeader(writer, bsp.Header);

            foreach (var lump in bsp.Header.Lumps)
            {
                if (lump.Data == null) continue;

                WriteLump(writer, lump);
            }

            Console.WriteLine($"BSP successfully written to: {outputPath}");
            writer.Flush();
        }

        public void WriteHeader(BinaryWriter writer, BSPHeader header)
        {
            writer.Write(Encoding.ASCII.GetBytes(header.Identifier.PadRight(4, '\0')));
            writer.Write(header.Version);

            foreach (var lump in header.Lumps)
            {
                if (lump.Data == null)
                {
                    writer.Write(0); // Offset
                    writer.Write(0); // Length
                }
                else
                {
                    writer.Write(lump.Offset);
                    writer.Write(lump.Length);

                    Console.WriteLine($"Writing Lump at {lump.Offset} ({lump.Length})");
                }
            }

            writer.Write(header.MapRevision);
        }

        public void WriteLump(BinaryWriter writer, BSPLump lump)
        {
            long start = writer.BaseStream.Position;
            lump.Offset = (int)start;

            object data = lump.Data;

            Console.WriteLine($"Writing {lump.Type} ({data})");

            byte[] lumpBytes = BinarySerialization.SerializeObject(data);

            writer.Write(lumpBytes);
        }
    }
}
