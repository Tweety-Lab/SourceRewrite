using System;
using System.IO;
using Silk.NET.OpenGL;
using System.Numerics;

namespace SourceRewrite.Rendering.OpenGL
{
    public class OpenGLShader : IDisposable, IShader
    {
        // Our handle and the GL instance this class will use, these are private because they have no reason to be public.
        // Most of the time you would want to abstract items to make things like this invisible.
        private uint _handle;
        private GL _gl;

        public OpenGLShader(GL gl, string vertexPath, string fragmentPath)
        {
            _gl = gl;

            uint vertex = LoadShader(ShaderType.VertexShader, vertexPath);
            uint fragment = LoadShader(ShaderType.FragmentShader, fragmentPath);
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

        // Uniforms are properties that applies to the entire geometry
        public void SetUniform(string name, int value)
        {
            // Setting a uniform on a shader using a name.
            int location = _gl.GetUniformLocation(_handle, name);
            if (location == -1) // If GetUniformLocation returns -1 the uniform is not found.
            {
                throw new Exception($"{name} uniform not found on shader.");
            }
            _gl.Uniform1(location, value);
        }

        public unsafe void SetUniform(string name, Matrix4x4 value)
        {
            //A new overload has been created for setting a uniform so we can use the transform in our shader.
            int location = _gl.GetUniformLocation(_handle, name);
            if (location == -1)
            {
                throw new Exception($"{name} uniform not found on shader.");
            }
            _gl.UniformMatrix4(location, 1, false, (float*)&value);
        }

        public void SetUniform(string name, float value)
        {
            int location = _gl.GetUniformLocation(_handle, name);
            if (location == -1)
            {
                throw new Exception($"{name} uniform not found on shader.");
            }
            _gl.Uniform1(location, value);
        }

        public void Dispose()
        {
            // Remember to delete the program when we are done.
            _gl.DeleteProgram(_handle);
        }

        private uint LoadShader(ShaderType type, string path)
        {
            // To load a single shader we need to:
            // 1) Load the shader from a file.
            // 2) Create the handle.
            // 3) Upload the source to opengl.
            // 4) Compile the shader.
            // 5) Check for errors.
            string src = File.ReadAllText(path);
            uint handle = _gl.CreateShader(type);
            _gl.ShaderSource(handle, src);
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
    }
}
