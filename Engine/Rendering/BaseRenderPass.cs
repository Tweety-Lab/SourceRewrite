using SourceRewrite.AssetTypes;
using SourceRewrite.Entities;
using SourceRewrite.Entities.Env;
using SourceRewrite.Entities.GUI;
using SourceRewrite.Entities.Lighting;
using SourceRewrite.Entities.Point;
using SourceRewrite.Windowing.Modules;
using System.Numerics;

namespace SourceRewrite.Rendering
{
    /// <summary>
    /// Interface for Render Passes used to group rendering operations.
    /// </summary>
    public interface IRenderPass
    {
        void OnRender();

        /// <summary>
        /// List of render pass flags.
        /// </summary>
        List<RenderFlag> RenderPassFlags { get; }
    }

    /// <summary>
    /// Possible Render Flags.
    /// </summary>
    public enum RenderFlag
    {
        DepthTest,
        Blend,
        CullFrontFaces,
        CullBackFaces
    }

    /// <summary>
    /// Manager that handles all render passes.
    /// </summary>
    public static class RenderPassManager
    {
        private static readonly List<IRenderPass> RenderPasses = new List<IRenderPass>();

        /// <summary>
        /// Render all registered render passes.
        /// </summary>
        public static void RenderAllPasses()
        {
            foreach (var pass in RenderPasses)
            {
                // Apply flags before rendering the pass
                ApplyFlags(pass.RenderPassFlags);

                // Execute the rendering
                pass.OnRender();

                // Revert flags after rendering the pass
                RevertFlags(pass.RenderPassFlags);
            }
        }

        /// <summary>
        /// Register a new Render Pass.
        /// </summary>
        /// <param name="pass">The render pass to register.</param>
        public static void RegisterPass(IRenderPass pass) => RenderPasses.Add(pass);

        private static void ApplyFlags(IEnumerable<RenderFlag> flags)
        {
            var rendererAPI = GameModules.GetModule<RenderModule>().Context.GetRendererAPI();
            foreach (var flag in flags)
            {
                rendererAPI.EnableFlag(flag);
            }
        }

        private static void RevertFlags(IEnumerable<RenderFlag> flags)
        {
            var rendererAPI = GameModules.GetModule<RenderModule>().Context.GetRendererAPI();
            foreach (var flag in flags)
            {
                rendererAPI.DisableFlag(flag);
            }
        }
    }

    /// <summary>
    /// Base class for Render Passes.
    /// </summary>
    public abstract class BaseRenderPass : IRenderPass
    {
        public readonly IRendererAPI Renderer = GameModules.GetModule<RenderModule>().Context.GetRendererAPI();

        /// <summary>
        /// Handle the actual rendering logic for the pass.
        /// </summary>
        public abstract void OnRender();

        /// <summary>
        /// Flag(s) that will be enabled during this render pass.
        /// </summary>
        public abstract List<RenderFlag> RenderPassFlags { get; }

        /// <summary>
        /// Render entities of a specific type in a recursive manner.
        /// </summary>
        /// <typeparam name="TEntity">Type of entity to render.</typeparam>
        protected void RenderEntities<TEntity>(BaseEntity root) where TEntity : BaseEntity
        {
            if (root is TEntity entity)
            {
                RenderEntity(entity);
            }

            foreach (var child in root.Children)
            {
                RenderEntities<TEntity>(child);
            }
        }

        /// <summary>
        /// Define how to render individual entities.
        /// </summary>
        protected abstract void RenderEntity<TEntity>(TEntity entity) where TEntity : BaseEntity;
    }



}
