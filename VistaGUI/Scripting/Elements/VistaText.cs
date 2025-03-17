using VistaGUI.Scripting.Elements;

namespace VistaGUI.Scripting.References
{
    /// <summary>
    /// Generic reference to Text Elements.
    /// </summary>
    [VistaElement("p", "b", "i", "span", "h1", "h2", "h3", "h4", "h5", "h6")]
    public class VistaText : VistaElement
    {
        public string TextContent
        {
            get => GetProperty("textContent");
            set => SetProperty("textContent", value);
        }

        public VistaText(string ID, VistaScriptingContext context) : base(ID, context) { }
    }
}
