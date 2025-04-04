using Silk.NET.Maths;

namespace SourceRewrite.Windowing.Modules
{
    public interface IGameModule
    {
        void OnStart();     // Called when the engine starts
        void OnUpdate(double deltaTime); // Called every frame before rendering
        void OnRender(double deltaTime); // Called every frame after update
        void OnShutdown();  // Called when the engine is shutting down
        void OnFramebufferResize(Vector2D<int> newSize); // Called when the framebuffer is resized
    }
}
