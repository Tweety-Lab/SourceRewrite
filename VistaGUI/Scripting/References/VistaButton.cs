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
    public class VistaButton : VistaElement
    {
        public string TextContent
        {
            get => GetProperty("textContent");
            set => SetProperty("textContent", value);
        }

        public VistaButton(string ID, VistaScriptingContext context) : base(ID, context) { }
    }
}
