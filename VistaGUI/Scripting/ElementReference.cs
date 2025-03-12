using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VistaGUI.Scripting
{
    // Represents a reference to an HTML element
    public class ElementReference
    {
        public string ID { get; private set; }
        private GUIScriptingContext Context { get; set; }

        public ElementReference(string id, GUIScriptingContext context)
        {
            ID = id;
            Context = context;
        }

        // Functions

        public void SetInnerHTML(string text) => Context.SetElementInnerHTML(ID, text);

        public void SetProperty(string property, string value) => Context.SetElementProperty(ID, property, value);
    }
}
