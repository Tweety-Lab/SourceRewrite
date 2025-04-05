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
                }
            }

            writer.Write(header.MapRevision);
        }

        public void WriteLump(BinaryWriter writer, BSPLump lump)
        {
            long start = writer.BaseStream.Position;
            lump.Offset = (int)start;

            Array data = lump.Data;

            Console.WriteLine($"Writing {lump.Type} ({data})");

            byte[] stringBytes = BinarySerialization.SerializeObject("TESTING SERIALIZATION!");
            byte[] intBytes = BinarySerialization.SerializeObject(123);
            byte[] floatBytes = BinarySerialization.SerializeObject(999.123f);
            byte[] boolBytes = BinarySerialization.SerializeObject(true);

            byte[] endStringBytes = BinarySerialization.SerializeObject("END OF SINGULAR DATA... STARTING ARRAY DATA");
            
            float floatValue = (float)BinarySerialization.DeserializeObject(typeof(float), floatBytes);
            Console.WriteLine(floatValue);

            writer.Write(stringBytes);
            writer.Write(intBytes);
            writer.Write(floatBytes);
            writer.Write(boolBytes);

            writer.Write(endStringBytes);

            // Write the string array data
            byte[] arrayBytes = BinarySerialization.SerializeObject(new string[3] { "A", "BB", "CCC" });
            string[] array = (string[])BinarySerialization.DeserializeObject(typeof(string[]), arrayBytes);

            Console.WriteLine($"Array: {string.Join(", ", array)}");
            writer.Write(arrayBytes);

            // Write the int array data
            arrayBytes = BinarySerialization.SerializeObject(new int[3] { 1, 2, 3 });
            int[] intArray = (int[])BinarySerialization.DeserializeObject(typeof(int[]), arrayBytes);

            Console.WriteLine($"Array: {string.Join(", ", intArray)}");
            writer.Write(arrayBytes);

            // Write the float array data
            arrayBytes = BinarySerialization.SerializeObject(new float[3] { 1.1f, 2.2f, 3.3f });
            float[] floatArray = (float[])BinarySerialization.DeserializeObject(typeof(float[]), arrayBytes);

            Console.WriteLine($"Array: {string.Join(", ", floatArray)}");
            writer.Write(arrayBytes);

            // Write the bool array data
            arrayBytes = BinarySerialization.SerializeObject(new bool[3] { true, false, true });
            bool[] boolArray = (bool[])BinarySerialization.DeserializeObject(typeof(bool[]), arrayBytes);

            Console.WriteLine($"Array: {string.Join(", ", boolArray)}");
            writer.Write(arrayBytes);

            byte[] structBytes = BinarySerialization.SerializeObject(new EntityLumpData() { EntityString = "TEST STRUCT SERIALIZATION"});
            EntityLumpData entityData = (EntityLumpData)BinarySerialization.DeserializeObject(typeof(EntityLumpData), structBytes);
            Console.WriteLine(entityData.EntityString);

            writer.Write(structBytes);


            long end = writer.BaseStream.Position;
            lump.Length = (int)(end - start);
        }
    }
}
