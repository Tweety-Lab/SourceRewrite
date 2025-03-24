using SourceRewrite.Attributes;
using SourceRewrite.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SourceRewrite
{
    public static class EngineConsole
    {
        public static EngineConsoleCanvas ConsoleCanvas { get; set; }

        static EngineConsole()
        {
            // Initialize the cache for ConCommandAttribute
            AttributeManager.Initialize<ConCommandAttribute>();
        }

        public static void Msg(string message)
        {
            ConsoleCanvas.Msg(message);
        }

        public static void Warning(string message)
        {
            ConsoleCanvas.Warning(message);
        }

        public static void Error(string message)
        {
            ConsoleCanvas.Error(message);
        }

        public static string EvaluateCommand(string command)
        {
            // Find the method with the matching command directly
            var method = AttributeManager.GetMethodByAttributeValue<ConCommandAttribute, string>("Command", command);

            if (method != null)
            {
                // Execute the method
                method.Invoke(null, null);
                return string.Empty;
            } else
            {
                return $"Unknown command \"{command}\"";
            }
        }
    }
}
