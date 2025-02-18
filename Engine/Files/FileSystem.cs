using SourceRewrite.Windowing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SourceRewrite.Rendering;
using SourceRewrite.AssetTypes;

namespace SourceRewrite.Files
{
    /// <summary>
    /// Interaction with File.
    /// </summary>
    public static class FileSystem
    {
        public static GamePath GamePath = new GamePath("../../");
        public static GamePath MountedGamePath = new GamePath(GameInfo.GetMountedPaths());

        /// <summary>
        /// Get the path to a Shader from it's path relative to "shaders" folder.
        /// </summary>
        public static string GetShaderPath(string name)
        {
            // Source 1 doesn't have exposed shaders, no need to search mounted games
            return $"{GamePath.ShadersPath}/{name}";
        }

        /// <summary>
        /// Get the path to a Material from it's path relative to "materials" folder.
        /// </summary>
        public static string GetMaterialPath(string name)
        {
            string path = $"{GamePath.MaterialsPath}/{name}";
            if (File.Exists(path)) return path;

            string mountedPath = $"{MountedGamePath.MaterialsPath}/{name}";
            return File.Exists(mountedPath) ? mountedPath : path;
        }

        /// <summary>
        /// Get the path to a Model from it's path relative to "models" folder.
        /// </summary>
        public static string GetModelPath(string name)
        {
            string path = $"{GamePath.ModelsPath}/{name}";
            if (File.Exists(path)) return path;

            string mountedPath = $"{MountedGamePath.ModelsPath}/{name}";
            return File.Exists(mountedPath) ? mountedPath : path;
        }

        /// <summary>
        /// Get the path to a Map from it's path relative to "maps" folder.
        /// </summary>
        public static string GetMapPath(string name)
        {
            // Source 1 doesn't have packed maps, no need to search mounted games.
            return $"{GamePath.MapsPath}/{name}";
        }


        /// <summary>
        /// Get a Shader from it's path relative to "shaders" folder
        /// </summary>
        public static Shader GetShader(string name)
        {
            string vertPath = GetShaderPath($"{name}.vert");
            string fragPath = GetShaderPath($"{name}.frag");
            return new Shader(vertPath, fragPath);
        }

        /// <summary>
        /// Get a Material from it's path relative to "materials" folder.
        /// </summary>
        public static Material GetMaterial(string name)
        {
            return new Material(GetMaterialPath(name));
        }

        /// <summary>
        /// Returns the paths to all mounted games defined in gameinfo.txt.
        /// </summary>
        public static string GetMountedPaths()
        {
            return GameInfo.GetMountedPaths();
        }
    }

    public struct GamePath
    {
        public string ShadersPath { get; private set; } = "../../shaders";
        public string MaterialsPath { get; private set; } = "../../materials";
        public string ModelsPath { get; private set; } = "../../models";
        public string MapsPath { get; private set; } = "../../maps";

        /// <summary>
        /// Path to the Game Folder (Folder containing gameinfo.txt).
        /// </summary>
        public static string BasePath { get; private set; } = "../../";
        
        public GamePath (string basePath)
        {
            ShadersPath = $"{basePath}/shaders";
            MaterialsPath = $"{basePath}/materials";
            ModelsPath = $"{basePath}/models";
            MapsPath = $"{basePath}/maps";

            BasePath = basePath;
        }
    }
}
