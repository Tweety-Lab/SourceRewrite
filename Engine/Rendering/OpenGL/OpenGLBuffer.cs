using Silk.NET.OpenGL;
namespace SourceRewrite.Rendering.OpenGL
{
    public class OpenGLBufferObject<TDataType> : IDisposable
        where TDataType : unmanaged
    {
        private readonly uint _handle;
        private readonly BufferTargetARB _bufferType;
        private readonly GL _gl;
        private bool _disposed;

        public unsafe OpenGLBufferObject(GL gl, Span<TDataType> data, BufferTargetARB bufferType)
        {
            _gl = gl;
            _bufferType = bufferType;

            // Generate buffer and get the handle
            _handle = _gl.GenBuffer();
            Bind();

            fixed (void* d = data)
            {
                _gl.BufferData(bufferType, (nuint)(data.Length * sizeof(TDataType)), d, BufferUsageARB.StaticDraw);
            }
        }

        public void Bind()
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(OpenGLBufferObject<TDataType>));

            _gl.BindBuffer(_bufferType, _handle);
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _gl.DeleteBuffer(_handle);
                _disposed = true;
            }
            GC.SuppressFinalize(this);
        }

        ~OpenGLBufferObject()
        {
            Dispose();
        }
    }
}