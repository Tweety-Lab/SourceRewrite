using SourceRewrite.Entities;
using System.Diagnostics;
using SourceRewrite.Maps;
using SourceRewrite.Components.GUI;
using SourceRewrite.Entities.GUI;
using SourceRewrite.InputSystem;
using Editor.Entities.GUI;

namespace Editor.Components.GUI
{
    /// <summary>
    /// All Tools in the main toolbar that can be toggled
    /// </summary>
    public enum ToggleableTools
    {
        None,
        Select,
        Magnify,
        Camera,
        Entity,
        Block,
        DecalApply,
        OverlayApply,
        Clipping,
        Vertex
    }

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

            // Load the Map Operations Toolbar
            MainToolbar.Canvas = canvas;
            MainToolbar.RegisterToolbarEvents();
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
