using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UltralightNet;

namespace VistaGUI
{
    // Update all GUI Views
    public static class GUIContext
    {
        public static List<GUIView> Views = new List<GUIView>();

        // This needs to be ran once per frame somewhere in the program
        public static void Update()
        {
            foreach (GUIView view in Views)
            {
                view.Update();
            }
        }
    }
}
