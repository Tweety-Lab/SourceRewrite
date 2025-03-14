using Silk.NET.Maths;
using SourceRewrite.Components;
using SourceRewrite.Objects;
using SourceRewrite.Rendering.OpenGL;
using SourceRewrite.Windowing;
using SourceRewrite.AssetTypes;


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

            // Render every Mesh Renderer
            foreach (GameObject gameobject in GameObject.ActiveObjects)
            {
                MeshRenderer meshRenderer = gameobject.GetComponentFromType<MeshRenderer>();
                if (meshRenderer != null)
                {
                    _apiInterface.RenderMesh(meshRenderer);
                }

                GUICanvas guiCanvas = gameobject.GetComponentFromType<GUICanvas>();
                if (guiCanvas != null)
                {
                    _apiInterface.RenderScreenspaceGUI(guiCanvas);
                }
            }
        }


        // Run any special cleanup logic that needs to be when the app is closed
        public void OnClose()
        {
            _apiInterface.OnClose();
        }

        // Run any special logic that needs to be ran per frame
        public void OnFramebufferResize(Vector2D<int> newSize)
        {
            _apiInterface.OnFramebufferResize(newSize);
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
