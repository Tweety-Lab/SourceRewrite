using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VistaGUI.Scripting.Elements
{
    // Metadata for VistaElements
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
    public class VistaElementAttribute : Attribute
    {
        public string[] TagNames { get; set; }

        // Constructor to accept a single tag name or multiple tag names
        public VistaElementAttribute(params string[] tagNames)
        {
            TagNames = tagNames;
        }
    }
}
