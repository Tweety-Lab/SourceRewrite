namespace VistaGUI.Scripting.References
{
    /// <summary>
    /// Generic reference to Unordered List (UL) Elements.
    /// </summary>
    public class VistaUnorderedList : VistaElement
    {

        /// <summary>
        /// Add a List Item (LI) to the Unordered List (UL).
        /// </summary>
        public void AddListItem(string listItemText)
        {
            // Adds a list item (LI) with the specified text content to the UL.
            string currentContent = GetProperty("textContent");
            SetProperty("textContent", currentContent + "<li>" + listItemText + "</li>");
        }

        /// <summary>
        /// Remove a List Item (LI) from the Unordered List (UL) by its text.
        /// </summary>
        public void RemoveListItem(string listItemText)
        {
            string currentContent = GetProperty("textContent");
            string itemToRemove = "<li>" + listItemText + "</li>";
            if (currentContent.Contains(itemToRemove))
            {
                SetProperty("textContent", currentContent.Replace(itemToRemove, string.Empty));
            }
        }

        public VistaUnorderedList(string ID, VistaScriptingContext context) : base(ID, context) { }
    }
}
