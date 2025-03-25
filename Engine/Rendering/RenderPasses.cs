using SourceRewrite.Entities;
using SourceRewrite.Entities.GUI;
using SourceRewrite.Windowing;
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
        Blend
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
            var rendererAPI = GameWindow.CurrentWindow.Renderer.GetRendererAPI();
            foreach (var flag in flags)
            {
                rendererAPI.EnableFlag(flag);
            }
        }

        private static void RevertFlags(IEnumerable<RenderFlag> flags)
        {
            var rendererAPI = GameWindow.CurrentWindow.Renderer.GetRendererAPI();
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
        public readonly IRendererAPI Renderer = GameWindow.CurrentWindow.Renderer.GetRendererAPI();

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


    /// <summary>
    /// Render MeshEntities that are Opaque.
    /// </summary>
    public class OpaquePass : BaseRenderPass
    {
        // Enable Depth-Testing
        public override List<RenderFlag> RenderPassFlags => new List<RenderFlag> { RenderFlag.DepthTest };

        public override void OnRender() => RenderEntities<MeshEntity>(EntityManager.Root);

        protected override void RenderEntity<TEntity>(TEntity entity)
        {
            if (entity is MeshEntity meshEntity)
            {
                var modelMatrix = RendererContext.GetEntityModelMatrix(meshEntity) ?? Matrix4x4.Identity;
                Renderer.RenderMesh(meshEntity.Mesh, modelMatrix);
            }
        }
    }

    /// <summary>
    /// Apply Lighting.
    /// </summary>
    public class LightingPass : BaseRenderPass
    {
        public override List<RenderFlag> RenderPassFlags => new List<RenderFlag> { };

        public override void OnRender() => RenderEntities<PointLight>(EntityManager.Root);

        protected override void RenderEntity<TEntity>(TEntity entity)
        {
            if (entity is PointLight lightEntity)
            {
                // Multiply the intensity (4th component of Color) by arbitrary adjustment factor for our unit system
                Vector4 modifiedColor = lightEntity.Color;
                modifiedColor.W *= 90000.0f;

                // Update Uniforms
                foreach (Shader shader in Shader.Shaders)
                {
                    shader.SetParameter("light_position", lightEntity.Transform.Position);
                    shader.SetParameter("light_color", modifiedColor / 255.0f); // Convert Color from 1-255 range to 0-1 range
                    shader.SetParameter("light_attenuation", new Vector3(lightEntity.ConstantAttenuation, lightEntity.LinearAttenuation, lightEntity.QuadraticAttenuation));
                }
            }
        }
    }

    /// <summary>
    /// Render GUICanvasEntities.
    /// </summary>
    public class ScreenspaceGUIRenderPass : BaseRenderPass
    {
        // Enable Blending
        public override List<RenderFlag> RenderPassFlags => new List<RenderFlag> { RenderFlag.Blend };

        public override void OnRender() => RenderEntities<GUICanvasEntity>(EntityManager.Root);

        protected override void RenderEntity<TEntity>(TEntity entity)
        {
            if (entity is GUICanvasEntity guiEntity)
            {
                Renderer.RenderScreenspaceGUI(guiEntity);
            }
        }
    }
}
