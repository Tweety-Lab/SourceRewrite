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
        public Dictionary<string, object> Properties = new();

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
                    DeveloperConsole.Warning($"Could not find material at '{filePath}'");
                    filePath = FileSystem.GetMaterialPath("dev/missing");
                }

                string content = File.ReadAllText(filePath);
                
                KeyValuesFormat keyValues = new KeyValuesFormat(content); // Parse the Material file

                Shader = FileSystem.GetShader(keyValues.ParentKeys[0].Name); // Set the Shader

                // Loop through every KeyValue in Material
                foreach (KeyValue keyValue in keyValues.ParentKeys[0].ChildKeyValues)
                {
                    // Get property name and value
                    string key = keyValue.Key;
                    string propertyName;

                    // Split at '$' or '%'
                    if (key.StartsWith('$'))
                        propertyName = key.Split('$')[1];
                    else if (key.StartsWith('%'))
                        propertyName = key.Split('%')[1];
                    else
                        propertyName = key; // fallback if neither is present

                    object propertyValue = keyValue.Value;

                    // Add to Properties
                    Properties.Add(propertyName, propertyValue);

                    // Special logic for Textures
                    // Textures are stored as paths to the texture file in the Material
                    if (Shader.GetParameter<Texture>(propertyName) != default)
                    {
                        Console.WriteLine("Texture");
                        Shader.SetParameter(propertyName, new Texture(FileSystem.GetTexturePath((string)propertyValue)));
                    }
                    // Keys starting with '$' are Shader properties
                    else if (keyValue.Key.StartsWith('$'))
                    {
                        // Set Shader uniform (property) to input property
                        Shader.SetParameter(propertyName, propertyValue);
                    }
                }

            }
            catch (Exception ex)
            {
                DeveloperConsole.Error($"A Material error occurred: {ex.Message}");
            }
        }

        // Get Property
        public object GetProperty(string propertyName) => Properties[propertyName];

        // Check if Property exists
        public bool HasProperty(string propertyName) => Properties.ContainsKey(propertyName);

        // Get a Flag (boolean property)
        public int GetFlag(string propertyName)
        {
            if (Properties.ContainsKey(propertyName))
                return (int)Properties[propertyName];
            else
                return 0;
        }
    }
}
