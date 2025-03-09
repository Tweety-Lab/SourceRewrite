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

        public void Update()
        {
            foreach (GUIView view in Views)
            {
                view.Update();
            }
        }
    }
}
