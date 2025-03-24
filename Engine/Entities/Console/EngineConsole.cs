using FileFormats.KeyValues;
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

        public static void EvaluateCommand(string command)
        {
            // Split the command into parts (first part is command name, rest are arguments)
            var parts = command.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 0)
                return;

            string commandName = parts[0];
            string[] args = parts.Length > 1 ? parts.Skip(1).ToArray() : Array.Empty<string>();

            // Find the method with the matching command
            var method = AttributeManager.GetMethodByAttributeValue<ConCommandAttribute, string>("Command", commandName);

            if (method != null)
            {
                try
                {
                    // Get parameters of the method
                    var parameters = method.GetParameters();
                    object[] convertedArgs = new object[parameters.Length];

                    // If method takes no parameters, invoke without args
                    if (parameters.Length == 0)
                    {
                        method.Invoke(null, null);
                        return;
                    }

                    // If method takes a string array, pass the args directly
                    if (parameters.Length == 1 && parameters[0].ParameterType == typeof(string[]))
                    {
                        method.Invoke(null, new object[] { args });
                        return;
                    }

                    if (parameters.Length != args.Length)
                    {
                        Error($"Command requires {parameters.Length} arguments, got {args.Length}");
                        return;
                    }

                    for (int i = 0; i < parameters.Length; i++)
                    {
                        try
                        {
                            // Handle boolean conversion for 0-1 values
                            if (parameters[i].ParameterType == typeof(bool))
                            {
                                bool boolValue = false;
                                if (args[i] == "1")
                                    boolValue = true;
                                else if (args[i] != "0")
                                {
                                    // Handle invalid boolean input
                                    Error($"Argument {i + 1} should be '0' or '1', got '{args[i]}'");
                                    return;
                                }
                                convertedArgs[i] = boolValue;
                            }
                            else
                            {
                                // Convert other arguments to the requested type
                                convertedArgs[i] = Convert.ChangeType(args[i], parameters[i].ParameterType);
                            }
                        }
                        catch (Exception ex)
                        {
                            Error($"Failed to convert argument {i + 1} ({args[i]}) to {parameters[i].ParameterType.Name}: {ex.Message}");
                        }
                    }

                    method.Invoke(null, convertedArgs);
                    return;
                }
                catch (Exception ex)
                {
                    Error($"Error executing command '{commandName}': {ex.Message}");
                    return;
                }
            }
            else
            {
                Msg($"Unknown command \"{commandName}\"");
            }
        }
    }
}
