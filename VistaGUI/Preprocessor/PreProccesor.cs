using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
