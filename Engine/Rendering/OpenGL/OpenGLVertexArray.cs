using Silk.NET.OpenGL;

namespace SourceRewrite.Rendering.OpenGL
{
    /// <summary>
    /// Represents an OpenGL Vertex Array Object (VAO) that manages vertex attribute pointers and binds VBOs and EBOs.
    /// </summary>
    /// <typeparam name="TVertexType">The type of the vertex data (must be unmanaged).</typeparam>
    /// <typeparam name="TIndexType">The type of the index data (must be unmanaged).</typeparam>
    public class OpenGLVertexArrayObject<TVertexType, TIndexType> : IDisposable
        where TVertexType : unmanaged
        where TIndexType : unmanaged
    {
        private readonly uint _handle;
        private readonly GL _gl;

        private bool _disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="OpenGLVertexArrayObject{TVertexType, TIndexType}"/> class.
        /// </summary>
        /// <param name="gl">The OpenGL instance to use.</param>
        /// <param name="vbo">The Vertex Buffer Object (VBO) to bind to this VAO.</param>
        /// <param name="ebo">The Element Buffer Object (EBO) to bind to this VAO.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="gl"/>, <paramref name="vbo"/>, or <paramref name="ebo"/> is null.</exception>
        public OpenGLVertexArrayObject(GL gl, OpenGLBufferObject<TVertexType> vbo, OpenGLBufferObject<TIndexType> ebo)
        {
            _gl = gl ?? throw new ArgumentNullException(nameof(gl));
            if (vbo == null) throw new ArgumentNullException(nameof(vbo));
            if (ebo == null) throw new ArgumentNullException(nameof(ebo));

            // Generate and bind the VAO
            _handle = _gl.GenVertexArray();
            Bind();

            // Bind the VBO and EBO to this VAO
            vbo.Bind();
            ebo.Bind();
        }

        /// <summary>
        /// Specifies the location and format of a vertex attribute.
        /// </summary>
        /// <param name="index">The index of the vertex attribute.</param>
        /// <param name="count">The number of components per vertex attribute (e.g., 3 for a vec3).</param>
        /// <param name="type">The data type of each component.</param>
        /// <param name="vertexSize">The stride (in bytes) between consecutive vertices.</param>
        /// <param name="offset">The offset (in bytes) of the first component in the vertex.</param>
        public unsafe void VertexAttributePointer(uint index, int count, VertexAttribPointerType type, int stride, int offset)
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(OpenGLVertexArrayObject<TVertexType, TIndexType>));

            _gl.VertexAttribPointer(
                index,
                count,
                type,
                false, // Normalized (set to false for most cases)
                (uint)stride * sizeof(float), // Assuming stride is in float count
                (void*)(offset * sizeof(float)) // Assuming offset is in float count
            );
            _gl.EnableVertexAttribArray(index);
        }

        /// <summary>
        /// Binds this VAO.
        /// </summary>
        public void Bind()
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(OpenGLVertexArrayObject<TVertexType, TIndexType>));

            _gl.BindVertexArray(_handle);
        }

        /// <summary>
        /// Unbinds this VAO.
        /// </summary>
        public void Unbind()
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(OpenGLVertexArrayObject<TVertexType, TIndexType>));

            _gl.BindVertexArray(0);
        }

        /// <summary>
        /// Disposes of the VAO and releases OpenGL resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // No managed resources to dispose
                }

                // Delete the VAO
                _gl.DeleteVertexArray(_handle);
                _disposed = true;
            }
        }

        ~OpenGLVertexArrayObject()
        {
            Dispose(false);
        }
    }
}