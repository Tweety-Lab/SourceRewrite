using FileFormats.VMF;
using MessagePack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileFormats.BSP.IO
{
    public class BSPReader
    {
        public BSPFormat BSP { get; }

        public BSPReader(string path)
        {
            BSP = new BSPFormat();

            using (var stream = File.OpenRead(path))
            {
                // Deserialize the header
                BSP.Header = MessagePackSerializer.Deserialize<BSPHeader>(stream);

                // Initialize the lumps dictionary if it's null
                BSP.Header.Lumps ??= new Dictionary<BSPLumpType, object>();
            }
        }

        // Deserialize lumps as their specific struct types
        public T[] GetLumpData<T>(BSPLumpType type) where T : struct
        {
            if (BSP.Header.Lumps.TryGetValue(type, out var data))
            {
                // Ensure data is a byte array before deserializing
                if (data is byte[] byteArray)
                {
                    // Deserialize the byte array back into the correct struct array
                    return MessagePackSerializer.Deserialize<T[]>(byteArray);
                }
            }
            return Array.Empty<T>();
        }
    }
}
