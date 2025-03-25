using SourceRewrite.Entities.GUI;
using SourceRewrite.Files;
using SourceRewrite.Maps;

namespace SourceRewrite.Components.GUI
{
    /// <summary>
    /// Placeholder FileExplorer GUI Element
    /// </summary>
    public class FileExplorerCanvas : ScreenspaceGUICanvas
    {
        public override void Start()
        {
            PanelName = "elements/file_explorer/file_explorer.html";
            base.Start();

            // On submit pressed
            RegisterEvent("Submit", () =>
            {
                string selectedFile = Canvas.GetElement("file-path").GetProperty("value");
                if (File.Exists(FileSystem.GetMapPath(selectedFile)))
                {
                    MapSystem.UnloadMap();
                    MapSystem.LoadMap(FileSystem.GetMapPath(selectedFile));

                    // We close the file explorer window on submit
                    DestroyDeferred();
                } else
                {
                    // If invalid map was entered we close the window
                    DestroyDeferred();
                }
            });
        }

        public override void OnDestroy()
        {
            UnregisterEvent("Submit");

            base.OnDestroy();
        }
    }
}
