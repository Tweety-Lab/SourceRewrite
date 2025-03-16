using SourceRewrite.AssetTypes;
using SourceRewrite.Components;
using SourceRewrite.Files;
using SourceRewrite.InputSystem;
using SourceRewrite.Objects;
using System.Diagnostics;
using System;
using System.Numerics;
using VistaGUI.Scripting.References;
using SourceRewrite.Maps;

namespace Editor.Components.GUI
{
    public class EditorCanvas : GameComponent
    {
        private GUICanvas canvas;

        public override void Start()
        {
            Console.WriteLine("Started Editor GUI");

            canvas = new GUICanvas();
            canvas.IsTransparent = true;
            canvas.PanelName = "editor/editor.html";

            GameObject.AddComponent(canvas);

            // HACK: Manually start the canvas component
            canvas.Start();

            GameObject lightEditorObject = null;

            // Register GUI Evenets
            canvas.RegisterEvent("OpenVDC", () => Process.Start(new ProcessStartInfo("https://developer.valvesoftware.com/wiki/Main_Page") { UseShellExecute = true }));

            bool lightEditorOpen = false;

            // Open Light Editor if it isn't already opened
            canvas.RegisterEvent("OpenLightEditor", OpenLightEditor);
            void OpenLightEditor()
            {
                if (!lightEditorOpen)
                {
                    // Create gameobject to house Light Editor GUICanvas
                    lightEditorObject = new GameObject();

                    // Add Light Edtior GUICanvas to gameobject
                    LightEditorCanvas lightEditorCanvas = new LightEditorCanvas();
                    lightEditorObject.AddComponent(lightEditorCanvas);

                    // Start GUI
                    lightEditorCanvas.Start();

                    lightEditorOpen = true;
                }
            }

            // Close Light Editor if it isn't already closed
            canvas.RegisterEvent("CloseLightEditor", CloseLightEditor);
            void CloseLightEditor()
            {
                if (lightEditorOpen)
                {
                    lightEditorObject.DestroyDeferred();
                    lightEditorOpen = false;
                }
            }

            // Unload Map
            canvas.RegisterEvent("CloseMap", () => MapSystem.UnloadMap());

            // Play Buttons
            canvas.RegisterEvent("PlayGame", () => Process.Start("Engine.exe")); // Just start Engine.exe as it loads game.dll
            canvas.RegisterEvent("PlayCurrentMap", () => Process.Start("Engine.exe", $"-map {MapSystem.CurrentMap.BSPFilePath}"));

        }

        public override void Update()
        {
        
        }

        public override void OnDestroy()
        {
            // Unregister all events
            canvas.UnregisterEvent("OpenVDC");
            canvas.UnregisterEvent("OpenLightEditor");
            canvas.UnregisterEvent("CloseLightEditor");
            canvas.UnregisterEvent("CloseMap");
            canvas.UnregisterEvent("PlayGame");
            canvas.UnregisterEvent("PlayCurrentMap");
        }
    }
}
