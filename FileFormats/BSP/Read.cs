using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileFormats.BSP
{
    public class BSPReader : IDisposable
    {
        private readonly BinaryReader _binaryReader;

        public BSPReader(string inputPath)
        {
            _binaryReader = new BinaryReader(File.Open(inputPath, FileMode.Open));
        }

        /// <summary>
        /// Reads a Lump from BSP.
        /// </summary>
        public void AddLumpData(Lump input)
        {
            // Ensure that we don't attempt to read beyond the stream length
            if (input.FileLength == 0)
                return;

            if (_binaryReader.BaseStream.Position + input.FileLength > _binaryReader.BaseStream.Length)
            {
                throw new InvalidOperationException("Attempting to read beyond the end of the stream.");
            }

            // Read the data according to the Lump's type and length
            switch (input.Data)
            {
                case byte[] byteData:
                    input.Data = _binaryReader.ReadBytes(input.FileLength);
                    break;

                case int[] intData:
                    intData = new int[input.FileLength / sizeof(int)];
                    for (int i = 0; i < intData.Length; i++)
                    {
                        intData[i] = _binaryReader.ReadInt32();
                    }
                    input.Data = intData;
                    break;

                case float[] floatData:
                    floatData = new float[input.FileLength / sizeof(float)];
                    for (int i = 0; i < floatData.Length; i++)
                    {
                        floatData[i] = _binaryReader.ReadSingle();
                    }
                    input.Data = floatData;
                    break;

                case string stringData:
                    byte[] stringBytes = _binaryReader.ReadBytes(input.FileLength);
                    stringData = Encoding.ASCII.GetString(stringBytes);
                    input.Data = stringData;
                    break;

                default:
                    throw new InvalidOperationException($"Unsupported lump data type: '{input.Data?.GetType().Name ?? "null"}'");
            }
        }

        /// <summary>
        /// Reads an object from BSP.
        /// </summary>
        public object Read()
        {
            byte firstByte = _binaryReader.ReadByte();
            // Check the type based on first byte (or assume it is based on data type)
            if (firstByte == 0) // assuming 0 means string
            {
                return ReadString();
            }
            else if (firstByte == 1) // assuming 1 means int
            {
                return _binaryReader.ReadInt32();
            }
            else
            {
                throw new InvalidOperationException("Unsupported type");
            }
        }

        private string ReadString()
        {
            byte[] stringBytes = _binaryReader.ReadBytes(256);  // Assuming a max string length of 256 bytes
            return Encoding.ASCII.GetString(stringBytes).TrimEnd('\0');
        }

        /// <summary>
        /// Reads a map from file.
        /// </summary>
        public BSPFormat ReadFromMap()
        {
            BSPFormat bspFormat = new BSPFormat();

            // Read header
            bspFormat.Header.ident = _binaryReader.ReadInt32();
            bspFormat.Header.version = _binaryReader.ReadInt32();
            bspFormat.Header.mapRevision = _binaryReader.ReadInt32();

            // Read all 64 lump entries
            for (int i = 0; i < 64; i++)
            {
                var lump = new Lump
                {
                    FileOffset = _binaryReader.ReadInt32(),
                    FileLength = _binaryReader.ReadInt32(),
                    Version = _binaryReader.ReadInt32(),
                    FourCC = _binaryReader.ReadChars(4),  // Read 4 bytes for the FourCC
                    Data = bspFormat.Header.lumps[i].Data // Let us know the datatype of the lump as defined in BSPFormat
                };
                bspFormat.Header.lumps[i] = lump;
            }

            Console.WriteLine($"Lump Offset: {bspFormat.Header.lumps[0].FileOffset}\nLump Length: {bspFormat.Header.lumps[0].FileLength}");
            Console.WriteLine($"Reading BSP v{bspFormat.Header.version}");

            // Read lump data at correct offsets
            foreach (var lump in bspFormat.Header.lumps)
            {
                if (lump.FileLength > 0)
                {
                    // Seek to the appropriate lump offset
                    _binaryReader.BaseStream.Seek(lump.FileOffset, SeekOrigin.Begin);

                    // Ensure there is enough data to read before calling ReadInt32
                    if (_binaryReader.BaseStream.Position + lump.FileLength <= _binaryReader.BaseStream.Length)
                    {
                        AddLumpData(lump);
                        Console.WriteLine($"Lump data read successfully for lump with offset {lump.FileOffset}");
                    }
                    else
                    {
                        Console.WriteLine($"Not enough data available for lump at offset {lump.FileOffset}, skipping.");
                    }
                }
            }

            return bspFormat;
        }

        public void Dispose()
        {
            _binaryReader?.Dispose();
        }
    }
}
