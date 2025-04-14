using SourceRewrite.Rendering.OpenGL;
using SourceRewrite.Files;
using FileFormats.VTF;
using SourceRewrite.Rendering;
using SourceRewrite.Windowing.Modules;
using System.Numerics;

namespace SourceRewrite.AssetTypes
{
    /// <summary>
    /// Texture Interaction Class.
    /// </summary>
    public class Texture : IDisposable
    {
        private IRendererAPI _rendererAPI = GameModules.GetModule<RenderModule>().Context.GetRendererAPI();
        private readonly ITexture _textureInterface; // Use an interface for better abstraction

        // Store width and height
        public uint Width { get; private set; }
        public uint Height { get; private set; }

        // Create a texture from a .VTF path
        public Texture(string path)
        {
            // If Texture cant be found, set it to missing
            if (!File.Exists(path))
            {
                Console.WriteLine($"Could not find texture at '{path}'");
                path = FileSystem.GetTexturePath("dev/missing");
            }

            // Load VTF
            VTFFormat texture = new VTFFormat(path);
            byte[] data = texture.GetBgra32Data();

            // Set width and height
            Width = (uint)texture.Width;
            Height = (uint)texture.Height;

            // Create a texture based on current renderer
            switch (GameModules.GetModule<RenderModule>().Context.API)
            {
                case RendererAPI.OpenGL:
                    // Convert Renderer API Interface to an OpenGLContext
                    OpenGLContext glContext = (OpenGLContext)_rendererAPI;

                    _textureInterface = new OpenGLTexture(glContext.OpenGL, data, (uint)texture.Height, (uint)texture.Width);

                    break;
            }
        }

        // Create a texture from bgra data
        public Texture(byte[] bgra32Data, uint height, uint width)
        {
            // Set width and height
            Width = width;
            Height = height;

            // Create a texture based on current renderer
            switch (GameModules.GetModule<RenderModule>().Context.API)
            {
                case RendererAPI.OpenGL:
                    // Convert Renderer API Interface to an OpenGLContext
                    OpenGLContext glContext = (OpenGLContext)_rendererAPI;

                    _textureInterface = new OpenGLTexture(glContext.OpenGL, bgra32Data, height, width);

                    break;
            }
        }

        /// <summary>
        /// Gets the Textures low-level Interface. Can be used for casting from SourceRewrite Texture to OpenGL Texture for example.
        /// </summary>
        public ITexture GetTextureInterface() => _textureInterface;

        public void Dispose() => _textureInterface.Dispose();
    }

    /// <summary>
    /// Texture Interface that allows for easy texture use across different Rendering APIs.
    /// </summary>
    public interface ITexture
    {
        public void Dispose();
    }
}
