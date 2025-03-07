using Silk.NET.OpenGL;
using FileFormats.VTF;

namespace SourceRewrite.Rendering.OpenGL
{
    public class OpenGLTexture : IDisposable, ITexture
    {
        private uint _handle;
        private GL _gl;

        public unsafe OpenGLTexture(GL gl, uint height, uint width, byte[] data)
        {
            //Saving the gl instance.
            _gl = gl;

            //Generating the opengl handle;
            _handle = _gl.GenTexture();
            Bind();

            // Upload the data
            fixed (byte* ptr = data)
            {
                _gl.TexImage2D(TextureTarget.Texture2D, 0, InternalFormat.Rgba, width,
                    height, 0, PixelFormat.Bgra, PixelType.UnsignedByte, ptr);
            }

            SetParameters();
        }

        public unsafe OpenGLTexture(GL gl, Span<byte> data, uint width, uint height)
        {
            //Saving the gl instance.
            _gl = gl;

            //Generating the opengl handle;
            _handle = _gl.GenTexture();
            Bind();

            //We want the ability to create a texture using data generated from code aswell.
            fixed (void* d = &data[0])
            {
                //Setting the data of a texture.
                _gl.TexImage2D(TextureTarget.Texture2D, 0, (int)InternalFormat.Rgba, width, height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, d);
                SetParameters();
            }
        }

        private void SetParameters()
        {
            _gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            _gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            _gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            _gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);

            //Generating mipmaps.
            _gl.GenerateMipmap(TextureTarget.Texture2D);
        }

        public void Bind(TextureUnit textureSlot = TextureUnit.Texture0)
        {
            //When we bind a texture we can choose which textureslot we can bind it to.
            _gl.ActiveTexture(textureSlot);
            _gl.BindTexture(TextureTarget.Texture2D, _handle);
        }

        public void Dispose()
        {
            //In order to dispose we need to delete the opengl handle for the texure.
            _gl.DeleteTexture(_handle);
        }

        // ITexture
        public void SetTexturePath(string path)
        {

        }

    }
}
