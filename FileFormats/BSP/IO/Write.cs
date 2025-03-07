using Sledge.Formats.Texture.Wad;
using System.Text;

namespace FileFormats.BSP
{
    public class BSPWriter : IDisposable
    {
        private readonly BinaryWriter _binaryWriter;

        public BSPWriter(string outputPath, string inputPath)
        {
            _binaryWriter = new BinaryWriter(File.Open(outputPath, FileMode.Create));
        }

        /// <summary>
        /// Writes a Lump to BSP.
        /// </summary>
        public void WriteLumpData(Lump input)
        {
            // Early return if no data
            if (input.Data == null) return;

            long startPosition = _binaryWriter.BaseStream.Position;
            int expectedLength = input.FileLength;

            // Type check and cast the Data property before writing
            switch (input.Data)
            {
                case byte[] byteData:
                    _binaryWriter.Write(byteData);
                    break;
                case int[] intData:
                    foreach (int value in intData)
                        _binaryWriter.Write(value);
                    break;
                case float[] floatData:
                    foreach (float value in floatData)
                        _binaryWriter.Write(value);
                    break;
                case uint[] uintData:
                    foreach (uint value in uintData)
                        _binaryWriter.Write(value);
                    break;
                case string[] stringArray:
                    foreach (string str in stringArray)
                    {
                        if (str != null)
                        {
                            byte[] stringArrayBytes = Encoding.ASCII.GetBytes(str);
                            _binaryWriter.Write(stringArrayBytes);
                        }
                        _binaryWriter.Write((byte)0); // Separate strings with Null Terminator
                    }
                    break;
                case string stringData:
                    // Convert string to bytes
                    byte[] stringBytes = Encoding.ASCII.GetBytes(stringData);
                    _binaryWriter.Write(stringBytes);
                    _binaryWriter.Write((byte)0); // Add Null Terminator
                    break;
                default:
                    throw new InvalidOperationException($"Can't write Lump Data: Unsupported BSP Lump data type: {input.Data.GetType().Name}");
            }

            // Verify we wrote the expected amount of data
            long bytesWritten = _binaryWriter.BaseStream.Position - startPosition;
            if (bytesWritten != expectedLength)
            {
                Console.WriteLine($"Warning: Expected to write {expectedLength} bytes but wrote {bytesWritten} bytes for lump type {input.DataType}");
            }
        }

        /// <summary>
        /// Writes an input to BSP.
        /// </summary>
        public void Write(object input)
        {
            switch (input)
            {
                case string str:
                    _binaryWriter.Write(str);
                    break;
                case int integer:
                    _binaryWriter.Write(integer);
                    break;
                case string[] stringArray:
                    WriteArray(stringArray, _binaryWriter.Write);
                    break;
                case int[] intArray:
                    WriteArray(intArray, _binaryWriter.Write);
                    break;
                case float[] floatArray:
                    WriteArray(floatArray, _binaryWriter.Write);
                    break;
                case uint[] uintArray:
                    WriteArray(uintArray, _binaryWriter.Write);
                    break;
                default:
                    throw new InvalidOperationException("Unsupported type");
            }

            _binaryWriter.Flush();
        }

        private void WriteArray<T>(T[] array, Action<T> writeAction)
        {
            _binaryWriter.Write(array.Length);
            foreach (var item in array)
            {
                writeAction(item);
            }
        }

        /// <summary>
        /// Writes a map to file.
        /// </summary>
        public void WriteToMap(BSPFormat input)
        {
            // Write header
            _binaryWriter.Write(input.Header.ident);
            _binaryWriter.Write(input.Header.version);
            _binaryWriter.Write(input.Header.mapRevision);

            input.RecalculateLumpOffsets();

            // Write all 64 lump entries
            for (int i = 0; i < 64; i++)
            {
                var lump = i < input.Header.lumps.Length ? input.Header.lumps[i] : new Lump();
                _binaryWriter.Write(lump.FileOffset);
                _binaryWriter.Write(lump.FileLength);
                _binaryWriter.Write(lump.Version);

                // Convert FourCC (char[]) to byte[]
                byte[] fourCCBytes = Encoding.UTF8.GetBytes(new string(lump.FourCC));

                _binaryWriter.Write(fourCCBytes);
            }

            Console.WriteLine($"Writing BSP v{input.Header.version}");

            // Write lump data at correct offsets
            foreach (var lump in input.Header.lumps)
            {
                if (lump.FileLength > 0 && lump.Data != null)
                {
                    _binaryWriter.BaseStream.Seek(lump.FileOffset, SeekOrigin.Begin);
                    WriteLumpData(lump);

                    // Pad to 4-byte alignment
                    long pos = _binaryWriter.BaseStream.Position;
                    int padding = (4 - (int)(pos % 4)) % 4;
                    for (int i = 0; i < padding; i++)
                        _binaryWriter.Write((byte)0);
                }
            }

            _binaryWriter.Flush();
        }

        public void Dispose()
        {
            _binaryWriter?.Dispose();
        }
    }
}