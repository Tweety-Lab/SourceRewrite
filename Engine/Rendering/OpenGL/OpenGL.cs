using Silk.NET.OpenGL;
using SourceRewrite.Windowing;
using System.Drawing;
using Silk.NET.Maths;
using System.Numerics;
using SourceRewrite.AssetTypes;
using SourceRewrite.Files;
using SourceRewrite.Entities.GUI;
using SourceRewrite.Entities.Point;

namespace SourceRewrite.Rendering.OpenGL
{
    // Disgusting horrid piece of code but luckily the render system is modular enough it doesnt matter right now
    public class OpenGLContext : IRendererAPI
    {
        // Add dictionary to map meshes to their buffer indices
        private Dictionary<Mesh, (int VaoIndex, int VboIndex, int NboIndex, int UboIndex, int EboIndex)> meshBufferMap =
            new Dictionary<Mesh, (int VaoIndex, int VboIndex, int NboIndex, int UboIndex, int EboIndex)>();

        private List<OpenGLBufferObject<uint>> eboList = new List<OpenGLBufferObject<uint>>();
        private List<OpenGLBufferObject<float>> vboList = new List<OpenGLBufferObject<float>>();
        private List<OpenGLBufferObject<float>> nboList = new List<OpenGLBufferObject<float>>();
        private List<OpenGLBufferObject<float>> uboList = new List<OpenGLBufferObject<float>>();
        private List<OpenGLVertexArrayObject<float, uint>> vaoList = new List<OpenGLVertexArrayObject<float, uint>>();

        private OpenGLBufferObject<float> _quadVbo;
        private OpenGLBufferObject<float> _quadUbo;
        private OpenGLBufferObject<uint> _quadEbo;
        private OpenGLVertexArrayObject<float, uint> _quadVao;

        // GUI Screenspace Shader
        private AssetTypes.Shader _guiShader;

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

        // Define logic for enabling and disabling render flags
        private readonly Dictionary<RenderFlag, Action<GL>> _renderFlagEnableMap = new()
{
    { RenderFlag.DepthTest, gl => gl.Enable(EnableCap.DepthTest) },
    { RenderFlag.Blend, gl => {
        gl.Enable(EnableCap.Blend);
        gl.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
    }}
};

        private readonly Dictionary<RenderFlag, Action<GL>> _renderFlagDisableMap = new()
{
    { RenderFlag.DepthTest, gl => gl.Disable(EnableCap.DepthTest) },
    { RenderFlag.Blend, gl => gl.Disable(EnableCap.Blend) }
};

        public void EnableFlag(RenderFlag renderFlag)
        {
            if (_renderFlagEnableMap.TryGetValue(renderFlag, out var enableAction))
            {
                enableAction(OpenGL);
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(renderFlag), $"Unsupported render flag: {renderFlag}");
            }
        }

        public void DisableFlag(RenderFlag renderFlag)
        {
            if (_renderFlagDisableMap.TryGetValue(renderFlag, out var disableAction))
            {
                disableAction(OpenGL);
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(renderFlag), $"Unsupported render flag: {renderFlag}");
            }
        }

        public unsafe void OnLoad(RendererContext renderer)
        {
            // Define the vertices for a full-screen quad in NDC (Normalized Device Coordinates)
            float[] quadVertices = {
                -1.0f,  1.0f,
                -1.0f, -1.0f,
                 1.0f, -1.0f,
                 1.0f,  1.0f
            };

            // Define UVs for the quad (flipped vertically)
            float[] quadUVs = {
                0.0f, 0.0f,
                0.0f, 1.0f,
                1.0f, 1.0f,
                1.0f, 0.0f
            };

            // Define the indices for the quad (two triangles)
            uint[] quadIndices = {
                0, 1, 2,
                0, 2, 3
            };

            // Create and bind the VBO for positions
            _quadVbo = new OpenGLBufferObject<float>(OpenGL, quadVertices, BufferTargetARB.ArrayBuffer);

            // Create and bind the VBO for UVs
            _quadUbo = new OpenGLBufferObject<float>(OpenGL, quadUVs, BufferTargetARB.ArrayBuffer);

            // Create and bind the EBO
            _quadEbo = new OpenGLBufferObject<uint>(OpenGL, quadIndices, BufferTargetARB.ElementArrayBuffer);

            // Create and bind the VAO
            _quadVao = new OpenGLVertexArrayObject<float, uint>(OpenGL, _quadVbo, _quadEbo);

            // Important: We need to explicitly set up the vertex attributes for the quad VAO
            _quadVao.Bind();

            // Set up the vertex attribute pointers
            // Bind position buffer and set attribute
            _quadVbo.Bind();
            OpenGL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 2 * sizeof(float), null);
            OpenGL.EnableVertexAttribArray(0);

            // Bind UV buffer and set attribute
            _quadUbo.Bind();
            OpenGL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 2 * sizeof(float), null);
            OpenGL.EnableVertexAttribArray(1);

            // Make sure to unbind the VAO
            _quadVao.Unbind();

            // Load UI shader if it hasn't been loaded yet
            if (_guiShader == null)
            {
                _guiShader = new AssetTypes.Shader(FileSystem.GetShaderPath("ScreenspaceGUI"));
            }
        }

        public unsafe void OnRender(RendererContext renderer)
        {
            OpenGL.Clear((uint)(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit));
        }

        public void OnFramebufferResize(Vector2D<int> newSize)
        {
            OpenGL.Viewport(newSize);
        }

        public unsafe void RenderMesh(Mesh meshObject, Matrix4x4 modelMatrix)
        {
            if (meshObject == null)
                throw new ArgumentNullException(nameof(meshObject));

            if (!meshBufferMap.ContainsKey(meshObject))
                throw new InvalidOperationException("Mesh has not been initialized.");

            // Reset OpenGL state before rendering a mesh
            OpenGL.BindVertexArray(0);

            // Get the shader for the mesh material
            OpenGLShader openglShader = (OpenGLShader)meshObject.Material.Shader?.GetShaderInterface();
            openglShader.Use();

            // Bind the textures if they exist
            if (meshObject.Material.Textures?.Any(texture => texture != null) == true)
            {
                // Check if texture exists before binding
                OpenGLTexture openglTexture = (OpenGLTexture)meshObject.Material.Textures[0]?.GetTextureInterface();
                if (openglTexture != null)
                {
                    openglTexture.Bind(TextureUnit.Texture0);
                }
            }

            // Projection matrix
            var windowSize = GameWindow.CurrentWindow.GetSilkWindow().Size;
            float aspectRatio = windowSize.X / (float)windowSize.Y;
            if (aspectRatio <= 0)
                aspectRatio = 1.0f; // Fallback to avoid division by zero

            var projection = PointCamera.ActiveCamera?.GetProjectionMatrix();

            // View matrix from active camera
            var view = PointCamera.ActiveCamera?.GetViewMatrix() ?? Matrix4x4.Identity;

            // Use the provided model matrix
            Matrix4x4 model = modelMatrix;

            openglShader.SetParameter("MODEL_MATRIX", model);
            openglShader.SetParameter("VIEW_MATRIX", view);
            openglShader.SetParameter("PROJECTION_MATRIX", projection);

            // Get the VAO for this specific mesh
            var bufferIndices = meshBufferMap[meshObject];
            var vao = vaoList[bufferIndices.VaoIndex];

            // Bind the VAO and render
            vao.Bind();
            OpenGL.DrawElements(PrimitiveType.Triangles, (uint)meshObject.Indices.Length, DrawElementsType.UnsignedInt, null);
            vao.Unbind();

            // Reset state after rendering
            OpenGL.BindTexture(TextureTarget.Texture2D, 0);
        }

        public unsafe void InitMesh(Mesh meshObject)
        {
            // Ensure we don't have any active VAO
            OpenGL.BindVertexArray(0);

            // Create buffers for vertices, normals, UVs, and indices
            vboList.Add(new OpenGLBufferObject<float>(OpenGL, meshObject.Vertices, BufferTargetARB.ArrayBuffer));
            nboList.Add(new OpenGLBufferObject<float>(OpenGL, meshObject.Normals, BufferTargetARB.ArrayBuffer));
            uboList.Add(new OpenGLBufferObject<float>(OpenGL, meshObject.UVs, BufferTargetARB.ArrayBuffer));
            eboList.Add(new OpenGLBufferObject<uint>(OpenGL, meshObject.Indices, BufferTargetARB.ElementArrayBuffer));

            // Create VAO
            vaoList.Add(new OpenGLVertexArrayObject<float, uint>(OpenGL, vboList[vboList.Count - 1], eboList[eboList.Count - 1]));

            // Store the mapping
            meshBufferMap[meshObject] = (
                VaoIndex: vaoList.Count - 1,
                VboIndex: vboList.Count - 1,
                NboIndex: nboList.Count - 1,
                UboIndex: uboList.Count - 1,
                EboIndex: eboList.Count - 1
            );

            var vao = vaoList[vaoList.Count - 1];

            // Bind VAO
            vao.Bind();

            // Set up position attribute (attribute 0)
            vboList[vboList.Count - 1].Bind();
            OpenGL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), null);
            OpenGL.EnableVertexAttribArray(0);

            // Set up normal attribute (attribute 1)
            nboList[nboList.Count - 1].Bind();
            OpenGL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), null);
            OpenGL.EnableVertexAttribArray(1);

            // Set up UV attribute (attribute 2)
            uboList[uboList.Count - 1].Bind();
            OpenGL.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, 2 * sizeof(float), null);
            OpenGL.EnableVertexAttribArray(2);

            // Bind element buffer
            eboList[eboList.Count - 1].Bind();

            // Unbind VAO
            vao.Unbind();

            // Reset state
            OpenGL.BindBuffer(BufferTargetARB.ArrayBuffer, 0);
            OpenGL.BindBuffer(BufferTargetARB.ElementArrayBuffer, 0);
        }

        public unsafe void RenderScreenspaceGUI(GUICanvasEntity canvas)
        {
            // Completely reset OpenGL state before GUI rendering
            OpenGL.BindVertexArray(0);
            OpenGL.BindBuffer(BufferTargetARB.ArrayBuffer, 0);
            OpenGL.BindBuffer(BufferTargetARB.ElementArrayBuffer, 0);

            // Enable blending for UI elements
            OpenGL.Enable(EnableCap.Blend);
            OpenGL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            // Use the UI-specific shader
            OpenGLShader guiOpenGLShader = (OpenGLShader)_guiShader.GetShaderInterface();
            guiOpenGLShader.Use();

            // Set tint color uniform if needed (assuming a white tint by default)
            guiOpenGLShader.SetParameter("tint", new Vector4(255, 255, 255, 255));

            // Bind the texture
            OpenGLTexture openglTexture = (OpenGLTexture)canvas.Container.RenderToTexture().GetTextureInterface();
            openglTexture.Bind(TextureUnit.Texture0);

            // Set texture uniform
            guiOpenGLShader.SetParameter("uTexture0", 0); // Texture unit 0

            // Bind the VAO and draw the quad
            _quadVao.Bind();

            // Double-check our attribute settings
            _quadVbo.Bind();
            OpenGL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 2 * sizeof(float), null);
            OpenGL.EnableVertexAttribArray(0);

            _quadUbo.Bind();
            OpenGL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 2 * sizeof(float), null);
            OpenGL.EnableVertexAttribArray(1);

            _quadEbo.Bind();

            // Draw the quad with explicit indices count
            OpenGL.DrawElements(PrimitiveType.Triangles, 6, DrawElementsType.UnsignedInt, null);

            // Clean up state
            _quadVao.Unbind();
            OpenGL.BindTexture(TextureTarget.Texture2D, 0);
        }

        public void OnClose()
        {
            _quadVbo?.Dispose();
            _quadUbo?.Dispose();
            _quadEbo?.Dispose();
            _quadVao?.Dispose();

            foreach (OpenGLBufferObject<uint> ebo in eboList)
            {
                ebo.Dispose();
            }

            foreach (OpenGLBufferObject<float> vbo in vboList)
            {
                vbo.Dispose();
            }

            foreach (OpenGLBufferObject<float> nbo in nboList)
            {
                nbo.Dispose();
            }

            foreach (OpenGLBufferObject<float> ubo in uboList)
            {
                ubo.Dispose();
            }

            foreach (OpenGLVertexArrayObject<float, uint> vao in vaoList)
            {
                vao.Dispose();
            }
        }
    }
}
