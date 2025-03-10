using Silk.NET.Assimp;
using Silk.NET.Input;
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
using System.Runtime.InteropServices.JavaScript;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using UltralightNet;
using UltralightNet.AppCore;
using UltralightNet.JavaScript;
using UltralightNet.JavaScript.Low;

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
        private byte[] pixelBuffer;

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

            view = renderer.CreateView(actualWidth, actualHeight, viewConfig);

            // Pre-allocate the pixel buffer
            pixelBuffer = new byte[actualWidth * actualHeight * 4];

            view.OnFinishLoading += (_, _, _) =>
            {
                hasLoaded = true;
            };

            // Set HTML Contents
            view.HTML = HTML;

            RenderToTexture();
        }

        public unsafe void RegisterJSCallback(string functionName, delegate* unmanaged[Cdecl]<JSContextRef, JSObjectRef, JSObjectRef, nuint, JSValueRef*, JSValueRef*, JSValueRef> csharpFunc)
        {
            JSContextRef contextRef = view.LockJSContext();

            // Convert the string to a byte array using UTF-8 encoding
            byte[] byteArray = Encoding.UTF8.GetBytes(functionName);

            JSStringRef name;

            // Pin the byte array in memory to get a pointer
            fixed (byte* ptr = byteArray)
            {
                name = JavaScriptMethods.JSStringCreateWithUTF8CString(ptr);
            }

            // Create the JavaScript function that calls the C# method
            JSObjectRef func = JavaScriptMethods.JSObjectMakeFunctionWithCallback(
                contextRef,
                name,
                csharpFunc
            );

            JSObjectRef globalObj = JavaScriptMethods.JSContextGetGlobalObject(contextRef);

            JavaScriptMethods.JSObjectSetProperty(contextRef, globalObj, name, func, JSPropertyAttributes.None, null);

            JavaScriptMethods.JSStringRelease(name);
        }

        public void Update()
        {
            renderer.Update();

            if (view.NeedsPaint)
                RenderToTexture();
        }

        public void SendMouseButtonDown(MouseButton mouseButton)
        {
            ULMouseEvent mouseEvent = new ULMouseEvent();
            mouseEvent.Type = ULMouseEventType.MouseDown;

            mouseEvent.X = (int)InputSystem.Input.GetMouseX();
            mouseEvent.Y = (int)InputSystem.Input.GetMouseY();

            if (mouseButton == MouseButton.Left)
                mouseEvent.Button = ULMouseEventButton.Left;
            

            if (mouseButton == MouseButton.Right)
                mouseEvent.Button = ULMouseEventButton.Right;

            view.FireMouseEvent(mouseEvent);
        }

        public void SendMouseButtonUp(MouseButton mouseButton)
        {
            ULMouseEvent mouseEvent = new ULMouseEvent();
            mouseEvent.Type = ULMouseEventType.MouseUp;

            mouseEvent.X = (int)InputSystem.Input.GetMouseX();
            mouseEvent.Y = (int)InputSystem.Input.GetMouseY();

            if (mouseButton == MouseButton.Left)
                mouseEvent.Button = ULMouseEventButton.Left;


            if (mouseButton == MouseButton.Right)
                mouseEvent.Button = ULMouseEventButton.Right;

            view.FireMouseEvent(mouseEvent);
        }

        public void SendMousePosition(Vector2 position)
        {
            ULMouseEvent mouseEvent = new ULMouseEvent();
            mouseEvent.Type = ULMouseEventType.MouseMoved;

            mouseEvent.X = (int)position.X;
            mouseEvent.Y = (int)position.Y;

            view.FireMouseEvent(mouseEvent);
        }

        

        private unsafe void RenderToTexture()
        {
            while (!hasLoaded)
            {
                renderer.Update();
                Thread.Sleep(10);
            }

            renderer.Render();

            // Get Surface
            ULSurface surface = view.Surface ?? throw new Exception("Surface not found, did you perhaps set ViewConfig.IsAccelerated to true?");

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
