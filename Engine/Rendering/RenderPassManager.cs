using SourceRewrite.Windowing.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SourceRewrite.Rendering
{
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

        /// <summary>
        /// Get a render pass from it's type.
        /// </summary>
        /// <param name="type"></param>
        public static T GetRenderPass<T>() where T : IRenderPass =>
            (T)RenderPasses.FirstOrDefault(x => x is T);

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
}
