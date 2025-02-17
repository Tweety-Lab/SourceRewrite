using Sledge.Formats.Texture.Vtf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileFormats.VTF
{
    /// <summary>
    /// Valve's Source 1 .vtf texture format.
    /// </summary>
    public class TextureFormat
    {
        private VtfFile vtfFile;
        private VtfImage vtfImage;

        // Texture Height and Width
        public int Height;
        public int Width;


        public TextureFormat(string path)
        {
            // Load the VTF file using Sledge.Formats.Texture
            using (var stream = File.OpenRead(path))
            {
                vtfFile = new VtfFile(stream);

                vtfImage = vtfFile.Images[vtfFile.Images.Count - 1]; // Highest quality mipmap

                Height = vtfImage.Height;
                Width = vtfImage.Width;
            }
        }

        /// <summary>
        /// Returns the Texture's data in BGRA32.
        /// </summary>
        public byte[] GetBgra32Data()
        {
            return vtfImage.GetBgra32Data();
        }
    }
}
