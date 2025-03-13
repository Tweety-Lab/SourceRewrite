using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VistaGUI.Scripting.References
{
    /// <summary>
    /// Generic reference to Button Elements.
    /// </summary>
    public class ButtonReference : ElementReference
    {
        public string TextContent
        {
            get => GetProperty("textContent");
            set => SetProperty("textContent", value);
        }

        public ButtonReference(string ID, VistaScriptingContext context) : base(ID, context) { }
    }
}
