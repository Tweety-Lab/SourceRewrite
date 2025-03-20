using Silk.NET.Maths;
using SourceRewrite.Entities;
using SourceRewrite.Rendering.OpenGL;
using SourceRewrite.Windowing;
using SourceRewrite.AssetTypes;
using SourceRewrite.Entities.GUI;
using Silk.NET.Input;
using System.Numerics;


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
        /// Calculate an entities ViewMatrix
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static Matrix4x4? GetEntityViewMatrix(BaseEntity entity)
        {
            Matrix4x4 transformation = Matrix4x4.CreateTranslation(entity.Transform.Position) *
                           Matrix4x4.CreateFromQuaternion(entity.Transform.Rotation) *
                           Matrix4x4.CreateScale(entity.Transform.Scale);

            return transformation;
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

            // Get all Passes
            List<IRenderPass> passes = RenderPassManager.GetRenderPasses();

            // Render all Passes
            foreach (IRenderPass pass in passes)
                pass.OnRender();
        }

        // Run any special cleanup logic that needs to be when the app is closed
        public void OnClose()
        {
            _apiInterface.OnClose();
        }

        public void OnFramebufferResize(Vector2D<int> newSize)
        {
            // Resize all visible Screenspace GUIs recursively starting from the root
            ResizeGUIs(EntityManager.Root, newSize);

            _apiInterface.OnFramebufferResize(newSize);
        }

        private void ResizeGUIs(BaseEntity root, Vector2D<int> newSize)
        {
            // Resize the Entities GUI if applicable
            if (root is GUICanvasEntity)
            {
                ResizeGUIComponent((GUICanvasEntity)root, newSize);
            }

            // Recursively resize all children's GUIs
            foreach (var child in root.Children)
            {
                ResizeGUIs(child, newSize);
            }
        }

        private void ResizeGUIComponent(GUICanvasEntity entity, Vector2D<int> newSize)
        {
            if (entity.PanelType != 0 && entity.Container.VistaView.Visible == true)
            {
                entity.Container.VistaView.Resize((uint)newSize.X, (uint)newSize.Y);
            }
        }

        public void InitMesh(MeshAsset meshObject)
        {
            _apiInterface.InitMesh(meshObject);
        }

        public void DrawScreenspaceQuad(GUICanvasEntity canvas)
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
        void RenderMesh(MeshEntity meshObject);
        void InitMesh(MeshAsset meshObject);
        void RenderScreenspaceGUI(GUICanvasEntity canvas);
    }

    /// <summary>
    /// Enumeration of supported Renderer APIs.
    /// </summary>
    public enum RendererAPI
    {
        OpenGL
    }
}
