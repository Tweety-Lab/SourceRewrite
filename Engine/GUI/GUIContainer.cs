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
        public VistaView VistaView;

        /// Renders the GUI Output to a texture
        public unsafe Rendering.Texture RenderToTexture()
        {
            // Dispose of old texture
            if (Output != null)
                Output.Dispose();

            // Create new texture only if the view is visible
            if (VistaView.Visible)
            {
                Output = new Rendering.Texture(VistaView.Output, VistaView.Height, VistaView.Width);
            }
            else
            {
                // Create a blank texture when invisible
                Output = CreateBlankTexture();
            }

            return Output;
        }

        /// <summary>
        /// Clears the current texture
        /// </summary>
        public void ClearTexture()
        {
            if (Output != null)
            {
                Output.Dispose();
                Output = CreateBlankTexture();
            }
        }

        /// <summary>
        /// Creates a blank transparent texture
        /// </summary>
        private Rendering.Texture CreateBlankTexture()
        {
            // Create a 1x1 transparent texture
            byte[] blankData = new byte[4] { 0, 0, 0, 0 }; // RGBA
            return new Rendering.Texture(blankData, 1, 1);
        }
    }
}
