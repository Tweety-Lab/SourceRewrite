using SourceRewrite.Files;
using SourceRewrite.GUI;
using SourceRewrite.Maps;
using SourceRewrite.Windowing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SourceRewrite.Components
{
    /// <summary>
    /// GUI Renderer.
    /// </summary>
    public class GUICanvas : GameComponent
    {
        [MapProperty("height")]
        private int height; // Panel Height

        [MapProperty("width")]
        private int width; // Panel Width

        [MapProperty("panelname")]
        private string panelName; // Name of HTML Panel to render

        public override void Start()
        {
            // Read HTML from panel
            string htmlContent = File.ReadAllText(FileSystem.GetGUIPath(panelName));

            // Create a GUI View
            GUIView view = new GUIView(htmlContent, height, width);
            GameWindow.CurrentWindow.GUI.Views.Add(view);
        }
    }
}
