using SourceRewrite.Rendering.OpenGL;
using SourceRewrite.Windowing;
using SourceRewrite.Files;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SourceRewrite.Rendering
{
    /// <summary>
    /// Texture Interaction Class.
    /// </summary>
    public class Texture
    {
        private IRendererAPI _rendererAPI = GameWindow.CurrentWindow.Renderer.GetRendererAPI();
        private ITexture _textureInterface; // Use an interface for better abstraction
        public Texture(string path)
        {
            // If Texture cant be found, set it to missing
            if (!File.Exists(path)) 
            {
                Console.WriteLine($"Could not find texture at '{path}'");
                path = FileSystem.GetMaterialPath("dev/missing.vtf");
            }

            // Create a shader based on current renderer
            switch (GameWindow.CurrentWindow.Renderer.API)
            {
                case RendererAPI.OpenGL:
                    // Convert Renderer API Interface to an OpenGLContext
                    OpenGLContext glContext = (OpenGLContext)_rendererAPI;

                    _textureInterface = new OpenGLTexture(glContext.OpenGL, path);

                    break;
            }
        }

        /// <summary>
        /// Gets the Textures low-level Interface. Can be used for casting from SourceRewrite Texture to OpenGL Texture for example.
        /// </summary>
        public ITexture GetTextureInterface() => _textureInterface;

        /// <summary>
        /// Dynamically set the path of the Texture.
        /// </summary>
        public void SetTexturePath(string path)
        {
            _textureInterface.SetTexturePath(path);
        }
    }

    /// <summary>
    /// Texture Interface that allows for easy texture use across different Rendering APIs.
    /// </summary>
    public interface ITexture
    {
        void SetTexturePath(string path);
    }
}
