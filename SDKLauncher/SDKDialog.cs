using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VistaGUI;

namespace SDKLauncher
{
    public class SDKDialog
    {
        public GUIDialog Dialog;
        private string title = "SDK Launcher";

        public SDKDialog()
        {
            string HTML = File.ReadAllText("sdklauncher_dialog.html");
            GUIConfig config = new GUIConfig();

            Dialog = new GUIDialog(HTML, config, 1, 270, 355, title);

            Dialog.Title = title;
        }
    }
}
