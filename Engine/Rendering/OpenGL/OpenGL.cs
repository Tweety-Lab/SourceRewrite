using Silk.NET.OpenGL;
using SourceRewrite.Windowing;
using System.Drawing;
using Silk.NET.Maths;
using SourceRewrite.Maths;
using System.Numerics;
using SourceRewrite.Objects;
using SourceRewrite.Components;
using System.Threading.Tasks.Dataflow;
using Silk.NET.Windowing;
using System.Reflection;

namespace SourceRewrite.Rendering.OpenGL
{
    // Disgusting horrid piece of code but luckily the render system is modular enough it doesnt matter right now
    public class OpenGLContext : IRendererAPI
    {
        // Store all lists
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
            OpenGL.Clear((uint)ClearBufferMask.ColorBufferBit);

        }

        public void OnFramebufferResize(Vector2D<int> newSize)
        {
            OpenGL.Viewport(newSize);
        }

        public unsafe void RenderMesh(MeshRenderer meshObject)
        {
            OpenGLShader openglShader = (OpenGLShader)meshObject.shader.GetShaderInterface();
            OpenGLTexture openglTexture = (OpenGLTexture)meshObject.texture.GetTextureInterface();

            openglShader.Use();
            openglTexture.Bind(TextureUnit.Texture0);


            // Projection matrix
            var projection = Matrix4x4.CreatePerspectiveFieldOfView(
                MathF.PI / 4, // FOV
                GameWindow.CurrentWindow.GetSilkWindow().Size.X / GameWindow.CurrentWindow.GetSilkWindow().Size.Y, // Aspect ratio
                0.1f, 100f // Near and far planes
            );

            // View matrix from active camera
            var view = Camera.ActiveCamera.GetViewMatrix();

            openglShader.SetUniform("uModel", meshObject.GameObject.GetComponentFromType<Transform>().ViewMatrix);
            openglShader.SetUniform("uView", view);
            openglShader.SetUniform("uProjection", projection);

            foreach (OpenGLVertexArrayObject<float, uint> vao in vaoList)
            {
                // Binding and using our VAO and shader.
                vao.Bind();
                OpenGL.DrawElements(PrimitiveType.Triangles, (uint)meshObject.Indices.Length, DrawElementsType.UnsignedInt, null);
            }
        }

        public unsafe void InitMesh(MeshRenderer meshObject)
        {
            // Instantiating our new abstractions
            eboList.Add(new OpenGLBufferObject<uint>(OpenGL, meshObject.Indices, BufferTargetARB.ElementArrayBuffer));
            vboList.Add(new OpenGLBufferObject<float>(OpenGL, meshObject.Vertices, BufferTargetARB.ArrayBuffer));
            vaoList.Add(new OpenGLVertexArrayObject<float, uint>(OpenGL, vboList[vboList.Count - 1], eboList[eboList.Count - 1]));

            //Telling the VAO object how to lay out the attribute pointers
            vaoList[vaoList.Count - 1].VertexAttributePointer(0, 3, VertexAttribPointerType.Float, 5, 0);
            vaoList[vaoList.Count - 1].VertexAttributePointer(1, 2, VertexAttribPointerType.Float, 5, 3);
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
