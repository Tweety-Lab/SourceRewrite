using System;
using VistaGUI;

namespace SDKLauncher
{
    class Program
    {
        private static GUIDialog dialog;
        private static string title = "SDK Launcher";

        static void Main(string[] args)
        {
            string HTML = File.ReadAllText("sdklauncher_dialog.html");
            GUIConfig config = new GUIConfig();

            dialog = new GUIDialog(HTML, config, 1, 270, 355, title);

            dialog.Title = "SDK Launcher";
        }
    }
}