using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SourceRewrite.Components.GUI
{
    /// <summary>
    /// Placeholder FileExplorer GUI Element
    /// </summary>
    public class FileExplorerCanvas : GameComponent
    {
        private GUICanvas canvas;

        public override void Start()
        {
            canvas = new GUICanvas();
            canvas.IsTransparent = true;
            canvas.PanelName = "elements/file_explorer/file_explorer.html";

            GameObject.AddComponent(canvas);

            // HACK: Manually start the canvas component
            canvas.Start();
        }
    }
}
