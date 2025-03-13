using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VistaGUI.Scripting.References;

namespace VistaGUI.Scripting
{
    // A dictionary mapping element types (HTML tags) to corresponding C# types.
    public static class TypeDictionary
    {
        public static readonly Dictionary<string, Type> Types = new Dictionary<string, Type>
        {
            { "p", typeof(TextReference) },
            { "b", typeof(TextReference) },
            { "i", typeof(TextReference) },
            { "span", typeof(TextReference) },
            { "h1", typeof(TextReference) },
            { "h2", typeof(TextReference) },
            { "h3", typeof(TextReference) }
        };
    }
}
