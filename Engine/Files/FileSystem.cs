using SourceRewrite.Rendering;
using SourceRewrite.AssetTypes;
using System.IO;

namespace SourceRewrite.Files
{
    /// <summary>
    /// Interaction with File.
    /// </summary>
    public static class FileSystem
    {
        public static GamePath GamePath = new GamePath("../../");
        public static GamePath[] MountedGamePaths = GetMountedGamePaths();

        /// <summary>
        /// Get the path to a Shader from its path relative to the "shaders" folder.
        /// </summary>
        public static string GetShaderPath(string name)
        {
            string path = $"{GamePath.ShadersPath}/{name}.shader";
            if (File.Exists(path)) return path;

            // Search through mounted game paths
            foreach (var mountedPath in MountedGamePaths)
            {
                string mountedShaderPath = $"{mountedPath.ShadersPath}/{name}.shader";
                if (File.Exists(mountedShaderPath)) return mountedShaderPath;
            }

            return path;
        }

        /// <summary>
        /// Get the path to a Material from its path relative to the "materials" folder.
        /// </summary>
        public static string GetMaterialPath(string name)
        {
            string path = $"{GamePath.MaterialsPath}/{name}.vmt";
            if (File.Exists(path)) return path;

            // Search through mounted game paths
            foreach (var mountedPath in MountedGamePaths)
            {
                string mountedMaterialPath = $"{mountedPath.MaterialsPath}/{name}.vmt";
                if (File.Exists(mountedMaterialPath)) return mountedMaterialPath;
            }

            return path;
        }

        /// <summary>
        /// Get the path to a Texture from its path relative to the "materials" folder.
        /// </summary>
        public static string GetTexturePath(string name)
        {
            string path = $"{GamePath.MaterialsPath}/{name}.vtf";
            if (File.Exists(path)) return path;

            // Search through mounted game paths
            foreach (var mountedPath in MountedGamePaths)
            {
                string mountedTexturePath = $"{mountedPath.MaterialsPath}/{name}.vtf";
                if (File.Exists(mountedTexturePath)) return mountedTexturePath;
            }

            return path;
        }

        /// <summary>
        /// Get the path to a Model from its path relative to the "models" folder.
        /// </summary>
        public static string GetModelPath(string name)
        {
            string path = $"{GamePath.ModelsPath}/{name}";
            if (File.Exists(path)) return path;

            // Search through mounted game paths
            foreach (var mountedPath in MountedGamePaths)
            {
                string mountedModelPath = $"{mountedPath.ModelsPath}/{name}";
                if (File.Exists(mountedModelPath)) return mountedModelPath;
            }

            return path;
        }

        /// <summary>
        /// Get the path to a Map from its path relative to the "maps" folder.
        /// </summary>
        public static string GetMapPath(string name)
        {
            // Source 1 doesn't have maps like ours, no need to search mounted games.
            return $"{GamePath.MapsPath}/{name}";
        }

        /// <summary>
        /// Get a Shader from its path relative to the "shaders" folder.
        /// </summary>
        public static Shader GetShader(string name)
        {
            string path = GetShaderPath($"{name}");
            return new Shader(path);
        }

        /// <summary>
        /// Get the path to a GUI file from its path relative to the "gui" folder.
        /// </summary>
        public static string GetGUIPath(string name)
        {
            // Source 1 doesn't have GUI like ours, no need to search mounted games.
            return $"{GamePath.GUIPath}/{name}";
        }

        /// <summary>
        /// Get a Material from its path relative to the "materials" folder.
        /// </summary>
        public static Material GetMaterial(string name)
        {
            return new Material(GetMaterialPath(name));
        }

        /// <summary>
        /// Returns the paths to all mounted games defined in gameinfo.txt.
        /// </summary>
        public static GamePath[] GetMountedGamePaths()
        {
            string[] mountedPaths = GameInfo.GetMountedPaths();
            GamePath[] gamePaths = new GamePath[mountedPaths.Length];

            for (int i = 0; i < mountedPaths.Length; i++)
            {
                gamePaths[i] = new GamePath(mountedPaths[i]);
            }

            return gamePaths;
        }
    }

    public struct GamePath
    {
        public string ShadersPath { get; }
        public string MaterialsPath { get; }
        public string ModelsPath { get; }
        public string MapsPath { get; }
        public string GUIPath { get; }
        public string BasePath { get; }

        /// <summary>
        /// Path to the Game Folder (Folder containing gameinfo.txt).
        /// </summary>
        public GamePath(string basePath)
        {
            ShadersPath = $"{basePath}/shaders";
            MaterialsPath = $"{basePath}/materials";
            ModelsPath = $"{basePath}/models";
            MapsPath = $"{basePath}/maps";
            GUIPath = $"{basePath}/gui";
            BasePath = basePath;
        }
    }
}
