using VistaGUI.Scripting.References;

namespace VistaGUI.Scripting
{
    
    public static class TypeDictionary
    {
        // A dictionary mapping element types (HTML tags) to corresponding C# types.
        public static readonly Dictionary<string, Type> Types = new()
        {
            // Text elements
            ["p"] = typeof(VistaText),
            ["b"] = typeof(VistaText),
            ["i"] = typeof(VistaText),
            ["span"] = typeof(VistaText),
            ["h1"] = typeof(VistaText),
            ["h2"] = typeof(VistaText),
            ["h3"] = typeof(VistaText),

            // Image Elements
            ["img"] = typeof(VistaImage),

            // Button Elements
            ["button"] = typeof(VistaButton),

            // Lists
            ["ul"] = typeof(VistaUnorderedList)

        };
    }
}
