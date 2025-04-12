using FileFormats.MDL.PHY;
using FileFormats.MDL.VTX;
using FileFormats.MDL.VVD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace FileFormats.MDL
{
    /// <summary>
    /// Valve's Source 1 .mdl model format.
    /// </summary>
    public class MDLFormat
    {
        /// <summary>
        /// Path of requested .VMT Textures
        /// </summary>
        public List<string> TexturePaths = new List<string>();

        /// <summary>
        /// Model Header.
        /// </summary>
        public MDLHeader Header;

        // Other Files
        public VTXFormat VTX;
        public VVDFormat VVD;
        public PHYFormat PHY;

        public MDLFormat(string path)
        {
            // Load the Model
            Header = new MDLHeader();

            // Start reading the file
            using (var reader = new BinaryReader(File.Open(path, FileMode.Open)))
            {
                // Read the header
                Header.ID = reader.ReadInt32();
                Header.Version = reader.ReadInt32();
                Header.Checksum = reader.ReadInt32();

                // Read the name
                Header.Name = new string(reader.ReadChars(64)).Trim('\0');
                Header.DataLength = reader.ReadInt32();

                // Read Vectors, three 4-byte floats in a row
                Header.EyePosition = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                Header.IllumPosition = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                Header.HullMin = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                Header.HullMax = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                Header.ViewBBMin = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                Header.ViewBBMax = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());

                // Read Model Flags
                Header.Flags = (MDLFlags)reader.ReadUInt32();

                // Read Offsets
                Header.BoneCount = reader.ReadInt32();
                Header.BoneOffset = reader.ReadInt32();

                Header.BoneControllerCount = reader.ReadInt32();
                Header.BoneControllerOffset = reader.ReadInt32();

                Header.HitboxCount = reader.ReadInt32();
                Header.HitboxOffset = reader.ReadInt32();

                Header.LocalAnimCount = reader.ReadInt32();
                Header.LocalAnimOffset = reader.ReadInt32();

                Header.LocalSequenceCount = reader.ReadInt32();
                Header.LocalSequenceOffset = reader.ReadInt32();

                Header.ActivityListVersion = reader.ReadInt32();
                Header.EventsIndexed = reader.ReadInt32();

                Header.TextureCount = reader.ReadInt32();
                Header.TextureOffset = reader.ReadInt32();

                // Set Texture Names
                TexturePaths = GetTextureNames(reader);

                // This Offset Points to a series of ints
                // Each int value, in turn, is an offset relative to the start of the file
                // at which there is a null-terminated string
                Header.TextureDirCount = reader.ReadInt32();
                Header.TextureDirOffset = reader.ReadInt32();

                Header.SkinReferenceCount = reader.ReadInt32();
                Header.SkinFamilyCount = reader.ReadInt32();
                Header.SkinReferenceIndex = reader.ReadInt32();

                Header.BodyPartCount = reader.ReadInt32();
                Header.BodyPartOffset = reader.ReadInt32();

                Header.AttachmentCount = reader.ReadInt32();
                Header.AttachmentOffset = reader.ReadInt32();
            }

            // Load other Files
            string vvdPath = path.Replace(".mdl", ".vvd");
            VVD = new VVDFormat(vvdPath);

            string vtxPath = path.Replace(".mdl", ".vtx");
            VTX = new VTXFormat(vtxPath);

            string phyPath = path.Replace(".mdl", ".phy");

            // Only read .PHY if the file exists
            if (File.Exists(phyPath))
                PHY = new PHYFormat(phyPath);
        }

        public uint[] GetMeshIndices()
        {
            if (VTX == null)
                return new uint[0];

            // Get the strip group and strip data
            var stripGroup = VTX.StripGroup;
            var strip = VTX.Strip;

            // Create an array to store the final indices
            uint[] meshIndices = new uint[strip.NumIndices];

            // Read the vertex data from the strip group
            // (Assuming vertex data is stored in a way similar to the Three.js loader)
            for (int i = 0; i < strip.NumIndices; i++)
            {
                // Step 1: Get the raw index from the strip
                int rawIndex = strip.IndexOffset + i;

                // Step 2: Read the index from the strip group's index data
                // (Assuming stripGroup.IndexData is a byte array containing UInt16 indices)
                int index2 = BitConverter.ToUInt16(stripGroup.IndexData, rawIndex * 2);

                // Step 3: Read the vertex index from the strip group's vertex data
                // (Assuming vertex data is structured as [position, normal, texcoord, bone weights, etc.])
                // The Three.js loader uses an offset of +4 bytes to read the vertex index
                int index3 = BitConverter.ToUInt16(stripGroup.VertexData, index2 * 9 + 4);

                // Step 4: Apply mesh vertex offset (if available)
                int index4 = VTX.Strip.VertOffset + index3;

                // Step 5: Apply model vertex offset (if available)
                // (Assuming Header.VertexIndex is in bytes, divide by 48 to get vertex count)
                int index5 = index4 + (int)(VTX.Strip.VertOffset / 48);

                meshIndices[i] = (uint)index5;
            }

            // Reverse the indices to fix winding order (like Three.js does)
            MDLHelper.ReverseInPlace(meshIndices);

            return meshIndices;
        }

        private List<string> GetTextureNames(BinaryReader reader)
        {
            List<string> textureNames = new List<string>();

            // Go to offset
            int currentOffset = (int)reader.BaseStream.Position;
            reader.BaseStream.Seek(Header.TextureOffset, SeekOrigin.Begin);

            // Read the texture offset
            int textureOffset = reader.ReadInt32();

            // Goto the texture offset accounting for the extra 4 bytes
            reader.BaseStream.Seek(textureOffset - 4, SeekOrigin.Current);

            // Read texture Name
            string textureName = MDLHelper.ReadNullTerminatedString(reader, ASCIIEncoding.ASCII);
            textureNames.Add(textureName);

            // Return to original position
            reader.BaseStream.Seek(currentOffset, SeekOrigin.Begin);

            return textureNames;
        }

    }

    public static class MDLHelper
    {
        public static string ReadNullTerminatedString(BinaryReader reader, Encoding encoding)
        {
            List<byte> bytes = new List<byte>();
            bool started = false;
            byte b;

            while (true)
            {
                b = reader.ReadByte();

                if (b == 0)
                {
                    if (!started)
                    {
                        // First byte is null, ignore and continue
                        continue;
                    }
                    else
                    {
                        // Reached end of string
                        break;
                    }
                }

                started = true;
                bytes.Add(b);
            }

            return encoding.GetString(bytes.ToArray());
        }

        // Reverse an array in-place
        public static void ReverseInPlace<T>(T[] array)
        {
            int i = 0;
            int j = array.Length - 1;
            while (i < j)
            {
                T temp = array[i];
                array[i] = array[j];
                array[j] = temp;
                i++;
                j--;
            }
        }

    }
}
