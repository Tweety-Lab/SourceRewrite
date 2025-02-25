using System;
using System.Text;

namespace FileFormats.Shaders
{
    public class ShaderFormat
    {
        public string VertexShader { get; private set; }
        public string FragmentShader { get; private set; }

        public ShaderFormat(string contents)
        {
            var lines = contents.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            int currentLine = 0;

            StringBuilder currentShaderContent = new StringBuilder();
            string currentShaderType = null;
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

                // Check for shader section start
                if (line.StartsWith("vertex()"))
                {
                    currentShaderType = "vertex";
                    currentShaderContent.Clear();
                    insideShaderBlock = false;
                    braceCount = 0;
                    firstBraceFound = false; // Reset for new shader block
                    currentLine++;
                    continue;
                }
                else if (line.StartsWith("fragment()"))
                {
                    currentShaderType = "fragment";
                    currentShaderContent.Clear();
                    insideShaderBlock = false;
                    braceCount = 0;
                    firstBraceFound = false; // Reset for new shader block
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
                        if (currentShaderType == "vertex")
                            VertexShader = currentShaderContent.ToString().Trim();
                        else if (currentShaderType == "fragment")
                            FragmentShader = currentShaderContent.ToString().Trim();
                        currentShaderType = null;
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
    }
}
