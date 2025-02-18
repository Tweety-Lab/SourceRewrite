using Sledge.Formats.Texture.Wad;
using Sledge.Formats.Texture.Wad.Lumps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FileFormats.BSP
{
    public class BSPReader : IDisposable
    {
        private readonly BinaryReader _binaryReader;
        public BSPFormat BSP;

        public BSPReader(string inputPath)
        {
            _binaryReader = new BinaryReader(File.Open(inputPath, FileMode.Open));
        }

        /// <summary>
        /// Reads a Lump from BSP.
        /// </summary>
        public void ProcessLumpData(Lump input, int lumpIndex)
        {
            // Return early if no data to read
            if (input.FileLength == 0)
                return;

            // Check if we're trying to read beyond stream length
            if (_binaryReader.BaseStream.Position + input.FileLength > _binaryReader.BaseStream.Length)
            {
                throw new InvalidOperationException("Attempting to read beyond the end of the stream.");
            }

            // Determine data type based on lump index
            switch ((Lump.LumpType)lumpIndex)
            {
                case Lump.LumpType.LUMP_VERTEXES:
                    float[] vertexData = new float[input.FileLength / sizeof(float)];
                    for (int i = 0; i < vertexData.Length; i++)
                    {
                        vertexData[i] = _binaryReader.ReadSingle();
                    }
                    BSP.Header.lumps[lumpIndex].Data = vertexData; // Update the Lump with our data
                    break;

                case Lump.LumpType.LUMP_INDICES:
                    uint[] indexData = new uint[input.FileLength / sizeof(uint)];
                    for (int i = 0; i < indexData.Length; i++)
                    {
                        indexData[i] = _binaryReader.ReadUInt32();
                    }
                    BSP.Header.lumps[lumpIndex].Data = indexData;
                    break;

                case Lump.LumpType.LUMP_MATERIAL:
                    byte[] stringBytes = _binaryReader.ReadBytes(input.FileLength);
                    string materialPath = Encoding.UTF8.GetString(stringBytes).TrimEnd('\0');
                    BSP.Header.lumps[lumpIndex].Data = materialPath;
                    break;

                default:
                    // For unknown lump types, just read raw bytes
                    input.Data = _binaryReader.ReadBytes(input.FileLength);
                    break;
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
            BSP = new BSPFormat();

            // Read header
            BSP.Header.ident = _binaryReader.ReadInt32();
            BSP.Header.version = _binaryReader.ReadInt32();
            BSP.Header.mapRevision = _binaryReader.ReadInt32();

            // Read all 64 lump entries
            for (int i = 0; i < 64; i++)
            {
                var lump = new Lump
                {
                    FileOffset = _binaryReader.ReadInt32(),
                    FileLength = _binaryReader.ReadInt32(),
                    Version = _binaryReader.ReadInt32(),
                    FourCC = _binaryReader.ReadChars(4),  // Read 4 bytes for the FourCC
                };
                BSP.Header.lumps[i] = lump;
            }

            Console.WriteLine($"Lump Offset: {BSP.Header.lumps[0].FileOffset}\nLump Length: {BSP.Header.lumps[0].FileLength}");
            Console.WriteLine($"Reading BSP v{BSP.Header.version}");

            // Read lump data at correct offsets
            foreach (var lumpPair in BSP.Header.lumps.Select((lump, index) => new { lump, index }))
            {
                if (lumpPair.lump.FileLength > 0)
                {
                    _binaryReader.BaseStream.Seek(lumpPair.lump.FileOffset, SeekOrigin.Begin);
                    ProcessLumpData(lumpPair.lump, lumpPair.index);
                }
            }

            return BSP;
        }

        /// <summary>
        /// Gets a lump from the BSP file by its type.
        /// </summary>
        public Lump GetLump(Lump.LumpType type)
        {
            if (BSP == null)
                ReadFromMap();

            int index = (int)type;
            if (index < 0 || index >= BSP.Header.lumps.Length)
                return new Lump();

            return BSP.Header.lumps[index];
        }

        /// <summary>
        /// Gets the data from a lump cast to the specified type.
        /// </summary>
        public T GetLumpData<T>(Lump.LumpType type) where T : class
        {
            var lump = GetLump(type);
            if (lump.Data == null)
                return null;

            return lump.Data as T;
        }

        public void Dispose()
        {
            _binaryReader?.Dispose();
        }
    }
}
