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

        // OpenGL Shader Version
        private string shaderVersion = "#version 330 core";

        // Regular expression to identify shader function declarations
        private static readonly Regex FunctionDeclarationRegex = new Regex(@"^\s*void\s+(\w+)\s*\(\s*\)", RegexOptions.Compiled);

        public ShaderFormat(string shaderSource)
        {
            string[] lines = shaderSource.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            List<string> globalScopeLines = new List<string>();

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

                // Check for Global Scope code
                // Code in the Global Scope gets replicated across all functions
                if (!insideFunctionBlock)
                {
                    globalScopeLines.Add(line);
                    currentLine++;
                    continue;
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

                        // Build the complete function content with shader version and global scope
                        StringBuilder completeFunction = new StringBuilder();
                        completeFunction.AppendLine(shaderVersion + "\n");

                        // Add global scope lines
                        foreach (string globalLine in globalScopeLines)
                        {
                            completeFunction.AppendLine(globalLine);
                        }

                        // Add the function content
                        completeFunction.Append(functionContent);

                        // Create the function
                        ShaderFormatFunction function = new ShaderFormatFunction
                        {
                            Name = currentFunctionName,
                            Content = completeFunction.ToString()
                        };

                        // Store all functions in the Functions list
                        Functions.Add(function);

                        Console.WriteLine(completeFunction.ToString());

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
}
