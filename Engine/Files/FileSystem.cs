using SourceRewrite.Windowing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SourceRewrite.Rendering;
using SourceRewrite.Assets;

namespace SourceRewrite.Files
{
    /// <summary>
    /// Interaction with File.
    /// </summary>
    public static class FileSystem
    {
        public static string ShadersPath { get; private set; } = "../../shaders";
        public static string MaterialsPath { get; private set; } = "../../materials";
        public static string ModelsPath { get; private set; } = "../../models";

        /// <summary>
        /// Path to the Game Folder (Folder containing gameinfo.txt).
        /// </summary>
        public static string GamePath { get; private set; } = "../../";

        /// <summary>
        /// Get the path to a Shader from it's name.
        /// </summary>
        public static string GetShaderPath(string name)
        {
            return $"{ShadersPath}/{name}";
        }

        /// <summary>
        /// Get the path to a Material from it's name.
        /// </summary>
        public static string GetMaterialPath(string name)
        {
            return $"{MaterialsPath}/{name}";
        }

        /// <summary>
        /// Get the path to a Model from it's name.
        /// </summary>
        public static string GetModelPath(string name)
        {
            return $"{ModelsPath}/{name}";
        }


        /// <summary>
        /// Get a Shader from it's name.
        /// </summary>
        public static Shader GetShader(string name)
        {
            return new Shader($"{ShadersPath}/{name}.vert", $"{ShadersPath}/{name}.frag");
        }

        /// <summary>
        /// Get a Material from it's name.
        /// </summary>
        public static Material GetMaterial(string name)
        {
            return new Material($"{MaterialsPath}/{name}");
        }
    }
}
