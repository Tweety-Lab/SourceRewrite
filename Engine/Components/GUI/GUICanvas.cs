using Silk.NET.Input;
using SourceRewrite.AssetTypes;
using SourceRewrite.Files;
using SourceRewrite.GUI;
using SourceRewrite.InputSystem;
using SourceRewrite.Maps;
using SourceRewrite.Objects;
using SourceRewrite.Rendering;
using SourceRewrite.Windowing;
using System.Numerics;
using VistaGUI;
using VistaGUI.Scripting.References;

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

        public int PanelType = 1; // Type of Panel, 0 = worldspace, 1 = screenspace.

        public GUIContainer Container;

        // Worldspace rendering
        private MeshAsset guiMesh;

        public override void Start()
        {
            // Create view config
            GUIConfig config = new GUIConfig();

            config.ResourcesPath = Path.GetFullPath(FileSystem.GamePath.GUIPath);

            config.IsTransparent = isTransparent;
            config.EnableJavaScript = true;

            int resolutionDensity = 12;
            if (PanelType != 0)
            {
                resolutionDensity = 1;
                width = (int)GameWindow.CurrentWindow.WindowSize.X;
                height = (int)GameWindow.CurrentWindow.WindowSize.Y;
            }

            VistaView vistaView = new VistaView(panelName, config, resolutionDensity, height, width);

            Container = new GUIContainer();

            // Create a GUI View
            Container.VistaView = vistaView;

            // Set up GUI Events
            RegisterEvent("ButtonPressed", () => ButtonPressed());

            void ButtonPressed()
            {
                // Set Text
                VistaText outputText = GetElementAsType<VistaText>("output-text");
                outputText.TextContent = "Button Pressed";

                // Set Text to red color
                outputText.SetStyle("color: red;");

                // Change Image
                VistaImage hammerImage = GetElementAsType<VistaImage>("hammer-image");
                hammerImage.ImageSource = "file:///images/hammer_old.png";

                // Disable the Button
                VistaButton button = GetElementAsType<VistaButton>("click-me");
                button.TextContent = "Disabled";
                button.Disabled = true;
            }

            // Render view depending on if it's worldspace or screenspace
            if (PanelType == 0)
            {
                RenderViewToObject();
            } else
            {
                RenderViewToScreen();
            }
        }

        bool isVisible = true;
        public override void Update(float deltaTime)
        {
            // Get current mouse position
            var mousePosition = Input.GetMousePosition();

            Container.VistaView.SendMousePosition(mousePosition);

            // Send Mouse Inputs
            if (Input.GetMouseButtonDown(0))
            {
                Container.VistaView.SendMouseButtonDown(0);
            }

            if (Input.GetMouseButtonUp(0))
            {
                Container.VistaView.SendMouseButtonUp(0);
            }

            // Toggle GUI Visibility
            if (Input.GetKeyDown(Key.Escape) && isVisible)
            {
                Container.VistaView.Visible = false;
                isVisible = false;
            } 
            else if (Input.GetKeyDown(Key.Escape) && !isVisible)
            {
                Container.VistaView.Visible = true;
                isVisible = true;
            }

            // Render view
            Texture texture = Container.RenderToTexture();

            // Update view depending on if it's worldspace or screenspace
            if (PanelType == 0)
            {
                if (guiMesh != null)
                    guiMesh.Material.Texture = texture;
            }
            else
            {

            }
        }

        /// <summary>
        /// Gets an Element from it's ID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public VistaElement GetElement(string id) => Container.VistaView.ScriptingContext.GetElement(id);

        /// <summary>
        /// Gets an Element from it's ID as a specific type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        public T GetElementAsType<T>(string id) where T : VistaElement
        {
            return Container.VistaView.ScriptingContext.GetElementAsType<T>(id);
        }

        /// <summary>
        /// Registers a C# Action that can be called from JavaScript.
        /// </summary>
        /// <param name="name">Javascript function name</param>
        /// <param name="action">C# Action</param>
        public void RegisterEvent(string name, Action action) => Container.VistaView.ScriptingContext.RegisterEvent(name, action);

        /// <summary>
        /// Sets the Inner HTML of an Element from it's ID.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="text"></param>
        public void SetElementInnerHTML(string id, string text) => Container.VistaView.ScriptingContext.SetElementInnerHTML(id, text);

        // Render a GUIView in worldspace
        private void RenderViewToObject()
        {
            // Create a Material from the view output
            Material guiMaterial = new Material("dev/missing");
            guiMaterial.Texture = Container.Output;

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

        // Render a GUIView in screenspace
        private void RenderViewToScreen()
        {
            var windowSize = GameWindow.CurrentWindow.WindowSize;
        }
    }
}
