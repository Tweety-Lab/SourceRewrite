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
using System.Runtime.InteropServices.JavaScript;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
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

        // Dictionary of function names and their associated action callback
        private static Dictionary<string, Action> JSCallbacks = new Dictionary<string, Action>();

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

        public unsafe void RegisterEvent(string name, Action action)
        {
            JSCallbacks.Add(name, action);
            RegisterJSCallback(name, &InvokeCSharpCallback);
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
            view.UnlockJSContext();
        }

        // Unmanaged callback that bridges JavaScript to C#
        [UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
        private unsafe static JSValueRef InvokeCSharpCallback(JSContextRef ctx, JSObjectRef function, JSObjectRef thisObject, nuint argumentCount, JSValueRef* arguments, JSValueRef* exception)
        {
            // Convert the managed string "name" to a UTF-8 byte*
            byte[] nameBytes = System.Text.Encoding.UTF8.GetBytes("name");
            fixed (byte* namePtr = nameBytes)
            {
                // Create a JSStringRef from the UTF-8 byte*
                JSStringRef nameProperty = JavaScriptMethods.JSStringCreateWithUTF8CString(namePtr);

                // Get the "name" property of the JavaScript function
                JSValueRef functionNameValue = JavaScriptMethods.JSObjectGetProperty(ctx, function, nameProperty, exception);
                JavaScriptMethods.JSStringRelease(nameProperty);

                // Check if the "name" property is a string
                if (JavaScriptMethods.JSValueIsString(ctx, functionNameValue))
                {
                    // Convert the JS value to a JSStringRef
                    JSStringRef functionNameJS = JavaScriptMethods.JSValueToStringCopy(ctx, functionNameValue, exception);

                    // Get the maximum size required for the UTF-8 buffer
                    nuint bufferSize = JavaScriptMethods.JSStringGetMaximumUTF8CStringSize(functionNameJS);

                    // Allocate a byte buffer to hold the UTF-8 string
                    byte* buffer = (byte*)NativeMemory.Alloc(bufferSize);

                    // Copy the JS string into the byte buffer as a UTF-8 string
                    nuint actualLength = JavaScriptMethods.JSStringGetUTF8CString(functionNameJS, buffer, bufferSize);

                    // Convert the byte buffer to a managed string
                    string functionName = System.Text.Encoding.UTF8.GetString(buffer, (int)actualLength - 1); // Subtract 1 to exclude the null terminator

                    // Try get associated action
                    if (JSCallbacks.TryGetValue(functionName, out Action action))
                    {
                        action();
                    }

                    // Free the allocated buffer
                    NativeMemory.Free(buffer);

                    // Release the JSStringRef
                    JavaScriptMethods.JSStringRelease(functionNameJS);
                }
                else
                {
                    Console.WriteLine("The function does not have a name property or it is not a string.");
                }
            }

            // Return undefined (no return value)
            return JavaScriptMethods.JSValueMakeUndefined(ctx);
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
