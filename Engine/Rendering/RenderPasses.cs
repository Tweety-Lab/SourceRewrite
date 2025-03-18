using SourceRewrite.Components;
using SourceRewrite.Objects;
using SourceRewrite.Windowing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

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
    /// Render Meshes from MeshRenderer components.
    /// </summary>
    public class MeshRendererPass : IRenderPass
    {
        // Implementing the Renderer property here to access the Renderer of the game window
        public IRendererAPI Renderer => GameWindow.CurrentWindow.Renderer.GetRendererAPI();

        public void OnRender()
        {
            // Render Game Objects starting at the root
            RenderGameObjects(GameObjectManager.Root);
        }

        private void RenderGameObjects(GameObject root)
        {
            // Render the current GameObject and its components
            RenderComponents(root);

            // Recursively render all children
            foreach (var child in root.Children)
            {
                RenderGameObjects(child);
            }
        }

        private void RenderComponents(GameObject gameObject)
        {
            // Render all Meshes
            MeshRenderer meshRenderer = gameObject.GetComponentFromType<MeshRenderer>();
            if (meshRenderer != null)
            {
                Renderer.RenderMesh(meshRenderer);
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
            RenderGameObjects(GameObjectManager.Root);
        }

        private void RenderGameObjects(GameObject root)
        {
            // Render the current GameObject and its components
            RenderComponents(root);

            // Recursively render all children
            foreach (var child in root.Children)
            {
                RenderGameObjects(child);
            }
        }

        private void RenderComponents(GameObject gameObject)
        {
            // Render all visible Screenspace GUIs
            GUICanvas guiCanvas = gameObject.GetComponentFromType<GUICanvas>();
            if (guiCanvas != null && guiCanvas.PanelType != 0 && guiCanvas.Container.VistaView.Visible == true)
            {
                Renderer.RenderScreenspaceGUI(guiCanvas);
            }
        }
    }
}
