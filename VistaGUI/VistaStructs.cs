using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VistaGUI
{
    // Abstraction for Ultralight ULViewConfig
    public struct GUIConfig
    {
        public bool IsTransparent { get; set; }
        public bool EnableJavaScript { get; set; }

        public string ResourcesPath { get; set; }
    }
}
