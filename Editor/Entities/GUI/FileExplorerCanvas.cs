using SourceRewrite.Entities;
using SourceRewrite.Entities.GUI;
using SourceRewrite.Files;
using SourceRewrite.Maps;

namespace SourceRewrite.Components.GUI
{
    /// <summary>
    /// Placeholder FileExplorer GUI Element
    /// </summary>
    public class FileExplorerCanvas : BaseEntity
    {
        public GUICanvasEntity Canvas;

        public override void Start()
        {
            Canvas = new GUICanvasEntity();
            Canvas.IsTransparent = true;
            Canvas.PanelName = "elements/file_explorer/file_explorer.html";
            EntityManager.AddGlobalEntity(Canvas);

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
                    DestroyDeferred();
                }
            });
        }

        public override void OnDestroy()
        {
            Canvas.UnregisterEvent("Submit");
        }
    }
}
