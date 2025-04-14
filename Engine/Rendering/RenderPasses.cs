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


    /// <summary>
    /// Render MeshEntities that are Opaque.
    /// </summary>
    public class OpaquePass : BaseRenderPass
    {
        // Enable Depth-Testing and Culling
        public override List<RenderFlag> RenderPassFlags => new List<RenderFlag> { RenderFlag.DepthTest, RenderFlag.CullBackFaces };

        public override void OnRender() => RenderEntities<MeshEntity>(EntityManager.Root);

        protected override void RenderEntity<TEntity>(TEntity entity)
        {
            if (entity is MeshEntity meshEntity && entity is not EnvSprite && entity is not EnvBeam) // Make sure not to also render billboard entities REPLACE THIS
            {
                var modelMatrix = RendererContext.GetEntityModelMatrix(meshEntity) ?? Matrix4x4.Identity;
                Renderer.RenderMesh(meshEntity.Mesh, modelMatrix);
            }
        }
    }

    public class OpaqueBrushEntityPass : BaseRenderPass
    {
        // Enable Depth-Testing and Culling
        public override List<RenderFlag> RenderPassFlags => new List<RenderFlag> { RenderFlag.DepthTest, RenderFlag.CullBackFaces };

        public override void OnRender() => RenderEntities<BrushEntity>(EntityManager.Root);

        protected override void RenderEntity<TEntity>(TEntity entity)
        {
            if (entity is BrushEntity brushEntity && entity is not EnvSprite && entity is not EnvBeam) // Make sure not to also render billboard entities REPLACE THIS
            {
                var modelMatrix = Matrix4x4.Identity;

                foreach (Mesh mesh in brushEntity.Brush)
                {
                    Renderer.RenderMesh(mesh, modelMatrix);
                }
            }
        }
    }

    /// <summary>
    /// Manages and applies lighting from all light sources in the scene.
    /// </summary>
    public class LightingPass : BaseRenderPass
    {
        private readonly List<object> _activeLights = new List<object>(); // Stores lights
        private const int MaxLights = 10; // Match the shader's array size

        public override List<RenderFlag> RenderPassFlags => new List<RenderFlag>();

        public override void OnRender()
        {
            // Clear previous frame's lights
            _activeLights.Clear();

            // Collect all active lights of all types
            RenderEntities<Light>(EntityManager.Root);
            RenderEntities<LightSpot>(EntityManager.Root);
            RenderEntities<LightDirectional>(EntityManager.Root);

            // Update shaders with all active lights
            UpdateShaderLighting();
        }

        protected override void RenderEntity<TEntity>(TEntity entity)
        {
            if ((entity is Light || entity is LightSpot || entity is LightDirectional) && _activeLights.Count < MaxLights)
            {
                _activeLights.Add(entity);
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
                    string lightPrefix = $"lights[{i}]";

                    if (_activeLights[i] is Light light)
                    {
                        // Handle regular point light
                        Vector4 modifiedColor = light.Color;
                        modifiedColor.W *= 90000.0f;
                        Vector4 normalizedColor = modifiedColor / 255.0f;

                        shader.SetParameter($"{lightPrefix}.position", light.Transform.Position);
                        shader.SetParameter($"{lightPrefix}.color", normalizedColor);
                        shader.SetParameter($"{lightPrefix}.attenuation",
                            new Vector3(light.ConstantAttenuation,
                                      light.LinearAttenuation,
                                      light.QuadraticAttenuation));

                        // Set point light defaults
                        shader.SetParameter($"{lightPrefix}.lightType", 0);
                        shader.SetParameter($"{lightPrefix}.direction", Vector3.Zero);
                        shader.SetParameter($"{lightPrefix}.cutOff", 0.0f);
                        shader.SetParameter($"{lightPrefix}.outerCutOff", 0.0f);
                    }
                    else if (_activeLights[i] is LightSpot lightSpot)
                    {
                        // Handle spotlight
                        Vector4 modifiedColor = lightSpot.Color;
                        modifiedColor.W *= 90000.0f;
                        Vector4 normalizedColor = modifiedColor / 255.0f;

                        shader.SetParameter($"{lightPrefix}.position", lightSpot.Transform.Position);
                        shader.SetParameter($"{lightPrefix}.color", normalizedColor);
                        shader.SetParameter($"{lightPrefix}.attenuation",
                            new Vector3(lightSpot.ConstantAttenuation,
                                      lightSpot.LinearAttenuation,
                                      lightSpot.QuadraticAttenuation));

                        // Set spotlight specific properties
                        shader.SetParameter($"{lightPrefix}.lightType", 1);
                        shader.SetParameter($"{lightPrefix}.direction", lightSpot.Transform.Forward);
                        shader.SetParameter($"{lightPrefix}.cutOff", MathF.Cos(Math.DegreesToRadians(lightSpot.InnerConeAngle)));
                        shader.SetParameter($"{lightPrefix}.outerCutOff", MathF.Cos(Math.DegreesToRadians(lightSpot.OuterConeAngle)));
                    }
                    else if (_activeLights[i] is LightDirectional lightDirectional)
                    {
                        // Handle directional light
                        Vector4 modifiedColor = lightDirectional.Color;
                        Vector4 normalizedColor = modifiedColor / 255.0f;
                        shader.SetParameter($"{lightPrefix}.color", normalizedColor);
                        shader.SetParameter($"{lightPrefix}.lightType", 2);
                        shader.SetParameter($"{lightPrefix}.direction", lightDirectional.Transform.Forward);
                    }
                }

                // Clear any remaining light slots in the array
                for (int i = _activeLights.Count; i < MaxLights; i++)
                {
                    string lightPrefix = $"lights[{i}]";
                    shader.SetParameter($"{lightPrefix}.position", Vector3.Zero);
                    shader.SetParameter($"{lightPrefix}.color", Vector4.Zero);
                    shader.SetParameter($"{lightPrefix}.attenuation", Vector3.One);
                    shader.SetParameter($"{lightPrefix}.lightType", 0);
                    shader.SetParameter($"{lightPrefix}.direction", Vector3.Zero);
                    shader.SetParameter($"{lightPrefix}.cutOff", 0.0f);
                    shader.SetParameter($"{lightPrefix}.outerCutOff", 0.0f);
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

    /// <summary>
    /// Render all entities that require billboarding.
    /// </summary>
    public class BillboardPass : BaseRenderPass
    {
        // Enable Blending and Depth-Testing
        public override List<RenderFlag> RenderPassFlags => new List<RenderFlag> { RenderFlag.Blend, RenderFlag.DepthTest };

        public override void OnRender() => RenderEntities<MeshEntity>(EntityManager.Root);

        protected override void RenderEntity<TEntity>(TEntity entity)
        {
            if (entity is EnvSprite spriteEntity)
            {
                // Get Model Matrix
                var modelMatrix = RendererContext.GetEntityModelMatrix(spriteEntity) ?? Matrix4x4.Identity;

                // Get Active Camera
                PointCamera camera = PointCamera.ActiveCamera;

                // Make model matrix face the camera
                modelMatrix = Matrix4x4.CreateFromQuaternion(camera.Transform.Rotation) * modelMatrix;

                // Render the sprite
                Renderer.RenderMesh(spriteEntity.Mesh, modelMatrix);
            }
            if (entity is EnvBeam beamEntity)
            {
                // Get Model Matrix
                var modelMatrix = RendererContext.GetEntityModelMatrix(beamEntity) ?? Matrix4x4.Identity;

                // Render the beam with the updated model matrix
                Renderer.RenderMesh(beamEntity.Mesh, modelMatrix);
            }
        }
    }
}
