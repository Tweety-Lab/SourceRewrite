using SourceRewrite.Rendering;
using SourceRewrite.Windowing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SourceRewrite.FileSystem.FileTypes
{
    public class Material
    {
        public Shader Shader;
        public Texture Texture;

        public Material(string filePath)
        {
            try
            {
                string content = File.ReadAllText(filePath);

                KeyValuesFormat keyValues = new KeyValuesFormat(content);

                Shader = FileSystem.GetShader(keyValues.ParentKeys[0].Name); // Get the Shader

                Texture = new Texture(FileSystem.GetMaterialPath((string) keyValues.GetKeyValue("basetexture").Value)); // Get the Texture

                Console.WriteLine(keyValues.ParentKeys[0].Name);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"A Material error occurred: {ex.Message}");
            }
        }
    }
}
