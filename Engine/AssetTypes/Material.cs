using SourceRewrite.Rendering;
using SourceRewrite.Windowing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileFormats.KeyValues;
using SourceRewrite.Files;
using System.IO;

namespace SourceRewrite.AssetTypes
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
                // If Material cant be found, set it to missing
                if (!File.Exists(filePath))
                {
                    Console.WriteLine($"Could not find material at '{filePath}'");
                    filePath = FileSystem.GetMaterialPath("dev/missing.vmt");
                }

                string content = File.ReadAllText(filePath);
                
                KeyValuesFormat keyValues = new KeyValuesFormat(content); // Parse the Material file

                Shader = FileSystem.GetShader(keyValues.ParentKeys[0].Name); // Set the Shader

                // Loop through every KeyValue in Material
                foreach (KeyValue keyValue in keyValues.ParentKeys[0].ChildKeyValues)
                {
                    // Special logic for base texture paths (REPLACE THIS)
                    if (keyValue.Key == "$basetexture")
                    {
                        Texture = new Texture(FileSystem.GetMaterialPath((string)keyValue.Value)); // Set the Texture
                    }
                    // Keys starting with '$' are Shader properties
                    else if (keyValue.Key.StartsWith('$'))
                    {
                        string propertyName = keyValue.Key.Split('$')[1];
                        object propertyValue = keyValue.Value;

                        // Set Shader uniform (property) to input property
                        Shader.SetParameter(propertyName, propertyValue);
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"A Material error occurred: {ex.Message}");
            }
        }
    }
}
