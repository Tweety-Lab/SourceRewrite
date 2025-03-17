using VistaGUI.Scripting.Elements;

namespace VistaGUI.Scripting.References
{
    /// <summary>
    /// Generic reference to Button Elements.
    /// </summary>
    [VistaElement("button")]
    public class VistaButton : VistaElement
    {
        /// <summary>
        /// Text Content of the Button.
        /// </summary>
        public string TextContent
        {
            get => GetProperty("textContent");
            set => SetProperty("textContent", value);
        }

        /// <summary>
        /// Determines if the button is disabled.
        /// </summary>
        public bool Disabled
        {
            get => GetProperty("disabled") == "true";
            set => SetProperty("disabled", value.ToString());
        }

        public VistaButton(string ID, VistaScriptingContext context) : base(ID, context) { }
    }
}
