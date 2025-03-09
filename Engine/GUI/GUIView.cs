using Silk.NET.Assimp;
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
using System.Text;
using System.Threading.Tasks;
using UltralightNet;
using UltralightNet.AppCore;

namespace SourceRewrite.GUI
{
    /// <summary>
    /// Renders GUI to a texture.
    /// </summary>
    public class GUIView
    {
        /// <summary>
        /// Texture the GUI renders to.
        /// </summary>
        public Rendering.Texture Output;

        private Renderer renderer;
        private View view;

        private bool hasLoaded = false;
        public GUIView(string HTML, int ResolutionScale, int height, int width)
        {
            // Set Font Loader
            AppCoreMethods.SetPlatformFontLoader();

            // Create Renderer
            var cfg = new ULConfig();
            renderer = ULPlatform.CreateRenderer(cfg);

            // View Config
            var viewConfig = new ULViewConfig();
            viewConfig.IsTransparent = true; // Enable transparency

            view = renderer.CreateView((uint)width * (uint)ResolutionScale, (uint)height * (uint)ResolutionScale, viewConfig);

            view.OnFinishLoading += (_, _, _) =>
            {
                hasLoaded = true;
            };

            // Set HTML Contents
            view.HTML = HTML;

            Output = RenderToTexture();
        }

        public void Update()
        {
            renderer.Update();
        }

        private unsafe Rendering.Texture RenderToTexture()
        {
            while (!hasLoaded)
            {
                Update();
                Thread.Sleep(10);
            }

            renderer.Render();

            // Get Surface
            ULSurface surface = view.Surface ?? throw new Exception("Surface not found, did you perhaps set ViewConfig.IsAccelerated to true?");

            // Get Bitmap
            ULBitmap bitmap = surface.Bitmap;

            uint dataSize = bitmap.Width * bitmap.Height * 4; // 4 bytes per pixel

            // Create a byte array to hold the pixel data
            byte[] byteArray = new byte[dataSize];

            byte* rawData = bitmap.RawPixels;

            // Copy the data from the IntPtr to the byte array
            Marshal.Copy((IntPtr)rawData, byteArray, 0, (int)dataSize);

            Output = new Rendering.Texture(byteArray, bitmap.Height, bitmap.Width);

            // Save bitmap to png file
            var path = Path.GetDirectoryName(typeof(Program).Assembly.Location)!;
            bitmap.WritePng(Path.Combine(path, "OUTPUT.png"));

            return Output;
        }
    }
}
