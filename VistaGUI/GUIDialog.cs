using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UltralightNet.AppCore;

namespace VistaGUI
{
    /// <summary>
    /// Standalone GUI View with its own window
    /// </summary>
    public class GUIDialog : GUIView
    {
        private readonly ULApp app;
        private readonly ULWindow window;

        /// <summary>
        /// Sets the window title.
        /// </summary>
        public string Title
        {
            set => window.Title = value;
        }

        public GUIDialog(string HTML, GUIConfig viewConfig, int ResolutionScale, int height, int width) : base(HTML, viewConfig, ResolutionScale, height, width)
        {
            app = ULApp.Create(new(), new());
            window = app.MainMonitor.CreateWindow((uint)height, (uint)width);

            window.Title = "Dialog";

            using var overlay = window.CreateOverlay(window.ScreenWidth, window.ScreenHeight);
            window.OnResize += (uint newWidth, uint newHeight) => overlay.Resize(newWidth, newHeight);
            window.OnClose += () => app.Quit();

            var view = overlay.View;

            view.HTML = HTML;
        }

        // Show the window
        public void ShowDialog()
        {
            window.Show();
        }

        // Hide the window
        public void HideDialog()
        {
            window.Hide();
        }
    }
}
