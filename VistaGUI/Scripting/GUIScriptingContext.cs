using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using UltralightNet;
using UltralightNet.JavaScript;
using UltralightNet.JavaScript.Low;

namespace VistaGUI.Scripting
{
    // A GUIView's JavaScript scripting context
    public class GUIScriptingContext
    {
        // Dictionary of function names and their associated action callback
        private static Dictionary<string, Action> JSCallbacks = new Dictionary<string, Action>();

        // Associated GUI View
        public GUIView GUIView;

        /// <summary>
        /// Registers a C# Action that can be called from JavaScript.
        /// </summary>
        /// <param name="name">Javascript function name</param>
        /// <param name="action">C# Action</param>
        public unsafe void RegisterEvent(string name, Action action)
        {
            JSCallbacks.Add(name, action);
            RegisterJSCallback(name, &InvokeCSharpCallback);
        }

        // Registers a C# function that can be called from JavaScript
        private unsafe void RegisterJSCallback(string functionName, delegate* unmanaged[Cdecl]<JSContextRef, JSObjectRef, JSObjectRef, nuint, JSValueRef*, JSValueRef*, JSValueRef> csharpFunc)
        {
            JSContextRef contextRef = GUIView.UltralightView.LockJSContext();

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
            GUIView.UltralightView.UnlockJSContext();
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
    }
}
