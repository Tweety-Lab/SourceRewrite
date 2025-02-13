using System;
using SourceRewrite.Input;
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
            // Create a shader based on current renderer
            switch (GameWindow.CurrentWindow.Renderer.API)
            {
                case RendererAPI.OpenGL:
                    // Convert Renderer API Interface to an OpenGLContext
                    OpenGLContext glContext = (OpenGLContext) _rendererAPI;

                    // Create a new OpenGL shader
                    _shaderInterface = new OpenGLShader(glContext.OpenGL, vertexPath, fragmentPath);

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
        /// Sets the shader's uniform to the specified value.
        /// </summary>
        public void SetUniform(string uniformName, object value)
        {
            _shaderInterface.SetUniform(uniformName, value);
        }

        /// <summary>
        /// Sets the shader's uniform to the specified value.
        /// </summary>
        public int GetIntUniform(string uniformName)
        {
           return _shaderInterface.GetIntUniform(uniformName);
        }
    }

    /// <summary>
    /// Shader Interface that allows for easy shader use across different Rendering APIs.
    /// </summary>
    public interface IShader
    {
        void SetFragmentCode(string input);
        void SetUniform(string uniformName, object value);
        int GetIntUniform(string uniformName);
    }
}
