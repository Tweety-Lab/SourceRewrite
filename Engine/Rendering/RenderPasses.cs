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
    /// Base class for geometry rendering passes.
    /// </summary>
    public abstract class GeometryPass : BaseRenderPass
    {
        public override void OnRender() => RenderEntities<BaseEntity>(EntityManager.Root);

        protected override void RenderEntity<TEntity>(TEntity entity)
        {
            switch (entity)
            {
                case MeshEntity meshEntity when !(entity is EnvSprite) && !(entity is EnvBeam):
                    RenderMeshEntity(meshEntity);
                    break;

                case BrushEntity brushEntity:
                    RenderBrushEntity(brushEntity);
                    break;
            }
        }

        private void RenderMeshEntity(MeshEntity meshEntity)
        {
            if (!ShouldRenderMaterial(meshEntity.Mesh.Material))
                return;

            var modelMatrix = RendererContext.GetEntityModelMatrix(meshEntity) ?? Matrix4x4.Identity;
            Renderer.RenderMesh(meshEntity.Mesh, modelMatrix);
        }

        private void RenderBrushEntity(BrushEntity brushEntity)
        {
            foreach (Mesh mesh in brushEntity.Brush)
            {
                if (!ShouldRenderMaterial(mesh.Material))
                    continue;

                Renderer.RenderMesh(mesh, Matrix4x4.Identity);
            }
        }

        protected abstract bool ShouldRenderMaterial(Material material);
    }

    /// <summary>
    /// Render Brushes and Entities that are Opaque.
    /// </summary>
    public class OpaquePass : GeometryPass
    {
        public override List<RenderFlag> RenderPassFlags => new List<RenderFlag> {
            RenderFlag.DepthTest,
            RenderFlag.CullBackFaces
        };

        protected override bool ShouldRenderMaterial(Material material)
        {
            if (material.GetFlag("compilenodraw") == 1) return false;
            if (material.GetFlag("compiletrigger") == 1) return false;

            return material.GetFlag("alphatest") != 1 &&
                   material.GetFlag("translucent") != 1;
        }
    }

    /// <summary>
    /// Render Brushes and Entities that are Transparent.
    /// </summary>
    public class TransparentPass : GeometryPass
    {
        public override List<RenderFlag> RenderPassFlags => new List<RenderFlag> {
            RenderFlag.DepthTest,
            RenderFlag.CullBackFaces,
            RenderFlag.Blend
        };

        protected override bool ShouldRenderMaterial(Material material)
        {
            if (material.GetFlag("compilenodraw") == 1) return false;
            if (material.GetFlag("compiletrigger") == 1) return ConCommands.RenderTriggers;

            return material.GetFlag("alphatest") == 1 ||
                   material.GetFlag("translucent") == 1;
        }
    }

    /// <summary>
    /// Manages and applies lighting from all light sources in the scene.
    /// </summary>
    public class LightingPass : BaseRenderPass
    {
        private readonly List<ILight> _activeLights = new List<ILight>(); // Stores lights
        private const int MaxLights = 10; // Match the shader's array size

        public override List<RenderFlag> RenderPassFlags => new List<RenderFlag>();

        public override void OnRender()
        {
            // Clear previous frame's lights
            _activeLights.Clear();

            // Render
            RenderEntities<PointEntity>(EntityManager.Root);

            // Update shaders with all active lights
            UpdateShaderLighting();
        }

        protected override void RenderEntity<TEntity>(TEntity entity)
        {
            if (entity is ILight light && _activeLights.Count < MaxLights)
            {
                _activeLights.Add(light);
            }
        }

        public bool LightsDirty = true;
        private void UpdateShaderLighting()
        {
            // Only Update if lights have changed
            if (!LightsDirty) return;

            foreach (Shader shader in Shader.Shaders)
            {
                shader.SetParameter("activeLights", _activeLights.Count);

                for (int i = 0; i < _activeLights.Count && i < MaxLights; i++)
                {
                    _activeLights[i].ApplyToShader(shader, i);
                }

                // Clear remaining light slots
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

            // Mark lights as clean
            LightsDirty = false;
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
