using SourceRewrite.Entities;
using SourceRewrite.Entities.GUI;
using SourceRewrite.Windowing;
using System.Numerics;
using System.Reflection;

namespace SourceRewrite.Rendering
{
    /// <summary>
    /// Render Passes are used to group together rendering operations.
    /// </summary>
    public interface IRenderPass
    {
        IRendererAPI Renderer { get; }
        void OnRender();
    }

    public static class RenderPassManager
    {
        // Get List of all classes that implement IRenderPass
        public static List<IRenderPass> GetRenderPasses()
        {
            // Get all types that implement IRenderPass in the current assembly
            var renderPassTypes = Assembly.GetExecutingAssembly()
                                          .GetTypes()
                                          .Where(t => typeof(IRenderPass).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract)
                                          .ToList();

            List<IRenderPass> renderPassInstances = new List<IRenderPass>();
            foreach (var type in renderPassTypes)
            {
                // Instantiate the class (assuming parameterless constructor)
                var renderPassInstance = Activator.CreateInstance(type) as IRenderPass;

                if (renderPassInstance != null)
                    renderPassInstances.Add(renderPassInstance);
            }

            return renderPassInstances;
        }
    }

    /// <summary>
    /// Render Meshes from MeshEntity components.
    /// </summary>
    public class MeshEntityPass : IRenderPass
    {
        // Implementing the Renderer property here to access the Renderer of the game window
        public IRendererAPI Renderer => GameWindow.CurrentWindow.Renderer.GetRendererAPI();

        public void OnRender()
        {
            // Render Game Objects starting at the root
            RenderEntities(EntityManager.Root);
        }

        private void RenderEntities(BaseEntity root)
        {
            // If the root is a MeshEntity, render it
            if (root is MeshEntity meshEntity)
            {
                Renderer.RenderMesh(meshEntity.Mesh, RendererContext.GetEntityModelMatrix(meshEntity) ?? Matrix4x4.Identity);
            }

            // Recursively render all children
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
        public IRendererAPI Renderer => GameWindow.CurrentWindow.Renderer.GetRendererAPI();

        public void OnRender()
        {
            // Render Game Objects starting at the root
            RenderEntities(EntityManager.Root);
        }

        private void RenderEntities(BaseEntity root)
        {
            // If the root is a GUICanvas, render it
            if (root is GUICanvasEntity canvas)
            {
                Renderer.RenderScreenspaceGUI(canvas);
            }

            // Recursively render all children
            foreach (var child in root.Children)
            {
                RenderEntities(child);
            }
        }
    }
}
