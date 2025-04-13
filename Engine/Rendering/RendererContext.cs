using Silk.NET.Maths;
using SourceRewrite.Entities;
using SourceRewrite.Rendering.OpenGL;
using SourceRewrite.Windowing;
using SourceRewrite.AssetTypes;
using SourceRewrite.Entities.GUI;
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

        // Mapping of renderers to render contexts
        private Dictionary<RendererAPI, Type> RenderMap = new Dictionary<RendererAPI, Type>
        {
            { RendererAPI.OpenGL, typeof(OpenGLContext) }
        };

        private readonly IRendererAPI _apiInterface; // Use an interface for better abstraction
        public RendererContext(RendererAPI chosenRenderer, GameWindow targetWindow)
        {
            API = chosenRenderer; // Pass chosen renderer to our API variable

            // Get the Chosen Renderer Context
            RenderMap.TryGetValue(chosenRenderer, out Type rendererType);

            // Create the Renderer Context
            if (rendererType != null)
            {
                _apiInterface = (IRendererAPI)Activator.CreateInstance(rendererType, new object[] { targetWindow });
            } else
            {
                Console.WriteLine("Renderer API type not found.");
            }
                
        }

        /// <summary>
        /// Calculate an entity's ModelMatrix (position, rotation, scale).
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static Matrix4x4? GetEntityModelMatrix(PointEntity entity)
        {
            Matrix4x4 transformation = Matrix4x4.CreateScale(entity.Transform.Scale) *
                                       Matrix4x4.CreateFromQuaternion(entity.Transform.Rotation) *
                                       Matrix4x4.CreateTranslation(entity.Transform.Position);

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
        public void SetClearColour(int r, int g, int b, int a) => _apiInterface.SetClearColour(r, g, b, a);

        // Run any special logic that needs to be ran on load
        public void OnLoad() => _apiInterface.OnLoad(this);

        // Run any special logic that needs to be ran per frame
        public void OnRender()
        {
            _apiInterface.OnRender(this);
            RenderPassManager.RenderAllPasses();
        }



        // Run any special cleanup logic that needs to be when the app is closed
        public void OnClose() => _apiInterface.OnClose();

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
                ResizeGUIEntity((GUICanvasEntity)root, newSize);
            }

            // Recursively resize all children's GUIs
            foreach (var child in root.Children)
            {
                ResizeGUIs(child, newSize);
            }
        }

        private void ResizeGUIEntity(GUICanvasEntity entity, Vector2D<int> newSize)
        {
            if (entity.PanelType != 0 && entity.Container.VistaView.Visible == true)
            {
                entity.Container.VistaView.Resize((uint)newSize.X, (uint)newSize.Y);
            }
        }

        public void InitMesh(Mesh meshObject)
        {
            // If no Normals exist, calculate them
            if (meshObject.Normals == null || meshObject.Normals.Length == 0)
                meshObject.CalculateNormals();

            // If no UVs exist, default to fixed UVs for each face
            if (meshObject.UVs == null || meshObject.UVs.Length == 0)
            {
                // Initialize UV array with the same length as the vertices
                meshObject.UVs = new float[meshObject.Vertices.Length / 3 * 2];

                // Apply fixed UVs for each triangle
                for (int i = 0; i < meshObject.Indices.Length; i += 3) // Process each triangle (3 indices)
                {
                    // Default UVs for a triangle
                    Vector2 uv1 = new Vector2(0, 0);
                    Vector2 uv2 = new Vector2(1, 0);
                    Vector2 uv3 = new Vector2(0, 1);

                    // Get the vertex indices for this triangle
                    uint idx1 = meshObject.Indices[i] * 2;
                    uint idx2 = meshObject.Indices[i + 1] * 2;
                    uint idx3 = meshObject.Indices[i + 2] * 2;

                    // Apply UVs to each vertex in the triangle
                    meshObject.UVs[idx1] = uv1.X;
                    meshObject.UVs[idx1 + 1] = uv1.Y;

                    meshObject.UVs[idx2] = uv2.X;
                    meshObject.UVs[idx2 + 1] = uv2.Y;

                    meshObject.UVs[idx3] = uv3.X;
                    meshObject.UVs[idx3 + 1] = uv3.Y;
                }
            }


            _apiInterface.InitMesh(meshObject);
        }

        public void RenderMesh(Mesh meshObject, Matrix4x4 modelMatrix) => _apiInterface.RenderMesh(meshObject, modelMatrix);

        public void DrawScreenspaceQuad(GUICanvasEntity canvas) => _apiInterface.RenderScreenspaceGUI(canvas);
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
        void RenderMesh(Mesh meshObject, Matrix4x4 modelMatrix);
        void InitMesh(Mesh meshObject);
        void RenderScreenspaceGUI(GUICanvasEntity canvas);

        // Flags
        void EnableFlag(RenderFlag renderFlag);
        void DisableFlag(RenderFlag renderFlag);
    }

    /// <summary>
    /// Supported Renderer APIs.
    /// </summary>
    public enum RendererAPI
    {
        OpenGL
    }
}
