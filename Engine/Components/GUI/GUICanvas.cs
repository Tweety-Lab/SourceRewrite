using Silk.NET.Input;
using Silk.NET.Maths;
using SourceRewrite.AssetTypes;
using SourceRewrite.Files;
using SourceRewrite.GUI;
using SourceRewrite.InputSystem;
using SourceRewrite.Maps;
using SourceRewrite.Objects;
using SourceRewrite.Windowing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
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

        private GUIView view;
        private MeshAsset guiMesh;

        public override void Start()
        {
            // Read HTML from panel
            string htmlContent = System.IO.File.ReadAllText(FileSystem.GetGUIPath(panelName));

            // Create view config
            ULViewConfig config = new ULViewConfig();
            config.IsTransparent = isTransparent;
            config.EnableJavaScript = true;

            // Create a GUI View
            view = new GUIView(htmlContent, config, 12, height, width);
            GameWindow.CurrentWindow.GUI.Views.Add(view);

            // Set up GUI Events
            RegisterEvent("PrintMessage", () => Console.WriteLine("Test Print defined in C# called from JS."));

            RenderViewToObject();
        }

        bool isVisible = true;
        public override void Update(float deltaTime)
        {
            // Get current mouse position
            var mousePosition = Input.GetMousePosition();

            view.SendMousePosition(mousePosition);

            // Send Mouse Inputs
            if (Input.GetMouseButtonDown(0))
            {
                view.SendMouseButtonDown(MouseButton.Left);
            }

            if (Input.GetMouseButtonUp(0))
            {
                view.SendMouseButtonUp(MouseButton.Left);
            }

            // Toggle GUI Visibility
            if (Input.GetKeyDown(Key.Escape) && isVisible)
            {
                view.Visible = false;
                isVisible = false;
            } 
            else if (Input.GetKeyDown(Key.Escape) && !isVisible)
            {
                view.Visible = true;
                isVisible = true;
            }

            // Update View
            if (guiMesh != null)
                guiMesh.Material.Texture = view.Output;
        }

        /// <summary>
        /// Registers a C# Action that can be called from JavaScript.
        /// </summary>
        /// <param name="name">Javascript function name</param>
        /// <param name="action">C# Action</param>
        public void RegisterEvent(string name, Action action)
        {
            view.RegisterEvent(name, action);
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
            holder.Transform.Scale = new Vector3(width / 32, 1f, height / 32);

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
