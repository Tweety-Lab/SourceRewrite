using VistaGUI.Scripting.References;

namespace VistaGUI.Scripting
{
    
    public static class TypeDictionary
    {
        // A dictionary mapping element types (HTML tags) to corresponding C# types.
        public static readonly Dictionary<string, Type> Types = new()
        {
            // Text elements
            ["p"] = typeof(TextReference),
            ["b"] = typeof(TextReference),
            ["i"] = typeof(TextReference),
            ["span"] = typeof(TextReference),
            ["h1"] = typeof(TextReference),
            ["h2"] = typeof(TextReference),
            ["h3"] = typeof(TextReference),

            // Image Elements
            ["img"] = typeof(ImageReference),

            // Button Elements
            ["button"] = typeof(ButtonReference)

        };
    }
}
