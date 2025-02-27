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
        /// All Uniforms that come from the engine, i.e. viewPos.
        /// </summary>
        public List<EngineUniform> EngineUniforms { get; private set; } = new List<EngineUniform>()
        {
            new EngineUniform { Type = "vec3", Name = "VIEW_POS" }
        };

        // OpenGL Shader Version
        private string shaderVersion = "#version 330 core";

        // Regular expression to identify shader function declarations
        private static readonly Regex FunctionDeclarationRegex = new Regex(@"^\s*void\s+(\w+)\s*\(\s*\)", RegexOptions.Compiled);

        // Regular expression to identify struct declarations
        private static readonly Regex StructDeclarationRegex = new Regex(@"^\s*struct\s+\w+\s*\{", RegexOptions.Compiled);

        public ShaderFormat(string shaderSource)
        {
            string[] lines = shaderSource.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            HashSet<string> globalScopeSet = new HashSet<string>();

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

                // Check for struct declaration in global scope
                Match structMatch = StructDeclarationRegex.Match(line);
                if (structMatch.Success && braceCount == 0)
                {
                    // Add the struct declaration to the global scope
                    globalScopeSet.Add(line);
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

                // Check for Global Scope code (code outside of functions)
                // Code in the Global Scope gets replicated across all functions
                if (braceCount == 0 && !line.StartsWith("void") && !FunctionDeclarationRegex.IsMatch(line))
                {
                    globalScopeSet.Add(line);
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

                        // Build the complete function content with shader version and global scope
                        StringBuilder completeFunction = new StringBuilder();
                        completeFunction.AppendLine(shaderVersion + "\n");

                        // Add global scope lines if they are not already in the function
                        foreach (string globalLine in globalScopeSet)
                        {
                            if (!FunctionContainsLine(currentFunctionContent, globalLine))
                            {
                                completeFunction.AppendLine(globalLine);
                            }
                        }

                        // Store function content for reference
                        string functionContentStr = currentFunctionContent.ToString();

                        // Add engine uniforms if referenced in the function
                        foreach (EngineUniform uniform in EngineUniforms)
                        {
                            if (functionContentStr.Contains(uniform.Name))
                            {
                                completeFunction.AppendLine($"uniform {uniform.Type} {uniform.Name};");
                            }
                        }

                        // Add the function content
                        completeFunction.Append(currentFunctionContent.ToString().Trim());

                        // Create the function
                        ShaderFormatFunction function = new ShaderFormatFunction
                        {
                            Name = currentFunctionName,
                            Content = completeFunction.ToString()
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
        /// Checks if a function already contains a specific global scope line.
        /// </summary>
        private bool FunctionContainsLine(StringBuilder functionContent, string globalLine)
        {
            foreach (var line in functionContent.ToString().Split('\n'))
            {
                if (line.Trim() == globalLine) // Exact match check
                {
                    return true;
                }
            }
            return false;
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

    public struct EngineUniform
    {
        public string Type { get; set; }
        public string Name { get; set; }
    }
}
