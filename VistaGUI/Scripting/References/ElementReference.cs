using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace VistaGUI.Scripting.References
{
    // Reference to a Generic HTML element.
    public class ElementReference
    {
        public string ID { get; private set; }
        private VistaScriptingContext Context { get; set; }

        public string InnerHTML
        {
            get => GetInnerHTML();
            set => SetInnerHTML(value);
        }

        public ElementReference(string id, VistaScriptingContext context)
        {
            ID = id;
            Context = context;
        }

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
