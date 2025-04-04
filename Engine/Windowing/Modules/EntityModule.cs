using Silk.NET.Maths;
using SourceRewrite.Entities;

namespace SourceRewrite.Windowing.Modules
{
    public class EntityModule : IGameModule
    {

        public void OnStart()
        {
            // Load Console as Global Entity
            EntityManager.AddGlobalEntity(new DeveloperConsoleCanvas());
        }

        public void OnUpdate(double deltaTime) { }

        public void OnRender(double deltaTime) { }

        public void OnShutdown() { }

        public void OnFramebufferResize(Vector2D<int> newSize) { }
    }
}
