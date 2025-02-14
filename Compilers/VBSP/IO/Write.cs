using SourceRewrite.Files.FileTypes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VBSP.IO
{
    public class BSPWriter
    {
        public BinaryWriter BinaryWriter;
        private string inputText;

        public BSPWriter(string outputPath, string inputPath)
        {
            BinaryWriter = new BinaryWriter(File.Open(outputPath, FileMode.Create));

            // Read the input text from the specified input file (.vmf)
            using (StreamReader reader = new StreamReader(inputPath))
            {
                inputText = reader.ReadToEnd(); // Read the entire content of the file
            }
        }
        
        /// <summary>
        /// Writes an input to BSP.
        /// </summary>
        public void Write(object input)
        {
            if (input is string str)
            {
                // Write string to binary
                BinaryWriter.Write(str);
            }
            else if (input is int integer)
            {
                // Write integer to binary
                BinaryWriter.Write(integer);
            }
            // Unsupported Type
            else
            {
                throw new InvalidOperationException("Unsupported type");
            }

            // Add to file
            BinaryWriter.Flush();
        }

        /// <summary>
        /// Writes a map to file.
        /// </summary>
        public void WriteToMap(MapFormat input)
        {
            // Write the bsp header
            BinaryWriter.Write(input.Header.ident);
            BinaryWriter.Write(input.Header.version);
            BinaryWriter.Write(input.Header.mapRevision);

            Console.WriteLine($"Writing BSP Type {input.Header.ident.ToString()}, version {input.Header.version}");
            
            foreach( lump_t lump in input.Header.lumps)
            {
                Console.WriteLine($"Writing Lump Starting at: {lump.fileofs}, length of {lump.filelen}");

                BinaryWriter.Write(lump.fileofs);
                BinaryWriter.Write(lump.filelen);
                BinaryWriter.Write(lump.version);
            }

            BinaryWriter.Write(inputText);

            // Add to file
            BinaryWriter.Flush();
        }
    }
}
