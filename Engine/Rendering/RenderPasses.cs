using SourceRewrite.AssetTypes;
using SourceRewrite.Entities;
using SourceRewrite.Entities.GUI;
using SourceRewrite.Windowing;
using System.Numerics;
using System.Reflection;

namespace SourceRewrite.Rendering
{
    /// <summary>
    /// Interface for Render Passes used to group rendering operations.
    /// </summary>
    public interface IRenderPass
    {
        void OnRender();
    }

    /// <summary>
    /// Manager that handles all render passes.
    /// </summary>
    public static class RenderPassManager
    {
        private static readonly List<IRenderPass> RenderPasses = new List<IRenderPass>();

        public static void RenderAllPasses()
        {
            foreach (var pass in RenderPasses)
            {
                pass.OnRender();
            }
        }

        /// <summary>
        /// Register a new Render Pass.
        /// </summary>
        /// <param name="pass"></param>
        public static void RegisterPass(IRenderPass pass) => RenderPasses.Add(pass);
    }

    /// <summary>
    /// Render Meshes from MeshEntity components.
    /// </summary>
    public class MeshEntityPass : IRenderPass
    {
        private readonly IRendererAPI _renderer = GameWindow.CurrentWindow.Renderer.GetRendererAPI();

        public void OnRender()
        {
            RenderEntities(EntityManager.Root);
        }

        private void RenderEntities(BaseEntity root)
        {
            if (root is MeshEntity meshEntity)
            {
                var modelMatrix = RendererContext.GetEntityModelMatrix(meshEntity) ?? Matrix4x4.Identity;
                _renderer.RenderMesh(meshEntity.Mesh, modelMatrix);
            }

            foreach (var child in root.Children)
            {
                RenderEntities(child);
            }
        }
    }

    /// <summary>
    /// Render Screenspace GUI from GUICanvas components.
    /// </summary>
    public class ScreenSpaceRenderPass : IRenderPass
    {
        private readonly IRendererAPI _renderer = GameWindow.CurrentWindow.Renderer.GetRendererAPI();

        public void OnRender()
        {
            RenderEntities(EntityManager.Root);
        }

        private void RenderEntities(BaseEntity root)
        {
            if (root is GUICanvasEntity canvas)
            {
                _renderer.RenderScreenspaceGUI(canvas);
            }

            foreach (var child in root.Children)
            {
                RenderEntities(child);
            }
        }
    }
}
