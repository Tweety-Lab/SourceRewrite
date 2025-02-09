using Silk.NET.OpenGL;
using SourceRewrite.Windowing;
using System.Drawing;
using Silk.NET.Maths;
using SourceRewrite.Maths;
using System.Numerics;
using SourceRewrite.Objects;

namespace SourceRewrite.Rendering.OpenGL
{
    // Disgusting horrid piece of code but luckily the render system is modular enough it doesnt matter right now
    public class OpenGLContext : IRendererAPI
    {

        private static OpenGLBufferObject<float> Vbo;
        private static OpenGLBufferObject<uint> Ebo;
        private static OpenGLVertexArrayObject<float, uint> Vao;

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
            // Create a mesh for testing
            Mesh testMesh = new Mesh(new Texture("../../../Assets/bricks.jpg"), new Shader("../../../Assets/shader.vert", "../../../Assets/shader.frag"));

            foreach (GameObject gameObject in GameObject.ActiveObjects)
            {
                if (gameObject.GetType() == typeof(Mesh))
                {
                    Mesh meshobject = (Mesh)gameObject;
                    // Instantiating our new abstractions
                    Ebo = new OpenGLBufferObject<uint>(OpenGL, meshobject.Indices, BufferTargetARB.ElementArrayBuffer);
                    Vbo = new OpenGLBufferObject<float>(OpenGL, meshobject.Vertices, BufferTargetARB.ArrayBuffer);
                    Vao = new OpenGLVertexArrayObject<float, uint>(OpenGL, Vbo, Ebo);

                    //Telling the VAO object how to lay out the attribute pointers
                    Vao.VertexAttributePointer(0, 3, VertexAttribPointerType.Float, 5, 0);
                    Vao.VertexAttributePointer(1, 2, VertexAttribPointerType.Float, 5, 3);
                }
            }
        }

        public unsafe void OnRender(RendererContext renderer)
        {
            OpenGL.Clear((uint)ClearBufferMask.ColorBufferBit);

            //Binding and using our VAO and shader.
            Vao.Bind();
        }

        public void OnFramebufferResize(Vector2D<int> newSize)
        {
            OpenGL.Viewport(newSize);
        }

        public unsafe void RenderMesh(Mesh meshObject)
        {
            OpenGLShader openglShader = (OpenGLShader)meshObject.shader.GetShaderInterface();
            OpenGLTexture openglTexture = (OpenGLTexture)meshObject.texture.GetTextureInterface();

            openglShader.Use();
            openglTexture.Bind(TextureUnit.Texture0);

            openglShader.SetUniform("uModel", meshObject.Transform.ViewMatrix);

            OpenGL.DrawElements(PrimitiveType.Triangles, (uint)meshObject.Indices.Length, DrawElementsType.UnsignedInt, null);
        }
        
        public void OnClose()
        {
            Vbo.Dispose();
            Ebo.Dispose();
            Vao.Dispose();
        }
    }
}
