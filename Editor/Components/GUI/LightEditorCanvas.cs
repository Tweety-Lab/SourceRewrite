using SourceRewrite.AssetTypes;
using SourceRewrite.Components;
using SourceRewrite.Files;
using SourceRewrite.InputSystem;
using SourceRewrite.Objects;
using System.Numerics;
using VistaGUI.Scripting.References;

namespace Editor.Components.GUI
{
    public class LightEditorCanvas : GameComponent
    {
        private GUICanvas canvas;

        enum TranslationMode
        {
            None,
            Position
        }

        private TranslationMode translationMode = TranslationMode.None;

        public override void Start()
        {
            Console.WriteLine("Started Light Editor GUI");

            canvas = new GUICanvas();
            canvas.IsTransparent = true;
            canvas.PanelName = "editor/light_editor.html";

            GameObject.AddComponent(canvas);

            // HACK: Manually start the canvas component
            canvas.Start();

            // Transforms
            canvas.RegisterEvent("PositionMode", () => translationMode = TranslationMode.Position);
            canvas.RegisterEvent("NoneMode", () => translationMode = TranslationMode.None);

            // Toggle Light Mesh Visualisation
            canvas.RegisterEvent("ToggleLights", () => ToggleLights());
            void ToggleLights()
            {
                foreach (GameObject gameObject in GameObject.ActiveObjects)
                {
                    if (gameObject.GetComponentFromType<PointLight>() != null && gameObject.GetComponentFromType<MeshRenderer>() == null)
                    {
                        Mesh mesh = new Mesh(FileSystem.GetModelPath("primitives/cube.model"), FileSystem.GetMaterial("dev/error"));
                        MeshRenderer meshRenderer = new MeshRenderer();
                        meshRenderer.Mesh = mesh;

                        gameObject.AddComponent(meshRenderer);
                    }
                    else if (gameObject.GetComponentFromType<PointLight>() != null && gameObject.GetComponentFromType<MeshRenderer>() != null)
                    {
                        gameObject.RemoveComponentOfType<MeshRenderer>();
                    }
                }
            }

            // Load a BSP (right now just sets current map text)
            canvas.RegisterEvent("LoadBSP", () => LoadBSP());
            void LoadBSP()
            {
                // Set Text
                VistaText currentMapText = canvas.GetElementAsType<VistaText>("current-map");
                currentMapText.TextContent = "Current Map: 'maps/bsp_test.bsp'";
            }

            // Delete all lights in the scene
            canvas.RegisterEvent("DeleteLights", () => DeleteLights());
            void DeleteLights()
            {
                foreach (GameObject gameObject in GameObject.ActiveObjects)
                {
                    if (gameObject.GetComponentFromType<PointLight>() != null)
                    {
                        gameObject.DestroyDeferred();
                    }
                }
            }
        }

        public override void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (translationMode == TranslationMode.Position)
                {
                    foreach (GameObject gameObject in GameObject.ActiveObjects)
                    {
                        if (gameObject.GetComponentFromType<PointLight>() != null)
                        {
                            Vector3 movement = new Vector3(Input.GetMouseMovement().X, 0, Input.GetMouseMovement().Y);
                            gameObject.Transform.Position += movement;
                        }
                    }
                }
            }
        }

        public override void OnDestroy()
        {
            // Unregister all events
            canvas.UnregisterEvent("PositionMode");
            canvas.UnregisterEvent("NoneMode");
            canvas.UnregisterEvent("ToggleLights");
            canvas.UnregisterEvent("LoadBSP");
            canvas.UnregisterEvent("DeleteLights");
        }
    }
}
