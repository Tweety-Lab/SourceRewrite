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

        // Track texture bindings with their texture units
        private readonly Dictionary<int, TextureBinding> _boundTextures = new Dictionary<int, TextureBinding>();

        // Store texture binding information
        private class TextureBinding
        {
            public AssetTypes.Texture Texture { get; set; }
            public int TextureUnit { get; set; }
            public bool IsActive { get; set; } = true;
        }

        // LRU cache for texture unit management
        private static readonly LRUTextureUnitCache _textureUnitCache;
        private static readonly object _textureUnitLock = new object();
        private static readonly int _maxTextureUnits;

        static OpenGLShader()
        {
            // In a real implementation, query this from OpenGL
            _maxTextureUnits = 16; // Conservative default, should be queried at runtime
            _textureUnitCache = new LRUTextureUnitCache(_maxTextureUnits);
        }

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

            // Refresh all active texture bindings
            foreach (var binding in _boundTextures.Values)
            {
                if (binding.IsActive)
                {
                    OpenGLTexture glTexture = (OpenGLTexture)binding.Texture.GetTextureInterface();
                    TextureUnit unit = TextureUnit.Texture0 + binding.TextureUnit;
                    glTexture.Bind(unit);
                }
            }
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

                    // Check if we already have this uniform bound
                    if (_boundTextures.TryGetValue(location, out var existingBinding))
                    {
                        // If binding a new texture to the same uniform
                        if (existingBinding.Texture != texture)
                        {
                            // Mark the old binding as inactive
                            existingBinding.IsActive = false;

                            // Get a texture unit (may reuse one from the LRU cache)
                            int unitIndex = GetTextureUnit(texture);
                            TextureUnit unit = TextureUnit.Texture0 + unitIndex;

                            // Activate, bind, and set uniform
                            glTexture.Bind(unit);
                            _gl.Uniform1(location, unitIndex);

                            // Update tracking info
                            _boundTextures[location] = new TextureBinding
                            {
                                Texture = texture,
                                TextureUnit = unitIndex,
                                IsActive = true
                            };
                        }
                        else if (!existingBinding.IsActive)
                        {
                            // Same texture but was inactive, reactivate it
                            int unitIndex = GetTextureUnit(texture);
                            TextureUnit unit = TextureUnit.Texture0 + unitIndex;

                            glTexture.Bind(unit);
                            _gl.Uniform1(location, unitIndex);

                            existingBinding.TextureUnit = unitIndex;
                            existingBinding.IsActive = true;
                        }
                        else
                        {
                            // Same texture and still active, just touch it in the LRU cache
                            lock (_textureUnitLock)
                            {
                                _textureUnitCache.Touch(existingBinding.TextureUnit);
                            }
                        }
                    }
                    else
                    {
                        // New texture binding
                        int unitIndex = GetTextureUnit(texture);
                        TextureUnit unit = TextureUnit.Texture0 + unitIndex;

                        // Activate, bind, and set uniform
                        glTexture.Bind(unit);
                        _gl.Uniform1(location, unitIndex);

                        // Track the binding
                        _boundTextures[location] = new TextureBinding
                        {
                            Texture = texture,
                            TextureUnit = unitIndex,
                            IsActive = true
                        };
                    }
                    break;
            }
        }

        /// <summary>
        /// Get Shader Parameter (Uniform).
        /// </summary>
        public unsafe T GetParameter<T>(string name)
        {
            int location = _gl.GetUniformLocation(_handle, name);
            if (location == -1)
            {
                throw new Exception($"Uniform '{name}' not found in shader");
            }

            _gl.UseProgram(_handle);

            // Handle different return types
            if (typeof(T) == typeof(float))
            {
                float value = 0;
                _gl.GetUniform(_handle, location, &value);
                return (T)(object)value;
            }
            else if (typeof(T) == typeof(int))
            {
                int value = 0;
                _gl.GetUniform(_handle, location, &value);
                return (T)(object)value;
            }
            else if (typeof(T) == typeof(Vector3))
            {
                Vector3 value = default;
                _gl.GetUniform(_handle, location, (float*)&value);
                return (T)(object)value;
            }
            else if (typeof(T) == typeof(Vector4))
            {
                Vector4 value = default;
                _gl.GetUniform(_handle, location, (float*)&value);
                return (T)(object)value;
            }
            else if (typeof(T) == typeof(Matrix4x4))
            {
                Matrix4x4 value = default;
                _gl.GetUniform(_handle, location, (float*)&value);
                return (T)(object)value;
            }
            else if (typeof(T) == typeof(AssetTypes.Texture))
            {
                if (_boundTextures.TryGetValue(location, out var binding) && binding.IsActive)
                {
                    return (T)(object)binding.Texture;
                }

                // No texture found
                return default;
            }
            else
            {
                throw new NotSupportedException($"Type {typeof(T)} is not supported for uniform retrieval");
            }
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

        // Improved texture unit management with LRU cache
        private static int GetTextureUnit(AssetTypes.Texture texture)
        {
            lock (_textureUnitLock)
            {
                return _textureUnitCache.GetTextureUnit();
            }
        }

        public void Dispose()
        {
            // Cleanup texture bindings
            _boundTextures.Clear();

            // Delete the program when we are done
            _gl.DeleteProgram(_handle);
        }
    }

    /// <summary>
    /// Least Recently Used (LRU) cache for texture unit management
    /// </summary>
    public class LRUTextureUnitCache
    {
        private readonly LinkedList<int> _lruList = new LinkedList<int>();
        private readonly Dictionary<int, LinkedListNode<int>> _unitMap = new Dictionary<int, LinkedListNode<int>>();
        private readonly int _capacity;

        public LRUTextureUnitCache(int capacity)
        {
            _capacity = capacity;

            // Initialize with all available texture units
            for (int i = 0; i < capacity; i++)
            {
                var node = _lruList.AddLast(i);
                _unitMap[i] = node;
            }
        }

        public int GetTextureUnit()
        {
            // Get the least recently used unit
            int unit = _lruList.First.Value;
            Touch(unit);
            return unit;
        }

        public void Touch(int unit)
        {
            // Move this unit to the end of the list (most recently used)
            if (_unitMap.TryGetValue(unit, out var node))
            {
                _lruList.Remove(node);
                _lruList.AddLast(node);
            }
            else
            {
                // This shouldn't happen if the cache is properly maintained
                var newNode = _lruList.AddLast(unit);
                _unitMap[unit] = newNode;
            }
        }
    }
}