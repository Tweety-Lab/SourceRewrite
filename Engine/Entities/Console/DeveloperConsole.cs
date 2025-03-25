using SourceRewrite.Attributes;

namespace SourceRewrite
{
    public static class DeveloperConsole
    {
        public static DeveloperConsoleCanvas ConsoleCanvas { get; set; }

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

            // Find method with the matching command
            var method = AttributeManager.GetMethodByAttributeValue<ConCommandAttribute, string>("Command", commandName);

            // Find convar with the matching command
            var convar = AttributeManager.GetPropertyByAttributeValue<ConVarAttribute, string>("Name", commandName);

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
                            // Convert the argument to the appropriate type
                            convertedArgs[i] = ConvertConValueToType(args[i], parameters[i].ParameterType);
                        }
                        catch (Exception ex)
                        {
                            Error($"Failed to convert argument {i + 1} ({args[i]}) to {parameters[i].ParameterType.Name}: {ex.Message}");
                            return;
                        }
                    }

                    method.Invoke(null, convertedArgs);
                    return;
                }
                catch (Exception ex)
                {
                    // Log the inner exception if available
                    var innerExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : "No inner exception";
                    Error($"Error executing command '{commandName}': {ex.Message}. Inner exception: {innerExceptionMessage}");
                    return;
                }
            }
            else if(convar != null)
            {
                try
                {
                    // Change Convar to the given argument

                    var propertyType = convar.PropertyType;
                    var convertedArg = ConvertConValueToType(args[0], propertyType);
                    convar.SetValue(null, convertedArg);
                }
                catch (Exception ex)
                {
                    Error($"Failed to set {commandName} to {args[0]}: {ex.Message}");
                }
            }
            else
            {
                Msg($"Unknown command \"{commandName}\"");
            }
        }


        /// <summary>
        /// Convert a Console variable value to the specified type.
        /// </summary>
        /// <param name="input">The input string to convert.</param>
        /// <param name="type">The target type to convert to.</param>
        /// <returns>The converted value.</returns>
        private static object ConvertConValueToType(string input, Type type)
        {
            if (type == typeof(bool))
            {
                if (input == "1")
                    return true;
                else if (input == "0")
                    return false;
                else
                    throw new ArgumentException($"Argument should be '0' or '1', got '{input}'");
            }
            else
            {
                // Handle conversion for other types
                try
                {
                    return Convert.ChangeType(input, type);
                }
                catch (Exception ex)
                {
                    throw new ArgumentException($"Failed to convert '{input}' to {type.Name}: {ex.Message}");
                }
            }
        }
    }
}
