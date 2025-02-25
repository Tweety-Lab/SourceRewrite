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

        // Regular expression to identify shader function declarations
        private static readonly Regex FunctionDeclarationRegex = new Regex(@"^\s*void\s+(\w+)\s*\(\s*\)", RegexOptions.Compiled);

        public ShaderFormat(string contents)
        {
            var lines = contents.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            int currentLine = 0;

            StringBuilder currentShaderContent = new StringBuilder();
            string currentFunctionName = null;
            int braceCount = 0;
            bool insideShaderBlock = false;
            bool firstBraceFound = false; // Track the first brace

            while (currentLine < lines.Length)
            {
                var line = lines[currentLine].Trim();

                if (string.IsNullOrWhiteSpace(line) || line.StartsWith("//")) // Ignore comments and empty lines
                {
                    currentLine++;
                    continue;
                }

                // Check for shader function declaration
                Match match = FunctionDeclarationRegex.Match(line);
                if (match.Success && braceCount == 0) // Only match top-level functions
                {
                    currentFunctionName = match.Groups[1].Value;
                    currentShaderContent.Clear();
                    insideShaderBlock = false;
                    braceCount = 0;
                    firstBraceFound = false;
                    currentLine++;
                    continue;
                }

                // Detect opening brace
                if (line.Contains("{"))
                {
                    braceCount++;
                    insideShaderBlock = true;

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
                    if (braceCount == 0 && insideShaderBlock) // End of shader block
                    {
                        insideShaderBlock = false;

                        // Store function content
                        string shaderContent = currentShaderContent.ToString().Trim();

                        // Store all functions in the Functions list
                        Functions.Add(new ShaderFormatFunction
                        {
                            Name = currentFunctionName,
                            Content = shaderContent
                        });

                        currentFunctionName = null;
                        currentLine++;
                        continue;
                    }
                }

                // Add content if inside a shader block
                if (insideShaderBlock && braceCount > 0)
                {
                    currentShaderContent.AppendLine(line);
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
}
