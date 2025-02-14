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
        private readonly int version;

        public BSPWriter(string filePath, int version = 21)
        {
            BinaryWriter = new BinaryWriter(File.Open(filePath, FileMode.Create));
            this.version = version;
        }
        
        /// <summary>
        /// Writes an input to BSP.
        /// </summary>
        public void Write(object input)
        {
            // Write version first (int)
            BinaryWriter.Write(version);

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
    }
}
