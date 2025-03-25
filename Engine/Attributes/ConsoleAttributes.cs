namespace SourceRewrite.Attributes
{

    /// <summary>
    /// Method that can be called from the Developer Console.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class ConCommandAttribute : Attribute
    {
        /// <summary>
        /// Command to call the method.
        /// </summary>
        public string Command { get; set; }

        public ConCommandAttribute(string command)
        {
            // Assign the command
            Command = command;
        }
    }

    /// <summary>
    /// Flags for ConVars.
    /// </summary>
    public enum ConVarFlag
    {
        None = 0,
        Save = 1
    }

    /// <summary>
    /// Variable that can be altered from the Developer Console.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class ConVarAttribute : Attribute
    {
        /// <summary>
        /// Name of the variable.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Flags for the variable.
        /// </summary>
        public ConVarFlag Flag { get; set; }

        public ConVarAttribute(string name, ConVarFlag flag = ConVarFlag.None)
        {
            Name = name;
            Flag = flag;
        }
    }
}
