using System.Numerics;
using System.Runtime.InteropServices;
using UltralightNet;
using UltralightNet.AppCore;
using VistaGUI.Scripting;

namespace VistaGUI
{
    /// <summary>
    /// Renders HTML to a bgra32 byte array
    /// </summary>
    public class VistaView
    {
        /// <summary>
        /// BGRA32 byte array.
        /// </summary>
        public byte[] Output;

        /// <summary>
        /// Height of the view.
        /// </summary>
        public uint Height;

        /// <summary>
        /// Width of the view.
        /// </summary>
        public uint Width;

        /// <summary>
        /// Returns true if the view needs to be updated.
        /// </summary>
        public bool NeedsPaint => UltralightView.NeedsPaint;

        /// <summary>
        /// JavaScript context.
        /// </summary>
        public VistaScriptingContext ScriptingContext;

        public bool Visible = true;
        public View UltralightView;

        private Renderer renderer;
        private byte[] pixelBuffer;
        

        private bool hasLoaded = false;
        private Vector2 mousePosition = Vector2.Zero;

        public unsafe VistaView(string HTMLPath, GUIConfig viewConfig, int ResolutionScale, int height, int width)
        {
            // Set Font Loader
            AppCoreMethods.SetPlatformFontLoader();

            // Set resources path
            AppCoreMethods.ulEnablePlatformFileSystem(viewConfig.ResourcesPath);


            // Create Renderer
            var cfg = new ULConfig();
            renderer = ULPlatform.CreateRenderer(cfg);
            

            uint actualWidth = (uint)width * (uint)ResolutionScale;
            uint actualHeight = (uint)height * (uint)ResolutionScale;

            // Create an Ultralight View config from GUIConfig
            ULViewConfig config = new ULViewConfig();
            config.IsTransparent = viewConfig.IsTransparent;
            config.EnableJavaScript = viewConfig.EnableJavaScript;
            config.EnableImages = true;

            UltralightView = renderer.CreateView(actualWidth, actualHeight, config);


            // Pre-allocate the pixel buffer
            pixelBuffer = new byte[actualWidth * actualHeight * 4];

            UltralightView.OnFinishLoading += (_, _, _) =>
            {
                hasLoaded = true;
            };

            // Set View Contents
            UltralightView.URL = $"file:///{HTMLPath}";

            // Load JS Scripting Context
            ScriptingContext = new VistaScriptingContext();
            ScriptingContext.UltralightView = UltralightView;

            RenderOutput();

            // Register this view with the GUIContext
            VistaContext.Views.Add(this);
        }

        // GUI Update
        public void Update()
        {
            // Only update/render if visible
            if (Visible)
            {
                renderer.Update();

                if (UltralightView.NeedsPaint)
                    RenderOutput();
            }
        }


        /// <summary>
        /// Sends a mouse position event to the GUI.
        /// </summary>
        /// <param name="position"></param>
        public void SendMousePosition(Vector2 position)
        {
            if (!Visible) return; // Don't process input if the GUI is not visible

            ULMouseEvent mouseEvent = new ULMouseEvent();
            mouseEvent.Type = ULMouseEventType.MouseMoved;

            // Update mouse position
            mousePosition = position;

            mouseEvent.X = (int)mousePosition.X;
            mouseEvent.Y = (int)mousePosition.Y;

            UltralightView.FireMouseEvent(mouseEvent);
        }

        /// <summary>
        /// Sends a mouse button down event to the GUI.
        /// </summary>
        /// <param name="mouseButton"></param>
        public void SendMouseButtonDown(int mouseButton)
        {
            if (!Visible) return; // Don't process input if the GUI is not visible

            ULMouseEvent mouseEvent = new ULMouseEvent();
            mouseEvent.Type = ULMouseEventType.MouseDown;

            mouseEvent.X =  (int)mousePosition.X;
            mouseEvent.Y = (int)mousePosition.Y;

            if (mouseButton == 0)
                mouseEvent.Button = ULMouseEventButton.Left;

            if (mouseButton == 1)
                mouseEvent.Button = ULMouseEventButton.Right;

            UltralightView.FireMouseEvent(mouseEvent);
        }

        /// <summary>
        /// Sends a mouse button up event to the GUI.
        /// </summary>
        /// <param name="mouseButton"></param>
        public void SendMouseButtonUp(int mouseButton)
        {
            if (!Visible) return; // Don't process input if the GUI is not visible

            ULMouseEvent mouseEvent = new ULMouseEvent();
            mouseEvent.Type = ULMouseEventType.MouseUp;

            mouseEvent.X = (int)mousePosition.X;
            mouseEvent.Y = (int)mousePosition.Y;

            if (mouseButton == 0)
                mouseEvent.Button = ULMouseEventButton.Left;


            if (mouseButton == 1)
                mouseEvent.Button = ULMouseEventButton.Right;

            UltralightView.FireMouseEvent(mouseEvent);
        }

        /// Renders the GUI to a texture
        private unsafe void RenderOutput()
        {
            while (!hasLoaded)
            {
                renderer.Update();
                Thread.Sleep(10);
            }

            renderer.Render();

            // Get Surface
            ULSurface surface = UltralightView.Surface ?? throw new Exception("Surface not found, did you perhaps set ViewConfig.IsAccelerated to true?");

            // Get Bitmap
            ULBitmap bitmap = surface.Bitmap;
            uint dataSize = bitmap.Width * bitmap.Height * 4;

            // Copy the data from the IntPtr to the pre-allocated byte array
            byte* rawData = bitmap.RawPixels;
            Marshal.Copy((IntPtr)rawData, pixelBuffer, 0, (int)dataSize);

            // Create output
            Output = pixelBuffer;
            Height = bitmap.Height;
            Width = bitmap.Width;

            bitmap.Dispose();
        }
    }
}
