using SourceRewrite.Files;
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
            // Load HTML from GUI Demo
            string HTMLContent = File.ReadAllText(FileSystem.GetGUIPath("gui_demo.html"));

            // Create a new GUI Context
            Views.Add(new GUIView(HTMLContent));
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
