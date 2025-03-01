using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SourceRewrite.Maps
{
    /// <summary>
    /// Map Properties are variables that are defined in the BSP (map) file.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true)]
    public class MapPropertyAttribute : Attribute
    {
        /// <summary>
        /// Name of the Property in the map file, should generally follow snake_case.
        /// </summary>
        public string Name { get; set; }

        public MapPropertyAttribute(string name)
        {
            // Assign the name
            Name = name;

            // Add the underscore at the start
            Name = "_" + Name;
        }
    }
}
