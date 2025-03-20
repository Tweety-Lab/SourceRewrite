using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SourceRewrite
{
    /// <summary>
    /// Information about the underlying Application
    /// </summary>
    public static class Application
    {
        /// <summary>
        /// The arguments passed to the application on launch. Key is argument name, value is argument value.
        /// </summary>
        public static Dictionary<string, string> Arguments { get; set; } = new Dictionary<string, string>();

        /// <summary>
        /// Abstraction for .exe argument handling
        /// </summary>
        public static class ArgumentManager
        {
            public static void SetArguments(string[] args)
            {
                Arguments = ConvertArgsToDictionary(args);
            }

            // Helper function to convert app arguments to dictionary
            public static Dictionary<string, string> ConvertArgsToDictionary(string[] args)
            {
                Dictionary<string, string> argsDictionary = new Dictionary<string, string>();

                for (int i = 0; i < args.Length; i++)
                {
                    string key = args[i];

                    // Check if the argument has a value (i.e., the next item in the array)
                    if (i + 1 < args.Length && !args[i + 1].StartsWith("-"))
                    {
                        // If yes, assign the next argument as the value
                        argsDictionary[key] = args[i + 1];
                        i++; // Skip the next argument as it is already used as the value
                    }
                    else
                    {
                        // If no value is provided, set the value as an empty string
                        argsDictionary[key] = string.Empty;
                    }
                }

                return argsDictionary;
            }
        }
    }
}
