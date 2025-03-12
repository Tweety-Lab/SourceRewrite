using Silk.NET.Input;
using SourceRewrite.AssetTypes;
using SourceRewrite.Files;
using SourceRewrite.GUI;
using SourceRewrite.InputSystem;
using SourceRewrite.Maps;
using SourceRewrite.Objects;
using SourceRewrite.Rendering;
using System.Numerics;
using VistaGUI;
using VistaGUI.Processor;

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

        private GUIContainer container;
        private MeshAsset guiMesh;

        public override void Start()
        {
            // Read VistaXML from panel
            string xmlContent = File.ReadAllText(FileSystem.GetGUIPath(panelName));

            // Convert it to WebHTML
            string HTMLContent = VistaProcessor.ProcessXML(xmlContent);

            // Create view config
            GUIConfig config = new GUIConfig();
            config.IsTransparent = isTransparent;
            config.EnableJavaScript = true;

            GUIView vistaView = new GUIView(HTMLContent, config, 12, height, width);

            container = new GUIContainer();

            // Create a GUI View
            container.VistaView = vistaView;

            // Set up GUI Events
            RegisterEvent("PrintMessage", () => SetElementInnerHTML("output-text", "Button Was Pressed!"));

            RenderViewToObject();
        }

        bool isVisible = true;
        public override void Update(float deltaTime)
        {
            // Get current mouse position
            var mousePosition = Input.GetMousePosition();

            container.VistaView.SendMousePosition(mousePosition);

            // Send Mouse Inputs
            if (Input.GetMouseButtonDown(0))
            {
                container.VistaView.SendMouseButtonDown(0);
            }

            if (Input.GetMouseButtonUp(0))
            {
                container.VistaView.SendMouseButtonUp(0);
            }

            // Toggle GUI Visibility
            if (Input.GetKeyDown(Key.Escape) && isVisible)
            {
                container.VistaView.Visible = false;
                isVisible = false;
            } 
            else if (Input.GetKeyDown(Key.Escape) && !isVisible)
            {
                container.VistaView.Visible = true;
                isVisible = true;
            }

            // Render view
            Texture texture = container.RenderToTexture();

            // Update View
            if (guiMesh != null)
                guiMesh.Material.Texture = texture;
        }

        /// <summary>
        /// Registers a C# Action that can be called from JavaScript.
        /// </summary>
        /// <param name="name">Javascript function name</param>
        /// <param name="action">C# Action</param>
        public void RegisterEvent(string name, Action action)
        {
            container.VistaView.ScriptingContext.RegisterEvent(name, action);
        }

        /// <summary>
        /// Sets the Inner HTML of an Element from it's ID.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="text"></param>
        public void SetElementInnerHTML(string id, string text)
        {
            container.VistaView.ScriptingContext.SetElementInnerHTML(id, text);
        }

        // Render a GUIView in worldspace
        private void RenderViewToObject()
        {
            // Create a Material from the view output
            Material guiMaterial = new Material("dev/missing");
            guiMaterial.Texture = container.Output;

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
