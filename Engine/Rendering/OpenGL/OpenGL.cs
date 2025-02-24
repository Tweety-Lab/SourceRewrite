using Silk.NET.OpenGL;
using SourceRewrite.Windowing;
using System.Drawing;
using Silk.NET.Maths;
using SourceRewrite.Maths;
using System.Numerics;
using SourceRewrite.Objects;
using SourceRewrite.Components;
using SourceRewrite.AssetTypes;
using System.Threading.Tasks.Dataflow;
using Silk.NET.Windowing;
using System.Reflection;
using Silk.NET.Vulkan;

namespace SourceRewrite.Rendering.OpenGL
{
    // Disgusting horrid piece of code but luckily the render system is modular enough it doesnt matter right now
    public class OpenGLContext : IRendererAPI
    {
        // Add dictionary to map meshes to their buffer indices
        private Dictionary<MeshAsset, (int VaoIndex, int VboIndex, int EboIndex)> meshBufferMap =
            new Dictionary<MeshAsset, (int VaoIndex, int VboIndex, int EboIndex)>();

        private List<OpenGLBufferObject<uint>> eboList = new List<OpenGLBufferObject<uint>>();
        private List<OpenGLBufferObject<float>> vboList = new List<OpenGLBufferObject<float>>();
        private List<OpenGLVertexArrayObject<float, uint>> vaoList = new List<OpenGLVertexArrayObject<float, uint>>();

        public GL OpenGL;
        public OpenGLContext(GameWindow targetWindow)
        {
            OpenGL = targetWindow.GetSilkWindow().CreateOpenGL();
            if (OpenGL == null)
            {
                throw new InvalidOperationException("Failed to create OpenGL context.");
            }
        }

        public void SetClearColour(int r, int g, int b, int a)
        {
            OpenGL.ClearColor(Color.FromArgb(a, r, g, b));
        }

        public unsafe void OnLoad(RendererContext renderer)
        {
        }

        public unsafe void OnRender(RendererContext renderer)
        {
            OpenGL.Clear((uint)(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit));
            OpenGL.Enable(EnableCap.DepthTest);
        }

        public void OnFramebufferResize(Vector2D<int> newSize)
        {
            OpenGL.Viewport(newSize);
        }

        public unsafe void RenderMesh(MeshRenderer meshObject)
        {
            if (meshObject == null || meshObject.Mesh == null)
                throw new ArgumentNullException(nameof(meshObject));

            if (!meshBufferMap.ContainsKey(meshObject.Mesh))
                throw new InvalidOperationException("Mesh has not been initialized.");

            OpenGLShader openglShader = (OpenGLShader)meshObject.Mesh.Material.Shader?.GetShaderInterface();
            OpenGLTexture openglTexture = (OpenGLTexture)meshObject.Mesh.Material.Texture?.GetTextureInterface();

            if (openglShader == null || openglTexture == null)
                throw new InvalidOperationException("Shader or Texture is not valid.");

            openglShader.Use();
            openglTexture.Bind(TextureUnit.Texture0);

            // Projection matrix
            var windowSize = GameWindow.CurrentWindow.GetSilkWindow().Size;
            float aspectRatio = windowSize.X / (float)windowSize.Y;
            if (aspectRatio <= 0)
                aspectRatio = 1.0f; // Fallback to avoid division by zero

            var projection = Matrix4x4.CreatePerspectiveFieldOfView(
                MathF.PI / 4, // FOV
                aspectRatio, // Aspect ratio
                0.1f, 100f // Near and far planes
            );

            // View matrix from active camera
            var view = Camera.ActiveCamera?.GetViewMatrix() ?? Matrix4x4.Identity;

            // Model matrix from transform
            var model = meshObject.GameObject.GetComponentFromType<Transform>()?.ViewMatrix ?? Matrix4x4.Identity;

            openglShader.SetParameter("uModel", model);
            openglShader.SetParameter("uView", view);
            openglShader.SetParameter("uProjection", projection);

            // Get the VAO for this specific mesh
            var bufferIndices = meshBufferMap[meshObject.Mesh];
            var vao = vaoList[bufferIndices.VaoIndex];

            vao.Bind();
            OpenGL.DrawElements(PrimitiveType.Triangles, (uint)meshObject.Mesh.Indices.Length, DrawElementsType.UnsignedInt, null);
            vao.Unbind();
        }

        public unsafe void InitMesh(MeshAsset meshObject)
        {
            eboList.Add(new OpenGLBufferObject<uint>(OpenGL, meshObject.Indices, BufferTargetARB.ElementArrayBuffer));
            vboList.Add(new OpenGLBufferObject<float>(OpenGL, meshObject.Vertices, BufferTargetARB.ArrayBuffer));
            vaoList.Add(new OpenGLVertexArrayObject<float, uint>(OpenGL, vboList[vboList.Count - 1], eboList[eboList.Count - 1]));

            // Store the mapping
            meshBufferMap[meshObject] = (
                VaoIndex: vaoList.Count - 1,
                VboIndex: vboList.Count - 1,
                EboIndex: eboList.Count - 1
            );

            var vao = vaoList[vaoList.Count - 1];
            vao.VertexAttributePointer(0, 3, VertexAttribPointerType.Float, 5, 0);
            vao.VertexAttributePointer(1, 2, VertexAttribPointerType.Float, 5, 3);
        }

        public void OnClose()
        {

            foreach (OpenGLBufferObject<uint> ebo in eboList)
            {
                ebo.Dispose();
            }

            foreach (OpenGLBufferObject<float> vbo in vboList)
            {
                vbo.Dispose();
            }

            foreach (OpenGLVertexArrayObject<float, uint> vao in vaoList)
            {
                vao.Dispose();
            }
        }
    }
}
