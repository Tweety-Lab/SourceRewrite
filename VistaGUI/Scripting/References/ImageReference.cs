using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VistaGUI.Scripting.References
{
    /// <summary>
    /// Generic reference to Image Elements.
    /// </summary>
    public class ImageReference : ElementReference
    {
        public string ImageSource
        {
            get => GetProperty("src");
            set => SetProperty("src", value);
        }

        public ImageReference(string ID, VistaScriptingContext context) : base(ID, context) { }
    }
}
