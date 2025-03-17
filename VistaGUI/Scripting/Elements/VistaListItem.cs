using VistaGUI.Scripting.Elements;

namespace VistaGUI.Scripting.References
{
    /// <summary>
    /// Generic reference to List Item (LI) Elements.
    /// </summary>
    [VistaElement("li")]
    public class VistaListItem : VistaElement
    {
        /// <summary>
        /// Text Content of the List Item.
        /// </summary>
        public string TextContent
        {
            get => GetProperty("textContent");
            set => SetProperty("textContent", value);
        }

        /// <summary>
        /// Determines if the List Item is marked as selected (for example, in a checklist or ordered list).
        /// </summary>
        public bool Selected
        {
            get => GetProperty("selected") == "true";
            set => SetProperty("selected", value.ToString());
        }

        /// <summary>
        /// Constructor for the List Item (LI) reference.
        /// </summary>
        public VistaListItem(string ID, VistaScriptingContext context) : base(ID, context) { }

        /// <summary>
        /// Remove this List Item (LI) from its parent List (UL/OL).
        /// </summary>
        public void Remove()
        {
            string currentContent = GetProperty("outerHTML");
            SetProperty("outerHTML", string.Empty); // Remove the item by clearing its outer HTML
        }
    }
}
