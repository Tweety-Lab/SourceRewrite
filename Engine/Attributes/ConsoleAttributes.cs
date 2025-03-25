using SourceRewrite.Entities;
using System.Reflection;

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
    /// Variable that can be altered from the Developer Console.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class ConVarAttribute : Attribute
    {
        /// <summary>
        /// Name of the variable.
        /// </summary>
        public string Name { get; set; }

        public ConVarAttribute(string name)
        {
            Name = name;
        }
    }
}
