using SourceRewrite.Files;
using SourceRewrite.Maps;
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

            // On submit pressed
            Canvas.RegisterEvent("Submit", () =>
            {
                string selectedFile = Canvas.GetElement("file-path").GetProperty("value");
                if (File.Exists(FileSystem.GetMapPath(selectedFile)))
                {
                    MapSystem.UnloadMap();
                    MapSystem.LoadMap(FileSystem.GetMapPath(selectedFile));

                    // We close the file explorer window on submit
                    GameObject.DestroyDeferred();
                }
            });
        }

        public override void OnDestroy()
        {
            Canvas.UnregisterEvent("Submit");
        }
    }
}
