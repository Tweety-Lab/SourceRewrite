using System;
using System.Text;
using System.Text.RegularExpressions;

namespace FileFormats.Shaders
{
    public class ShaderFormat
    {
        /// <summary>
        /// All functions in the shader file, for example vertex() or fragment().
        /// </summary>
        public List<ShaderFormatFunction> Functions { get; private set; } = new List<ShaderFormatFunction>();

        /// <summary>
        /// All Global Uniforms in the shader file, global uniforms are accessible by all shader functions.
        /// </summary>
        public List<GlobalUniform> GlobalUniforms = new List<GlobalUniform>();

        // OpenGL Shader Version
        private string shaderVersion = "#version 330 core";

        // Regular expression to identify shader function declarations
        private static readonly Regex FunctionDeclarationRegex = new Regex(@"^\s*void\s+(\w+)\s*\(\s*\)", RegexOptions.Compiled);

        // Regular expression to identify uniforms
        private static readonly Regex UniformDeclerationRegex = new Regex(@"uniform\s+(\w+)\s+(\w+)\s*;", RegexOptions.Compiled);

        public ShaderFormat(string shaderSource)
        {
            string[] lines = shaderSource.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            int currentLine = 0;

            StringBuilder currentFunctionContent = new StringBuilder();
            string currentFunctionName = null;
            int braceCount = 0;
            bool insideFunctionBlock = false;
            bool firstBraceFound = false; // Track the first brace

            while (currentLine < lines.Length)
            {
                var line = lines[currentLine].Trim();

                if (string.IsNullOrWhiteSpace(line) || line.StartsWith("//")) // Ignore comments and empty lines
                {
                    currentLine++;
                    continue;
                }

                // Check for Global Uniforms
                Match uniformMatch = UniformDeclerationRegex.Match(line);
                if (uniformMatch.Success && insideFunctionBlock == false)
                {
                    string uniformType = uniformMatch.Groups[1].Value;
                    string uniformName = uniformMatch.Groups[2].Value;
                    GlobalUniforms.Add(new GlobalUniform { Type = uniformType, Name = uniformName });
                    currentLine++;
                    continue;
                }

                // Check for shader function declaration
                Match functionMatch = FunctionDeclarationRegex.Match(line);
                if (functionMatch.Success && braceCount == 0) // Only match top-level functions
                {
                    currentFunctionName = functionMatch.Groups[1].Value;
                    currentFunctionContent.Clear();
                    insideFunctionBlock = false;
                    braceCount = 0;
                    firstBraceFound = false;
                    currentLine++;
                    continue;
                }

                // Detect opening brace
                if (line.Contains("{"))
                {
                    braceCount++;
                    insideFunctionBlock = true;

                    // Skip appending the first brace
                    if (!firstBraceFound)
                    {
                        firstBraceFound = true;
                        currentLine++;
                        continue;
                    }
                }

                // Detect closing brace
                if (line.Contains("}"))
                {
                    braceCount--;
                    if (braceCount == 0 && insideFunctionBlock) // End of shader block
                    {
                        insideFunctionBlock = false;

                        // Store function content
                        string functionContent = currentFunctionContent.ToString().Trim();

                        // Insert shader version at the beginning of the function
                        functionContent = functionContent.Insert(0, shaderVersion + "\n");

                        // Insert global uniforms after the shader version
                        foreach (var uniform in GlobalUniforms)
                        {
                            functionContent = functionContent.Insert(shaderVersion.Length + 1, $"uniform {uniform.Type} {uniform.Name};\n");
                        }

                        ShaderFormatFunction function = new ShaderFormatFunction
                        {
                            Name = currentFunctionName,
                            Content = functionContent
                        };

                        // Store all functions in the Functions list
                        Functions.Add(function);

                        currentFunctionName = null;
                        currentLine++;
                        continue;
                    }
                }

                // Add content if inside a shader block
                if (insideFunctionBlock && braceCount > 0)
                {
                    currentFunctionContent.AppendLine(line);
                }

                currentLine++;
            }
        }

        /// <summary>
        /// Gets a function from the shader file, for example vertex() or fragment().
        /// </summary>
        public ShaderFormatFunction GetFunction(string name)
        {
            return Functions.Find(f => f.Name == name);
        }
    }

    public struct ShaderFormatFunction
    {
        public string Name { get; set; }
        public string Content { get; set; }
    }

    // Global Uniforms are Uniforms accessible by all shader functions
    public struct GlobalUniform
    {
        public string Type { get; set; }
        public string Name { get; set; }
    }
}
