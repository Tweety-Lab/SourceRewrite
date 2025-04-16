using SourceRewrite.Entities.Env;
using SourceRewrite.Entities.GUI;
using SourceRewrite.Entities.Lighting;
using SourceRewrite.Entities.Point;
using SourceRewrite.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using SourceRewrite.AssetTypes;

namespace SourceRewrite.Rendering
{
    /// <summary>
    /// Render Brush and Point Entities that are Opaque.
    /// </summary>
    public class OpaquePass : BaseRenderPass
    {
        // Enable Depth-Testing and Culling
        public override List<RenderFlag> RenderPassFlags => new List<RenderFlag> { RenderFlag.DepthTest, RenderFlag.CullBackFaces };

        public override void OnRender() => RenderEntities<BaseEntity>(EntityManager.Root);

        protected override void RenderEntity<TEntity>(TEntity entity)
        {
            if (entity is MeshEntity meshEntity && entity is not EnvSprite && entity is not EnvBeam)
            {
                if (!ShouldRenderMaterial(meshEntity.Mesh.Material))
                    return;

                var modelMatrix = RendererContext.GetEntityModelMatrix(meshEntity) ?? Matrix4x4.Identity;
                Renderer.RenderMesh(meshEntity.Mesh, modelMatrix);
            }
            else if (entity is BrushEntity brushEntity)
            {
                var modelMatrix = Matrix4x4.Identity;

                foreach (Mesh mesh in brushEntity.Brush)
                {
                    if (!ShouldRenderMaterial(mesh.Material))
                        continue;

                    Renderer.RenderMesh(mesh, modelMatrix);
                }
            }
        }

        // Determinse if a material should be rendered in opaque pass
        private bool ShouldRenderMaterial(Material material)
        {
            // COMPILE TIME OPTIMIZATION
            // TODO: Move this to VBSP
            if (material.GetFlag("compilenodraw") == 1)
                return false;

            if (material.GetFlag("alphatest") == 1)
                return false;

            if (material.GetFlag("translucent") == 1)
                return false;

            if (material.GetFlag("compiletrigger") == 1)
                return ConCommands.RenderTriggers; // Allow to toggle trigger visibilty with showtriggers_toggle

            return true;
        }
    }

    /// <summary>
    /// Render Brush and Point Entities that are Translucent.
    /// </summary>
    public class TranslucentPass : BaseRenderPass
    {
        // Enable Depth-Testing and Culling
        public override List<RenderFlag> RenderPassFlags => new List<RenderFlag> { RenderFlag.DepthTest, RenderFlag.CullBackFaces, RenderFlag.Blend };

        public override void OnRender() => RenderEntities<BaseEntity>(EntityManager.Root);

        protected override void RenderEntity<TEntity>(TEntity entity)
        {
            if (entity is MeshEntity meshEntity && entity is not EnvSprite && entity is not EnvBeam)
            {
                if (!ShouldRenderMaterial(meshEntity.Mesh.Material))
                    return;

                var modelMatrix = RendererContext.GetEntityModelMatrix(meshEntity) ?? Matrix4x4.Identity;
                Renderer.RenderMesh(meshEntity.Mesh, modelMatrix);
            }
            else if (entity is BrushEntity brushEntity)
            {
                var modelMatrix = Matrix4x4.Identity;

                foreach (Mesh mesh in brushEntity.Brush)
                {
                    if (!ShouldRenderMaterial(mesh.Material))
                        continue;

                    Renderer.RenderMesh(mesh, modelMatrix);
                }
            }
        }

        // Determinse if a material should be rendered in translucent pass
        private bool ShouldRenderMaterial(Material material)
        {
            // COMPILE TIME OPTIMIZATION
            // TODO: Move this to VBSP
            if (material.GetFlag("compilenodraw") == 1)
                return false;

            if (material.GetFlag("compiletrigger") == 1)
                return ConCommands.RenderTriggers; // Allow to toggle trigger visibilty with showtriggers_toggle

            if (material.GetFlag("alphatest") == 1)
                return true;

            if (material.GetFlag("translucent") == 1)
                return true;

            return false;
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
