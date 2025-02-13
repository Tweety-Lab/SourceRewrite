using SourceRewrite.Rendering;
using SourceRewrite.Windowing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SourceRewrite.FileSystem.FileTypes;

namespace SourceRewrite.Assets
{
    /// <summary>
    /// Material Class.
    /// </summary>
    public class Material
    {
        public Shader Shader;
        public Texture Texture;

        public Material(string filePath)
        {
            try
            {
                string content = File.ReadAllText(filePath);
                
                KeyValuesFormat keyValues = new KeyValuesFormat(content); // Parse the Material file

                Shader = FileSystem.FileSystem.GetShader(keyValues.ParentKeys[0].Name); // Set the Shader
                Texture = new Texture(FileSystem.FileSystem.GetMaterialPath((string) keyValues.GetKeyValue("$basetexture").Value)); // Set the Texture
            }
            catch (Exception ex)
            {
                Console.WriteLine($"A Material error occurred: {ex.Message}");
            }
        }
    }
}
