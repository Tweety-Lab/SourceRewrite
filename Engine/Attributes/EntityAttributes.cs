namespace SourceRewrite.Attributes
{
    /// <summary>
    /// Defines the Class Name of the Entity for use in Hammer.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class EntityAttribute : Attribute
    {
        public string ClassName { get; set; }

        public EntityAttribute(string className)
        {
            // Assign the class name
            ClassName = className;
        }
    }


    /// <summary>
    /// Entity Properties are variables that are defined in the BSP (map) file.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true)]
    public class EntityPropertyAttribute : Attribute
    {
        /// <summary>
        /// Name of the Property in the map file.
        /// </summary>
        public string Name { get; set; }

        public EntityPropertyAttribute(string name)
        {
            // Assign the name
            Name = name;
        }
    }

    /// <summary>
    /// Allows the Entity to run no matter what. Will Run in Editor, Play Mode, etc.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class AlwaysExecuteAttribute : Attribute { }
}
