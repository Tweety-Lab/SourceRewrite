using SourceRewrite.Entities;
using System.Diagnostics;
using SourceRewrite.Maps;
using SourceRewrite.Components.GUI;
using SourceRewrite.Entities.GUI;
using SourceRewrite.InputSystem;
using Editor.Logic;

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

    public class EditorCanvas : ScreenspaceGUICanvas
    {
        public override void Start()
        {
            PanelName = "editor/editor.html";
            base.Start();

            // Register GUI Events
            RegisterGUIEvents();
        }

        public override void Update()
        {

        }

        public override void OnDestroy()
        {
            // Unregister all events
            UnregisterEvent("OpenVDC");
            UnregisterEvent("CloseMap");
            UnregisterEvent("PlayGame");
            UnregisterEvent("PlayCurrentMap");
            UnregisterEvent("OpenMap");

            // Cleanup Toolbar
            MainToolbar.UnregisterToolbarEvents();

        }

        // Register GUI Events
        private void RegisterGUIEvents()
        {
            RegisterMenuEvents();

            // Load the Map Operations Toolbar
            MainToolbar.Canvas = Canvas;
            MainToolbar.RegisterToolbarEvents();
        }

        private void RegisterMenuEvents()
        {
            // Open VDC Website
            RegisterEvent("OpenVDC", () => Process.Start(new ProcessStartInfo("https://developer.valvesoftware.com/wiki/Main_Page") { UseShellExecute = true }));

            // Unload Map
            RegisterEvent("CloseMap", () => MapSystem.UnloadMap());

            // Play Buttons
            RegisterEvent("PlayGame", () => Process.Start("Engine.exe")); // Just start Engine.exe as it loads game.dll

            // Start Engine.exe with the current map as an argument
            RegisterEvent("PlayCurrentMap", () =>
            {
                string mapFileName = MapSystem.CurrentMap.BSPFilePath.Split('/').Last(); // Get the file name of the map
                Process.Start("Engine.exe", $"-map {mapFileName}"); // Start the process with the map argument
            });

            // Allow user to select map file then open it
            RegisterEvent("OpenMap", () =>
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
