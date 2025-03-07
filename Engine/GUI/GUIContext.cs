using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UltralightNet;
using UltralightNet.AppCore;

namespace SourceRewrite.GUI
{
    public class GUIContext
    {
        public List<GUIView> Views = new List<GUIView>();

        public GUIContext()
        {
            // Create a new GUI Context
            Views.Add(new GUIView());
        }

        public void Update()
        {
            foreach (GUIView view in Views)
            {
                view.Update();
            }
        }
    }
}
