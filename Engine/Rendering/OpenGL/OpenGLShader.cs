using Silk.NET.OpenGL;
using SourceRewrite.AssetTypes;
using System.Numerics;

namespace SourceRewrite.Rendering.OpenGL
{
    public class OpenGLShader : IDisposable, IShader
    {
        // Our handle and the GL instance this class will use
        private readonly uint _handle;
        private readonly GL _gl;

        public OpenGLShader(GL gl, string vertexSource, string fragmentSource)
        {
            _gl = gl;

            uint vertex = LoadShader(ShaderType.VertexShader, vertexSource);
            uint fragment = LoadShader(ShaderType.FragmentShader, fragmentSource);

            _handle = _gl.CreateProgram();
            _gl.AttachShader(_handle, vertex);
            _gl.AttachShader(_handle, fragment);
            _gl.LinkProgram(_handle);
            _gl.GetProgram(_handle, GLEnum.LinkStatus, out var status);

            if (status == 0)
            {
                throw new Exception($"Program failed to link with error: {_gl.GetProgramInfoLog(_handle)}");
            }

            _gl.DetachShader(_handle, vertex);
            _gl.DetachShader(_handle, fragment);
            _gl.DeleteShader(vertex);
            _gl.DeleteShader(fragment);
        }

        public void Use()
        {
            // Using the program
            _gl.UseProgram(_handle);
        }

        /// <summary>
        /// Set Shader Parameter (Uniform).
        /// </summary>
        public unsafe void SetParameter(string name, object value)
        {
            int location = _gl.GetUniformLocation(_handle, name);
            if (location == -1) // If GetUniformLocation returns -1 the uniform is not found.
            {
                return;
            }

            _gl.UseProgram(_handle); // Bind our Shader to allow uniform changes

            // Automatically handle different data types
            switch (value)
            {
                case float f:
                    _gl.Uniform1(location, f);
                    break;
                case int i:
                    _gl.Uniform1(location, i);
                    break;
                case Matrix4x4 matrix4:
                    _gl.UniformMatrix4(location, 1, false, (float*)&matrix4);
                    break;
                case Vector4 vector4:
                    _gl.Uniform4(location, 1, (float*)&vector4);
                    break;
                case Vector3 vector3:
                    _gl.Uniform3(location, 1, (float*)&vector3);
                    break;
                case AssetTypes.Texture texture:
                    OpenGLTexture glTexture = (OpenGLTexture)texture.GetTextureInterface();

                    // Get the next available texture unit
                    TextureUnit unit = GetNextAvailableTextureUnit();
                    int unitIndex = (int)unit - (int)TextureUnit.Texture0;

                    // Activate, bind, and set uniform
                    glTexture.Bind(unit);
                    _gl.Uniform1(location, unitIndex);

                    break;
            }
        }

        /// <summary>
        /// Get Shader Int Parameter (Uniform).
        /// </summary>
        public unsafe int GetIntParameter(string name)
        {
            int location = _gl.GetUniformLocation(_handle, name);
            if (location == -1)
            {
                throw new ArgumentException($"Uniform {name} not found in shader");
            }

            int[] output = new int[1];
            _gl.GetUniform(_handle, location, output);
            return output[0];
        }

        public void Dispose()
        {
            // Remember to delete the program when we are done.
            _gl.DeleteProgram(_handle);
        }

        private uint LoadShader(ShaderType type, string source)
        {
            // To load a single shader we need to:
            // 1) Create the handle.
            // 2) Upload the source to OpenGL.
            // 3) Compile the shader.
            // 4) Check for errors.
            uint handle = _gl.CreateShader(type);
            _gl.ShaderSource(handle, source);
            _gl.CompileShader(handle);
            string infoLog = _gl.GetShaderInfoLog(handle);
            if (!string.IsNullOrWhiteSpace(infoLog))
            {
                throw new Exception($"Error compiling shader of type {type}, failed with error {infoLog}");
            }

            return handle;
        }

        // IShader
        public void SetFragmentCode(string input)
        {
            // Create a new fragment shader from the input string
            uint fragment = _gl.CreateShader(ShaderType.FragmentShader);
            _gl.ShaderSource(fragment, input);
            _gl.CompileShader(fragment);

            // Check for compilation errors
            string infoLog = _gl.GetShaderInfoLog(fragment);
            if (!string.IsNullOrWhiteSpace(infoLog))
            {
                _gl.DeleteShader(fragment); // Cleanup on error
                throw new Exception($"Error compiling fragment shader: {infoLog}");
            }

            // Attach and link the new shader
            _gl.AttachShader(_handle, fragment);
            _gl.LinkProgram(_handle);

            // Check for linking errors
            _gl.GetProgram(_handle, GLEnum.LinkStatus, out var status);
            if (status == 0)
            {
                _gl.DeleteShader(fragment); // Cleanup on error
                throw new Exception($"Program failed to relink after updating fragment shader: {_gl.GetProgramInfoLog(_handle)}");
            }
        }

        static int currentUnit = 0;
        private TextureUnit GetNextAvailableTextureUnit()
        {
            // Simple implementation - just cycle through units (replace this)
            currentUnit = (currentUnit + 1) % 128; // Assuming 128 texture units available
            return TextureUnit.Texture0 + currentUnit;
        }
    }
}
