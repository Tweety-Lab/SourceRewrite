using Silk.NET.Assimp;
using Silk.NET.Input;
using Silk.NET.Vulkan;
using SourceRewrite.AssetTypes;
using SourceRewrite.Components;
using SourceRewrite.Files;
using SourceRewrite.Objects;
using SourceRewrite.Windowing;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using UltralightNet;
using UltralightNet.AppCore;
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
