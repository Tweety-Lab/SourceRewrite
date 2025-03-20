namespace SourceRewrite.Maps
{
    /// <summary>
    /// Entity Properties are variables that are defined in the BSP (map) file.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true)]
    public class EntityPropertyAttribute : Attribute
    {
        /// <summary>
        /// Name of the Property in the map file
        /// </summary>
        public string Name { get; set; }

        public EntityPropertyAttribute(string name)
        {
            // Assign the name
            Name = name;
        }
    }
}
