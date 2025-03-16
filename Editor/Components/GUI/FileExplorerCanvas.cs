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
        public GUICanvas Canvas;

        public override void Start()
        {
            Canvas = new GUICanvas();
            Canvas.IsTransparent = true;
            Canvas.PanelName = "elements/file_explorer/file_explorer.html";

            GameObject.AddComponent(Canvas);

            // HACK: Manually start the canvas component
            Canvas.Start();
        }

        public override void OnDestroy()
        {
            Canvas.UnregisterEvent("Submit");
        }
    }
}
