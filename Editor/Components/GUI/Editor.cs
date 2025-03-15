using SourceRewrite.AssetTypes;
using SourceRewrite.Components;
using SourceRewrite.Files;
using SourceRewrite.InputSystem;
using SourceRewrite.Objects;
using System.Diagnostics;
using System;
using System.Numerics;
using VistaGUI.Scripting.References;

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

            // Register GUI Evenets
            canvas.RegisterEvent("OpenVDC", () => Process.Start(new ProcessStartInfo("https://developer.valvesoftware.com/wiki/Main_Page") { UseShellExecute = true }));

            canvas.RegisterEvent("OpenLightEditor", OpenLightEditor);
            void OpenLightEditor()
            {
                LightEditorCanvas lightEditorCanvas = new LightEditorCanvas();

                GameObject.AddComponent(lightEditorCanvas);

                lightEditorCanvas.Start();
            }
        }

        public override void Update(float deltaTime)
        {
        
        }
    }
}
