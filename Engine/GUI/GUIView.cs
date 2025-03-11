using Silk.NET.Assimp;
using Silk.NET.Input;
using Silk.NET.Vulkan;
using SourceRewrite.AssetTypes;
using SourceRewrite.Components;
using SourceRewrite.Files;
using SourceRewrite.GUI.Scripting;
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
        public bool Visible = true;
        public View UltralightView;

        private Renderer renderer;
        private byte[] pixelBuffer;
        private GUIScriptingContext scriptingContext;

        private bool hasLoaded = false;

        public unsafe GUIView(string HTML, ULViewConfig viewConfig, int ResolutionScale, int height, int width)
        {

            // Set Font Loader
            AppCoreMethods.SetPlatformFontLoader();

            // Create Renderer
            var cfg = new ULConfig();
            renderer = ULPlatform.CreateRenderer(cfg);

            uint actualWidth = (uint)width * (uint)ResolutionScale;
            uint actualHeight = (uint)height * (uint)ResolutionScale;

            UltralightView = renderer.CreateView(actualWidth, actualHeight, viewConfig);

            // Pre-allocate the pixel buffer
            pixelBuffer = new byte[actualWidth * actualHeight * 4];

            UltralightView.OnFinishLoading += (_, _, _) =>
            {
                hasLoaded = true;
            };

            // Set HTML Contents
            UltralightView.HTML = HTML;

            // Load JS Scripting Context
            scriptingContext = new GUIScriptingContext();
            scriptingContext.GUIView = this;

            RenderToTexture();
        }

        // GUI Update
        public void Update()
        {
            // Only update/render if visible
            if (Visible)
            {
                renderer.Update();

                if (UltralightView.NeedsPaint)
                    RenderToTexture();
            }
        }

        /// <summary>
        /// Registers a C# Action that can be called from JavaScript.
        /// </summary>
        /// <param name="name">Javascript function name</param>
        /// <param name="action">C# Action</param>
        public unsafe void RegisterEvent(string name, Action action)
        {
            if (scriptingContext != null)
                scriptingContext.RegisterEvent(name, action);
        }

        /// <summary>
        /// Sends a mouse button down event to the GUI.
        /// </summary>
        /// <param name="mouseButton"></param>
        public void SendMouseButtonDown(MouseButton mouseButton)
        {
            if (!Visible) return; // Don't process input if the GUI is not visible

            ULMouseEvent mouseEvent = new ULMouseEvent();
            mouseEvent.Type = ULMouseEventType.MouseDown;

            mouseEvent.X = (int)InputSystem.Input.GetMouseX();
            mouseEvent.Y = (int)InputSystem.Input.GetMouseY();

            if (mouseButton == MouseButton.Left)
                mouseEvent.Button = ULMouseEventButton.Left;
            

            if (mouseButton == MouseButton.Right)
                mouseEvent.Button = ULMouseEventButton.Right;

            UltralightView.FireMouseEvent(mouseEvent);
        }

        /// <summary>
        /// Sends a mouse button up event to the GUI.
        /// </summary>
        /// <param name="mouseButton"></param>
        public void SendMouseButtonUp(MouseButton mouseButton)
        {
            if (!Visible) return; // Don't process input if the GUI is not visible

            ULMouseEvent mouseEvent = new ULMouseEvent();
            mouseEvent.Type = ULMouseEventType.MouseUp;

            mouseEvent.X = (int)InputSystem.Input.GetMouseX();
            mouseEvent.Y = (int)InputSystem.Input.GetMouseY();

            if (mouseButton == MouseButton.Left)
                mouseEvent.Button = ULMouseEventButton.Left;


            if (mouseButton == MouseButton.Right)
                mouseEvent.Button = ULMouseEventButton.Right;

            UltralightView.FireMouseEvent(mouseEvent);
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

            mouseEvent.X = (int)position.X;
            mouseEvent.Y = (int)position.Y;

            UltralightView.FireMouseEvent(mouseEvent);
        }

        
        /// Renders the GUI to a texture
        private unsafe void RenderToTexture()
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

            // Dispose of old texture
            if (Output != null)
                Output.Dispose();

            // Create new texture
            Output = new Rendering.Texture(pixelBuffer, bitmap.Height, bitmap.Width);

            bitmap.Dispose();
        }
    }
}
