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
        private static readonly List<IRenderPass> RenderPasses = GetRenderPasses();

        public static void RenderAllPasses()
        {
            foreach (var pass in RenderPasses)
            {
                pass.OnRender();
            }
        }

        private static List<IRenderPass> GetRenderPasses()
        {
            return Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(t => typeof(IRenderPass).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract)
                .Select(t => (IRenderPass)Activator.CreateInstance(t))
                .Where(pass => pass != null)
                .ToList();
        }
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

    // Mesh Class with UV generation
    public static class MeshExtensions
    {
        public static float[] GenerateDefaultUVs(this Mesh meshObject)
        {
            var uvs = new float[meshObject.Vertices.Length / 3 * 2];

            for (int i = 0; i < meshObject.Vertices.Length; i += 12) // Assuming 4 vertices per face
            {
                var uv1 = new Vector2(0, 0);
                var uv2 = new Vector2(1, 0);
                var uv3 = new Vector2(1, 1);
                var uv4 = new Vector2(0, 1);

                int baseUvIndex = (i / 3) * 2;

                // Apply UVs to each vertex in the face
                uvs[baseUvIndex] = uv1.X;
                uvs[baseUvIndex + 1] = uv1.Y;

                uvs[baseUvIndex + 2] = uv2.X;
                uvs[baseUvIndex + 3] = uv2.Y;

                uvs[baseUvIndex + 4] = uv3.X;
                uvs[baseUvIndex + 5] = uv3.Y;

                uvs[baseUvIndex + 6] = uv4.X;
                uvs[baseUvIndex + 7] = uv4.Y;
            }

            return uvs;
        }
    }
}
