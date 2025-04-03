using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace VistaGUI.Preprocessor
{
    /// <summary>
    /// Takes VistaGUI files and pre-processes them for use in the engine.
    /// </summary>
    public static class PreProccesor
    {
        /// <summary>
        /// Actions that are performed on HTML files.
        /// </summary>
        public static List<Func<string, string>> HTMLRules = new List<Func<string, string>>()
        {
            // Rule: Auto include scripts for custom elements
            html =>
            {
                bool modified = false;
                StringBuilder scriptIncludes = new StringBuilder();

                // Check for each custom element in the ElementMap
                foreach (var element in ProcessorElements.ElementMap)
                {
                    string elementName = element.Key;
                    string scriptPath = element.Value;

                    // Check if the element exists in the HTML
                    if (Regex.IsMatch(html, $@"<{elementName}[^>]*>", RegexOptions.IgnoreCase))
                    {
                        Console.WriteLine($"Adding script for: {elementName}");
                        scriptIncludes.AppendLine($"<script src=\"file:///{scriptPath}\"></script>");
                        modified = true;
                    }
                }

                // Inject scripts before </body>
                if (modified)
                {
                    if (html.Contains("</body>"))
                    {
                        html = Regex.Replace(html, @"</body>", scriptIncludes.ToString() + "</body>", RegexOptions.IgnoreCase);
                    }
                    else
                    {
                        html += scriptIncludes.ToString(); // Append at end if no </body> found
                    }
                }

                return html;
            }
        };

        public static string ProcessHTML(string html)
        {
            string processedHTML = html;

            // Apply HTML Processing Rules
            foreach (var rule in HTMLRules)
            {
                processedHTML = rule(processedHTML);
            }

            return processedHTML;
        }
    }
}
