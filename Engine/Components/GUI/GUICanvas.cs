using Silk.NET.Input;
using SourceRewrite.AssetTypes;
using SourceRewrite.Files;
using SourceRewrite.GUI;
using SourceRewrite.Maps;
using SourceRewrite.Objects;
using SourceRewrite.Windowing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using UltralightNet;

namespace SourceRewrite.Components
{
    /// <summary>
    /// GUI Renderer.
    /// </summary>
    public class GUICanvas : GameComponent
    {
        [MapProperty("height")]
        private int height; // Panel Height

        [MapProperty("width")]
        private int width; // Panel Width

        [MapProperty("panelname")]
        private string panelName; // Name of HTML Panel to render

        [MapProperty("IsTransparent")]
        private bool isTransparent; // Is the panel background transparent

        GUIView view;
        MeshAsset guiMesh;

        public override void Start()
        {
            // Read HTML from panel
            string htmlContent = File.ReadAllText(FileSystem.GetGUIPath(panelName));

            // Create view config
            ULViewConfig config = new ULViewConfig();
            config.IsTransparent = isTransparent;
            config.EnableJavaScript = true;

            // Create a GUI View
            view = new GUIView(htmlContent, config, 12, height, width);
            GameWindow.CurrentWindow.GUI.Views.Add(view);

            RenderViewToObject();
        }

        public override void Update(float deltaTime)
        {
            // Send Mouse Position
            view.SendMousePosition(InputSystem.Input.GetMousePosition());

            // Send Mouse Inputs
            if (InputSystem.Input.GetMouseButtonDown(0))
            {
                view.SendMouseButtonDown(MouseButton.Left);
            }

            // Update View
            if (guiMesh != null)
                guiMesh.Material.Texture = view.Output;
        }

        // Render a GUIView in worldspace
        private void RenderViewToObject()
        {
            // Create a Material from the view output
            Material guiMaterial = new Material("dev/missing");
            guiMaterial.Texture = view.Output;

            // Create a plane mesh to house the gui
            guiMesh = new Mesh(FileSystem.GetModelPath("primitives/plane.model"), guiMaterial);

            // Create a holder object
            GameObject holder = new GameObject();
            holder.Transform.Scale = new Vector3(height / 32, 1f, width / 32);

            // Add Transform
            holder.Transform.Position = GameObject.Transform.Position;
            holder.Transform.Rotation = GameObject.Transform.Rotation;

            // Add MeshRenderer
            MeshRenderer cubeRenderer = new MeshRenderer();
            holder.AddComponent(cubeRenderer);

            // Render the gui mesh
            cubeRenderer.Mesh = guiMesh;
        }
    }
}
