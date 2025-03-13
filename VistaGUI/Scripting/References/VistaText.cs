using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VistaGUI.Scripting.References
{
    /// <summary>
    /// Generic reference to Text Elements.
    /// </summary>
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
