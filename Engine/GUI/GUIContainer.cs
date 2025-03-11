using VistaGUI;

namespace SourceRewrite.GUI
{
    /// <summary>
    /// Renders GUI to a texture.
    /// </summary>
    public class GUIContainer
    {
        /// <summary>
        /// Texture the GUI renders to.
        /// </summary>
        public Rendering.Texture Output;
        public GUIView VistaView;
        
        /// Renders the GUI Output to a texture
        public unsafe Rendering.Texture RenderToTexture()
        {
            // Dispose of old texture
            if (Output != null)
                Output.Dispose();

            // Create new texture
            Output = new Rendering.Texture(VistaView.Output, VistaView.Height, VistaView.Width);
            return Output;
        }
    }
}
