using FileFormats.KeyValues;
using SourceRewrite.Files;

namespace SourceRewrite.AssetTypes
{
    /// <summary>
    /// Material Class.
    /// </summary>
    public class Material
    {
        public Shader Shader;
        public Texture[] Textures = new Texture[4]; // Allows up to 4 texture maps in a Material (Change this eventually)

        // File-Path Constructor
        public Material(string filePath = "")
        {
            // Dont process non-file materials
            if (filePath == string.Empty)
                return;

            try
            {
                // If Material cant be found, set it to missing
                if (!File.Exists(filePath))
                {
                    Console.WriteLine($"Could not find material at '{filePath}'");
                    filePath = FileSystem.GetMaterialPath("dev/missing");
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
                        Textures[0] = new Texture(FileSystem.GetTexturePath((string)keyValue.Value)); // Set the Texture
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
