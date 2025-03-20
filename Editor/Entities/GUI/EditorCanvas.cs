using SourceRewrite.Entities;
using System.Diagnostics;
using SourceRewrite.Maps;
using SourceRewrite.Components.GUI;
using SourceRewrite.Entities.GUI;

namespace Editor.Components.GUI
{
    public class EditorCanvas : BaseEntity
    {
        private GUICanvasEntity canvas;

        public override void Start()
        {
            Console.WriteLine("Started Editor GUI");

            canvas = new GUICanvasEntity();
            canvas.IsTransparent = true;
            canvas.PanelName = "editor/editor.html";
            EntityManager.AddGlobalEntity(canvas);

            // HACK: Manually start the canvas component
            canvas.Start();

            // Register GUI Events
            RegisterGUIEvents();
        }

        public override void Update()
        {

        }

        public override void OnDestroy()
        {
            // Unregister all events
            canvas.UnregisterEvent("OpenVDC");
            canvas.UnregisterEvent("CloseMap");
            canvas.UnregisterEvent("PlayGame");
            canvas.UnregisterEvent("PlayCurrentMap");
            canvas.UnregisterEvent("OpenMap");

        }

        // Register GUI Events
        private void RegisterGUIEvents()
        {
            RegisterMenuEvents();
        }

        private void RegisterMenuEvents()
        {
            // Open VDC Website
            canvas.RegisterEvent("OpenVDC", () => Process.Start(new ProcessStartInfo("https://developer.valvesoftware.com/wiki/Main_Page") { UseShellExecute = true }));

            // Unload Map
            canvas.RegisterEvent("CloseMap", () => MapSystem.UnloadMap());

            // Play Buttons
            canvas.RegisterEvent("PlayGame", () => Process.Start("Engine.exe")); // Just start Engine.exe as it loads game.dll

            // Start Engine.exe with the current map as an argument
            canvas.RegisterEvent("PlayCurrentMap", () =>
            {
                string mapFileName = MapSystem.CurrentMap.BSPFilePath.Split('/').Last(); // Get the file name of the map
                Process.Start("Engine.exe", $"-map {mapFileName}"); // Start the process with the map argument
            });

            // Allow user to select map file then open it
            canvas.RegisterEvent("OpenMap", () =>
            {
                // Add FileExplorerCanvas entity
                FileExplorerCanvas fileExplorerCanvas = new FileExplorerCanvas();
                fileExplorerCanvas.Parent = this;

                // Start GUI
                fileExplorerCanvas.Start();
            });
        }
    }
}
