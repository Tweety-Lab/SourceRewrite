using FileFormats.KeyValues;

namespace SourceRewrite.Entities
{
    public class EntityIOConnection
    {
        public string OutputName { get; set; }
        public string TargetName { get; set; }
        public string InputName { get; set; }
        public object Parameter { get; set; }

        public void Fire()
        {
            // TODO: Use attribute cache system

            // Search Map for Target Entity
            BaseEntity entity = EntityManager.MapContainer.FindInChildren(TargetName);

            // if entity is found, use reflection to find it
            if (entity != null)
            {
                // Get Input Method if it has InputAttribute
                var method = entity.GetType().GetMethod(InputName);
                if (method != null)
                {
                    // Check the number of parameters the method expects
                    var methodParameters = method.GetParameters();

                    // If the method expects no parameters, invoke it without passing any parameter
                    if (methodParameters.Length == 0)
                    {
                        method.Invoke(entity, null);
                    }
                    else if (methodParameters.Length == 1 && Parameter != null)
                    {
                        // Invoke with the parameter if it expects 1 parameter
                        var convertedParam = Convert.ChangeType(Parameter, methodParameters[0].ParameterType);
                        method.Invoke(entity, new object[] { convertedParam });
                    }
                }
            }
        }
    }

    public static class EntityIOUtility
    {
        // We store strings in BSPs like so: "connection_OutputName "TargetNameInputNameParameterdiscard-discard""
        public static EntityIOConnection ParseIOString(string ioString)
        {
            // Initialize the EntityIOConnection object
            EntityIOConnection io = new EntityIOConnection();

            try
            {
                // Remove the connection_ prefix
                ioString = ioString.Replace("connection_", "").Trim();

                // Find the first space which separates OutputName from the rest
                int firstSpace = ioString.IndexOf(' ');
                if (firstSpace <= 0)
                {
                    throw new FormatException("Invalid IO string format - missing space separator");
                }

                // Extract OutputName (before the space)
                io.OutputName = ioString.Substring(0, firstSpace).Trim();

                // The rest of the string is in quotes, extract the content between quotes
                int firstQuote = ioString.IndexOf('"', firstSpace);
                int lastQuote = ioString.LastIndexOf('"');
                if (firstQuote < 0 || lastQuote < 0 || firstQuote == lastQuote)
                {
                    throw new FormatException("Invalid IO string format - missing quotes around parameters");
                }

                string quotedContent = ioString.Substring(firstQuote + 1, lastQuote - firstQuote - 1);

                // Split the quoted content by the delimiter ()
                string[] parts = quotedContent.Split(new[] { "" }, StringSplitOptions.RemoveEmptyEntries);

                // Ensure there are enough parts
                if (parts.Length >= 2)
                {
                    io.TargetName = parts[0];
                    io.InputName = parts[1];
                    io.Parameter = parts.Length > 2 ? parts[2] : null;

                    // Only try to convert if the parameter is not null
                    if (io.Parameter != null)
                    {
                        io.Parameter = KeyValuesUtility.ConvertValueToType(io.Parameter.ToString());
                    }
                }
                else
                {
                    throw new FormatException("Not enough parts after filtering discard values");
                }
            }
            catch (Exception ex)
            {
                System.Console.WriteLine($"Error parsing IO string: {ex.Message}");
                io.OutputName = ioString; // Fallback to original string
            }

            return io;
        }
    }
}
