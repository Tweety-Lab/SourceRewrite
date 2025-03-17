using VistaGUI.Scripting.Elements;

namespace VistaGUI.Scripting.References
{
    // Reference to a Generic HTML element.
    public class VistaElement
    {
        public string ID { get; private set; }
        private VistaScriptingContext Context { get; set; }

        // Generate HTML for this element
        public string GenerateHTML()
        {
            var attribute = (VistaElementAttribute)Attribute.GetCustomAttribute(this.GetType(), typeof(VistaElementAttribute));

            // Check if the attribute is found and has at least one tag name
            if (attribute?.TagNames?.Length > 0)
            {
                var tagName = attribute.TagNames[0]; // Use the first tag name
                return $"<{tagName} id=\"{ID}\"></{tagName}>";
            }

            return string.Empty;
        }

        public string InnerHTML
        {
            get => GetInnerHTML();
            set => SetInnerHTML(value);
        }

        public string OuterHTML
        {
            get => GetProperty("outerHTML");
            set => SetProperty("outerHTML", value);
        }

        public VistaElement(string id, VistaScriptingContext context)
        {
            ID = id;
            Context = context;
        }

        // Events
        public void AddEvent(string eventName, string functionName) => Context.AddEvent(ID, eventName, functionName);

        // Inner HTML
        private void SetInnerHTML(string text) => Context.SetElementInnerHTML(ID, text);
        private string GetInnerHTML() => Context.GetElementInnerHTML(ID);

        // Element Properties
        public void SetProperty(string property, string value) => Context.SetElementProperty(ID, property, value);
        public string GetProperty(string property) => Context.GetElementProperty(ID, property);

        // Element Style
        public void SetStyle(string style) => Context.SetElementStyle(ID, style);
        public string GetStyle() => Context.GetElementStyle(ID);
    }
}
