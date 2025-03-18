using Silk.NET.Maths;
using SourceRewrite.Components;
using SourceRewrite.Objects;
using SourceRewrite.Rendering.OpenGL;
using SourceRewrite.Windowing;
using SourceRewrite.AssetTypes;
using SourceRewrite.Maps;


namespace SourceRewrite.Rendering
{
    /// <summary>
    /// Renderer Interaction Class.
    /// </summary>
    public class RendererContext
    {
        /// <summary>
        /// The renderer's API. (e.g., OpenGL, Direct3D, Vulkan).
        /// </summary>
        /// <remarks>
        /// This is an enumeration of type <see cref="RendererAPI"/>.  
        /// Possible values: <see cref="RendererAPI.OpenGL"/>.
        /// </remarks>
        public RendererAPI API { get; private set; }

        private IRendererAPI _apiInterface; // Use an interface for better abstraction
        public RendererContext(RendererAPI chosenRenderer, GameWindow targetWindow)
        {
            API = chosenRenderer; // Pass chosen renderer to our API variable

            // Create a renderer context based on chosen renderer
            switch (chosenRenderer)
            {
                case RendererAPI.OpenGL:
                    _apiInterface = new OpenGLContext(targetWindow);
                    break;
            }
        }

        /// <summary>
        /// Gets the Renderers low-level API. (e.g, OpenGL, Direct3D, Vulkan)
        /// </summary>
        public IRendererAPI GetRendererAPI() => _apiInterface;

        /// <summary>
        /// Sets the clear (skybox) color.
        /// </summary>
        /// <param name="r">Red component (0-255).</param>
        /// <param name="g">Green component (0-255).</param>
        /// <param name="b">Blue component (0-255).</param>
        /// <param name="a">Alpha (opacity) component (0-255).</param>
        public void SetClearColour(int r, int g, int b, int a)
        {
            _apiInterface.SetClearColour(r, g, b, a);
        }

        // Run any special logic that needs to be ran on load
        public void OnLoad()
        {
            _apiInterface.OnLoad(this);
        }

        // Run any special logic that needs to be ran per frame
        public void OnRender()
        {
            _apiInterface.OnRender(this);

            // Recursively render all GameObjects starting from Root
            RenderGameObjects(GameObjectManager.Root);
        }

        private void RenderGameObjects(GameObject root)
        {
            // Render the current GameObject and its components
            RenderComponents(root);

            // Recursively render all children
            foreach (var child in root.Children)
            {
                RenderGameObjects(child);
            }
        }

        private void RenderComponents(GameObject gameObject)
        {
            // Render all Meshes
            MeshRenderer meshRenderer = gameObject.GetComponentFromType<MeshRenderer>();
            if (meshRenderer != null)
            {
                _apiInterface.RenderMesh(meshRenderer);
            }

            // Render all visible Screenspace GUIs
            GUICanvas guiCanvas = gameObject.GetComponentFromType<GUICanvas>();
            if (guiCanvas != null && guiCanvas.PanelType != 0 && guiCanvas.Container.VistaView.Visible == true)
            {
                _apiInterface.RenderScreenspaceGUI(guiCanvas);
            }
        }

        // Run any special cleanup logic that needs to be when the app is closed
        public void OnClose()
        {
            _apiInterface.OnClose();
        }

        public void OnFramebufferResize(Vector2D<int> newSize)
        {
            // Resize all visible Screenspace GUIs recursively starting from the root
            ResizeGUIs(GameObjectManager.Root, newSize);

            _apiInterface.OnFramebufferResize(newSize);
        }

        private void ResizeGUIs(GameObject root, Vector2D<int> newSize)
        {
            // Resize the current GameObject's GUI if applicable
            ResizeGUIComponent(root, newSize);

            // Recursively resize all children's GUIs
            foreach (var child in root.Children)
            {
                ResizeGUIs(child, newSize);
            }
        }

        private void ResizeGUIComponent(GameObject gameObject, Vector2D<int> newSize)
        {
            GUICanvas guiCanvas = gameObject.GetComponentFromType<GUICanvas>();
            if (guiCanvas != null && guiCanvas.PanelType != 0 && guiCanvas.Container.VistaView.Visible == true)
            {
                guiCanvas.Container.VistaView.Resize((uint)newSize.X, (uint)newSize.Y);
            }
        }

        public void InitMesh(MeshAsset meshObject)
        {
            _apiInterface.InitMesh(meshObject);
        }

        public void DrawScreenspaceQuad(GUICanvas canvas)
        {
            _apiInterface.RenderScreenspaceGUI(canvas);
        }
    }

    /// <summary>
    /// Renderer API Interface that allows support for multiple Renderer APIs.
    /// </summary>
    public interface IRendererAPI
    {
        void SetClearColour(int r, int g, int b, int a);
        void OnRender(RendererContext renderer);
        void OnLoad(RendererContext renderer);
        void OnClose();
        void OnFramebufferResize(Vector2D<int> newSize);
        void RenderMesh(MeshRenderer meshObject);
        void InitMesh(MeshAsset meshObject);
        void RenderScreenspaceGUI(GUICanvas canvas);
    }

    /// <summary>
    /// Enumeration of supported Renderer APIs.
    /// </summary>
    public enum RendererAPI
    {
        OpenGL
    }
}
