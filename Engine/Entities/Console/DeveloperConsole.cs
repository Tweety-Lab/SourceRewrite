using SourceRewrite.Attributes;
using SourceRewrite.Entities.Console;
using SourceRewrite.Files;
using SourceRewrite.InputSystem;
using SourceRewrite.Windowing;
using System.Text.RegularExpressions;

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
            var regex = new Regex(@"(\""(.*?)\"")|(\S+)");
            var parts = regex.Matches(command)
                             .Cast<Match>()
                             .Select(m => m.Value.Trim('"'))
                             .ToArray();

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
            else if(convar != null && args.Length == 1)
            {
                try
                {
                    // Change Convar to the given argument
                    var propertyType = convar.PropertyType;

                    // Dont run if it's a cheat and cheats are disabled
                    ConVarAttribute attribute = AttributeManager.GetAttribute<ConVarAttribute>(convar);
                    if (attribute.Flag == ConVarFlag.Cheat && !ConVars.CheatsEnabled)
                        return;

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
        /// Load a config file and execute each command in it.
        /// </summary>
        /// <param name="path"></param>
        public static void LoadConfig(string path)
        {
            // Check if file exists
            if (!File.Exists(path))
                return;

            // Read the config file
            string[] lines = File.ReadAllLines(path);

            // Evaluate each line as a command (ignoring comments)
            foreach (string line in lines)
            {
                if (line.StartsWith("//"))
                    continue;

                EvaluateCommand(line);
            }
        }

        public static void WriteConfig(string path)
        {
            // Get all Binds
            var binds = Input.GetBoundActions();

            // Get all ConVars
            var convars = AttributeManager.GetPropertiesWithAttribute<ConVarAttribute>();

            // Write to file
            using (StreamWriter writer = new StreamWriter(path))
            {
                // Write all Binds
                foreach (var bind in binds)
                {
                    writer.WriteLine($"bind {bind}");
                }

                // Write all ConVars
                foreach (var convar in convars)
                {
                    // Retrieve the ConVar attribute
                    ConVarAttribute conVarAttribute = (ConVarAttribute)convar.Value;

                    // Skip ConVars that don't have the Save flag
                    if (conVarAttribute.Flag != ConVarFlag.Archive)
                        continue;

                    // Get the current value of the ConVar
                    var currentValue = convar.Key.GetValue(null);

                    // Write the ConVar name and value to the file
                    writer.WriteLine($"{conVarAttribute.Name} \"{currentValue}\"");
                }
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
