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
    /// Manages and applies lighting from all point light sources in the scene.
    /// </summary>
    public class LightingPass : BaseRenderPass
    {
        private readonly List<PointLight> _activeLights = new List<PointLight>();
        private const int MaxLights = 10; // Match the shader's array size

        public override List<RenderFlag> RenderPassFlags => new List<RenderFlag>();

        public override void OnRender()
        {
            // Clear previous frame's lights
            _activeLights.Clear();

            // Collect all active lights
            RenderEntities<PointLight>(EntityManager.Root);

            // Update shaders with all active lights
            UpdateShaderLighting();
        }

        protected override void RenderEntity<TEntity>(TEntity entity)
        {
            if (entity is PointLight lightEntity && _activeLights.Count < MaxLights)
            {
                _activeLights.Add(lightEntity);
            }
        }

        private void UpdateShaderLighting()
        {
            foreach (Shader shader in Shader.Shaders)
            {
                // Set the number of active lights
                shader.SetParameter("activeLights", _activeLights.Count);

                // Update each light in the array
                for (int i = 0; i < _activeLights.Count && i < MaxLights; i++)
                {
                    var light = _activeLights[i];

                    // Multiply the intensity (4th component of Color) by adjustment factor
                    Vector4 modifiedColor = light.Color;
                    modifiedColor.W *= 90000.0f;

                    // Normalize color from 0-255 to 0-1 range
                    Vector4 normalizedColor = modifiedColor / 255.0f;

                    // Set all light properties
                    string lightPrefix = $"lights[{i}]";
                    shader.SetParameter($"{lightPrefix}.position", light.Transform.Position);
                    shader.SetParameter($"{lightPrefix}.color", normalizedColor);
                    shader.SetParameter($"{lightPrefix}.attenuation",
                        new Vector3(light.ConstantAttenuation,
                                  light.LinearAttenuation,
                                  light.QuadraticAttenuation));
                }

                // Clear any remaining light slots in the array
                for (int i = _activeLights.Count; i < MaxLights; i++)
                {
                    string lightPrefix = $"lights[{i}]";
                    shader.SetParameter($"{lightPrefix}.position", Vector3.Zero);
                    shader.SetParameter($"{lightPrefix}.color", Vector4.Zero);
                    shader.SetParameter($"{lightPrefix}.attenuation", Vector3.One);
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
