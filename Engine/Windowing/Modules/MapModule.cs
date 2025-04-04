using Silk.NET.Maths;
using SourceRewrite.Files;
using SourceRewrite.Maps;

namespace SourceRewrite.Windowing.Modules
{
    public class MapModule : IGameModule
    {

        public void OnStart()
        {
            // Get -map window argument
            Application.Arguments.TryGetValue("-map", out string mapPath);
            if (mapPath != null)
            {
                MapSystem.LoadMap(FileSystem.GetMapPath(mapPath)); // Load Map from argument
            }
            else
            {
                MapSystem.LoadMap(FileSystem.GetMapPath("default.bsp")); // Load default map
            }
        }

        public void OnUpdate(double deltaTime) { }

        public void OnRender(double deltaTime) { }

        public void OnShutdown() { }

        public void OnFramebufferResize(Vector2D<int> newSize) { }
    }
}
