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

        private static Transform ItemTransform = new Transform();

        private Mesh test_mesh;

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
            test_mesh = new Mesh();

            // Instantiating our new abstractions
            Ebo = new OpenGLBufferObject<uint>(OpenGL, test_mesh.Indices, BufferTargetARB.ElementArrayBuffer);
            Vbo = new OpenGLBufferObject<float>(OpenGL, test_mesh.Vertices, BufferTargetARB.ArrayBuffer);
            Vao = new OpenGLVertexArrayObject<float, uint>(OpenGL, Vbo, Ebo);

            //Telling the VAO object how to lay out the attribute pointers
            Vao.VertexAttributePointer(0, 3, VertexAttribPointerType.Float, 5, 0);
            Vao.VertexAttributePointer(1, 2, VertexAttribPointerType.Float, 5, 3);

            //Mixed transformation.
            ItemTransform = new Transform();
            ItemTransform.Position = new Vector3(0.0f, 0.0f, 0f);
            ItemTransform.Rotation = Quaternion.CreateFromAxisAngle(Vector3.UnitZ, 1f);
            ItemTransform.Scale = 0.5f;
        }

        public unsafe void OnRender(RendererContext renderer)
        {
            OpenGL.Clear((uint)ClearBufferMask.ColorBufferBit);

            //Binding and using our VAO and shader.
            Vao.Bind();

            OpenGLShader opengl_testshader = (OpenGLShader) test_mesh.shader.GetShaderInterface();
            OpenGLTexture opengl_testtexture = (OpenGLTexture) test_mesh.texture.GetTextureInterface();

            opengl_testshader.Use();
            opengl_testtexture.Bind(TextureUnit.Texture0);

            // Using the transformations.
            opengl_testshader.SetUniform("uModel", ItemTransform.ViewMatrix);

            OpenGL.DrawElements(PrimitiveType.Triangles, (uint)test_mesh.Indices.Length, DrawElementsType.UnsignedInt, null);
        }

        public void OnFramebufferResize(Vector2D<int> newSize)
        {
            OpenGL.Viewport(newSize);
        }
        
        public void OnClose()
        {
            Vbo.Dispose();
            Ebo.Dispose();
            Vao.Dispose();
        }
    }
}
