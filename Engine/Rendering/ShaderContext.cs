using System;
using SourceRewrite.Rendering;
using SourceRewrite.Rendering.OpenGL;
using SourceRewrite.Windowing;

namespace SourceRewrite.Rendering
{
    /// <summary>
    /// Shader Interaction Class.
    /// </summary>
    public class Shader
    {
        private IRendererAPI _rendererAPI = GameWindow.CurrentWindow.Renderer.GetRendererAPI();
        private IShader _shaderInterface; // Use an interface for better abstraction

        public Shader(string vertexPath, string fragmentPath)
        {
            string vertexSource = File.ReadAllText(vertexPath);
            string fragmentSource = File.ReadAllText(fragmentPath);

            // Create a shader based on current renderer
            switch (GameWindow.CurrentWindow.Renderer.API)
            {
                case RendererAPI.OpenGL:
                    // Convert Renderer API Interface to an OpenGLContext
                    OpenGLContext glContext = (OpenGLContext) _rendererAPI;

                    // Create a new OpenGL shader
                    _shaderInterface = new OpenGLShader(glContext.OpenGL, vertexSource, fragmentSource);

                    break;
            }
        }

        /// <summary>
        /// Gets the Shaders low-level Interface. Can be used for casting from SourceRewrite Shader to OpenGL Shader for example.
        /// </summary>
        public IShader GetShaderInterface() => _shaderInterface;

        /// <summary>
        /// Dynamically Set the shaders Fragment Code.
        /// </summary>
        public void SetFragmentCode(string input)
        {
            _shaderInterface.SetFragmentCode(input);
        }

        /// <summary>
        /// Sets the shader's specified parameter (uniform) to the specified value.
        /// </summary>
        public void SetParameter(string uniformName, object value)
        {
            _shaderInterface.SetParameter(uniformName, value);
        }

        /// <summary>
        /// Sets the shader's uniform to the specified value.
        /// </summary>
        public int GetIntParameter(string uniformName)
        {
           return _shaderInterface.GetIntParameter(uniformName);
        }
    }

    /// <summary>
    /// Shader Interface that allows for easy shader use across different Rendering APIs.
    /// </summary>
    public interface IShader
    {
        void SetFragmentCode(string input);
        void SetParameter(string uniformName, object value);
        int GetIntParameter(string uniformName);
    }
}
