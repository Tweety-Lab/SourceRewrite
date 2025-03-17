using VistaGUI.Scripting.Elements;

namespace VistaGUI.Scripting.References
{
    /// <summary>
    /// Generic reference to Unordered List (UL) Elements.
    /// </summary>
    [VistaElement("ul")]
    public class VistaUnorderedList : VistaElement
    {

        /// <summary>
        /// Add a List Item (LI) to the Unordered List (UL).
        /// </summary>
        public void AddListItem(VistaListItem listItem)
        {
            // Adds a list item (LI) to the UL by using the list item instance (VistaListItem)
            string currentContent = GetProperty("innerHTML");
            string listItemHTML = listItem.GenerateHTML();  // Generate the HTML for the ListItem
            SetProperty("innerHTML", currentContent + listItemHTML);
        }

        /// <summary>
        /// Add a List Item (LI) to the Unordered List (UL).
        /// </summary>
        public void AddListItem(string listItemText)
        {
            // Adds a list item (LI) with the specified text content to the UL.
            string currentContent = GetProperty("innerHTML");
            SetProperty("innerHTML", currentContent + "<li>" + listItemText + "</li>");
        }

        /// <summary>
        /// Remove a List Item (LI) from the Unordered List (UL) by its text.
        /// </summary>
        public void RemoveListItem(string listItemText)
        {
            string currentContent = GetProperty("innerHTML");
            string itemToRemove = "<li>" + listItemText + "</li>";
            if (currentContent.Contains(itemToRemove))
            {
                SetProperty("innerHTML", currentContent.Replace(itemToRemove, string.Empty));
            }
        }

        /// <summary>
        /// Clear all List Items (LI) from the Unordered List (UL).
        /// </summary>
        public void Clear()
        {
            // Clears all items by setting the innerHTML to an empty string.
            SetProperty("innerHTML", string.Empty);
        }

        public VistaUnorderedList(string ID, VistaScriptingContext context) : base(ID, context) { }
    }
}
