using Silk.NET.Maths;
using SourceRewrite.Rendering;

namespace SourceRewrite.Windowing.Modules
{
    public class RenderModule : IGameModule
    {
        public RendererContext Context { get; private set; }

        public void OnStart()
        {
            Context = new RendererContext(RendererAPI.OpenGL, GameWindow.CurrentWindow);
            Context.OnLoad();

            Context.SetClearColour(13, 13, 13, 255);

            // Register Default Render Passes
            RenderPassManager.RegisterPass(new OpaquePass());
            RenderPassManager.RegisterPass(new BillboardPass());
            RenderPassManager.RegisterPass(new LightingPass());
            RenderPassManager.RegisterPass(new ScreenspaceGUIRenderPass());
        }

        public void OnUpdate(double deltaTime) { }

        public void OnRender(double deltaTime)
        {
            Context.OnRender();
        }

        public void OnShutdown()
        {
            Context.OnClose();
        }

        public void OnFramebufferResize(Vector2D<int> newSize)
        {
            Context.OnFramebufferResize(newSize);
        }
    }
}
