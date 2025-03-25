using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using UltralightNet;
using UltralightNet.JavaScript;
using UltralightNet.JavaScript.Low;
using VistaGUI.Scripting.Elements;
using VistaGUI.Scripting.References;

namespace VistaGUI.Scripting
{
    // A GUIView's JavaScript scripting context
    public class VistaScriptingContext
    {
        // Dictionary of function names and their associated action callback
        private static Dictionary<string, Action<string[]>> JSCallbacks = new Dictionary<string, Action<string[]>>();

        // Associated Ultralight View
        public View UltralightView;

        // Retrieves an element by its ID
        public VistaElement GetElement(string elementId)
        {
            return new VistaElement(elementId, this);
        }

        // Retrieves an element by its ID, returns a reference as a specific type (T).
        public T GetElementAsType<T>(string elementId) where T : VistaElement
        {
            // Get the tag name of the element
            string tagName = GetElementType(elementId);

            // Get all types in the current assembly that inherit from VistaElement
            var elementTypes = Assembly.GetAssembly(typeof(VistaElement))
                .GetTypes()
                .Where(type => typeof(VistaElement).IsAssignableFrom(type) && type.IsClass)
                .ToList();

            Type matchedType = null;

            // Search for a matching type based on tag names in the attribute
            foreach (var type in elementTypes)
            {
                var attribute = type.GetCustomAttribute<VistaElementAttribute>();
                if (attribute != null && attribute.TagNames.Contains(tagName.ToLower()))  // Match tag names
                {
                    matchedType = type;
                    break;
                }
            }

            // If no match is found handle the case
            if (matchedType == null)
            {
                Console.WriteLine($"No matching VistaElement found for tag name: {tagName}");
                return null;
            }

            // Ensure the requested type matches the resolved type
            if (typeof(T) != matchedType && !typeof(T).IsAssignableFrom(matchedType))
            {
                Console.WriteLine($"Cannot cast element {elementId} of tag name {tagName} to {typeof(T)}.");
                return null;
            }

            // Return the instance of the requested type, passing elementId to the constructor
            return (T)Activator.CreateInstance(matchedType, elementId, this);
        }

        // Adds an event to an element
        public void AddEvent(string elementID, string eventName, string functionName)
        {
            // Set the event handler directly as an attribute
            string jsCode = $@"
        document.getElementById('{elementID}')['{eventName}'] = function() {{
            var args = Array.prototype.slice.call(arguments);
            {functionName}(args);
        }};";
            UltralightView.EvaluateScript(jsCode, out string output);

            // Print output
            if (output != string.Empty)
                Console.WriteLine($"Vista Script: {output}");
        }

        // Sets the InnerHTML of an element
        public void SetElementInnerHTML(string elementId, string text)
        {
            // Escape single quotes, double quotes, and newlines for JavaScript
            string escapedText = text
                .Replace("\\", "\\\\")  // Escape backslashes first
                .Replace("'", "\\'")     // Escape single quotes
                .Replace("\"", "\\\"")   // Escape double quotes
                .Replace("\r", "\\r")    // Escape carriage returns
                .Replace("\n", "\\n");   // Escape newlines

            // Ensure the HTML content is passed directly as raw HTML
            string script = $"document.getElementById('{elementId}').innerHTML = '{escapedText}';";

            // Execute the JavaScript to set innerHTML
            UltralightView.EvaluateScript(script, out string output);

            // Print output
            if (output != string.Empty)
                Console.WriteLine($"Vista Script: {output} in {elementId} {escapedText}");
        }

        // Get the InnerHTML of an element
        public string GetElementInnerHTML(string elementId)
        {
            return UltralightView.EvaluateScript($"document.getElementById('{elementId}').innerHTML;", out _);
        }

        // Sets a property of an element
        public void SetElementProperty(string elementId, string property, string value)
        {
            // Escape single quotes, double quotes, and newlines for JavaScript
            string escapedValue = value
                .Replace("\\", "\\\\")  // Escape backslashes first
                .Replace("'", "\\'")     // Escape single quotes
                .Replace("\"", "\\\"")   // Escape double quotes
                .Replace("\r", "\\r")    // Escape carriage returns
                .Replace("\n", "\\n");   // Escape newlines

            UltralightView.EvaluateScript($"document.getElementById('{elementId}').{property} = '{escapedValue}';", out string output);

            // Print output
            if (output != string.Empty)
                Console.WriteLine($"Vista Script: {output}");
        }


        // Gets a property of an element
        public string GetElementProperty(string elementId, string property)
        {
            return UltralightView.EvaluateScript($"document.getElementById('{elementId}').{property};", out _);
        }

        // Sets the style of an element
        public void SetElementStyle(string elementId, string style)
        {
            string escapedStyle = style.Replace("'", "\\'").Replace("\"", "\\\"");
            UltralightView.EvaluateScript($"document.getElementById('{elementId}').style.cssText = '{escapedStyle}';", out string output);

            // Print output
            if (output != string.Empty)
                Console.WriteLine($"Vista Script: {output}");
        }


        // Gets the syle of an element
        public string GetElementStyle(string elementId)
        {
            return UltralightView.EvaluateScript($"document.getElementById('{elementId}').style.cssText;", out _);
        }

        // Gets the type of an element
        public string GetElementType(string elementId)
        {
            return UltralightView.EvaluateScript($"document.getElementById('{elementId}').tagName;", out _).ToLower();
        }

        /// <summary>
        /// Registers a C# Action that can be called from JavaScript.
        /// </summary>
        /// <param name="name">JavaScript function name</param>
        /// <param name="action">C# Action (optional arguments)</param>
        public unsafe void RegisterEvent(string name, Action action)
        {
            // Wrap the user's action in a lambda that ignores arguments
            JSCallbacks.Add(name, (args) => action());
            RegisterJSCallback(name, &InvokeCSharpCallback);
        }

        /// <summary>
        /// Registers a C# Action that can be called from JavaScript with arguments.
        /// </summary>
        /// <param name="name">JavaScript function name</param>
        /// <param name="action">C# Action that accepts arguments</param>
        public unsafe void RegisterEvent(string name, Action<string[]> action)
        {
            JSCallbacks.Add(name, action);
            RegisterJSCallback(name, &InvokeCSharpCallback);
        }

        /// <summary>
        /// Unregisters a C# Action that can be called from JavaScript.
        /// </summary>
        /// <param name="name">Javascript function name</param>
        public unsafe void UnregisterEvent(string name)
        {
            if (JSCallbacks.Remove(name))
            {
                JSContextRef contextRef = UltralightView.LockJSContext();

                // Convert the string to a byte array using UTF-8 encoding
                byte[] byteArray = Encoding.UTF8.GetBytes(name);

                JSStringRef nameRef;

                // Pin the byte array in memory to get a pointer
                fixed (byte* ptr = byteArray)
                {
                    nameRef = JavaScriptMethods.JSStringCreateWithUTF8CString(ptr);
                }

                // Get and remove the global object
                JSObjectRef globalObj = JavaScriptMethods.JSContextGetGlobalObject(contextRef);
                JavaScriptMethods.JSObjectDeleteProperty(contextRef, globalObj, nameRef);

                // Release the JSStringRef
                JavaScriptMethods.JSStringRelease(nameRef);

                UltralightView.UnlockJSContext();
            }
        }

        // Registers a C# function that can be called from JavaScript
        private unsafe void RegisterJSCallback(string functionName, delegate* unmanaged[Cdecl]<JSContextRef, JSObjectRef, JSObjectRef, nuint, JSValueRef*, JSValueRef*, JSValueRef> csharpFunc)
        {
            JSContextRef contextRef = UltralightView.LockJSContext();

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
            UltralightView.UnlockJSContext();
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
                    if (JSCallbacks.TryGetValue(functionName, out Action<string[]> action))
                    {
                        // Extract arguments from JavaScript and convert them to strings
                        string[] args = new string[argumentCount];
                        for (nuint i = 0; i < argumentCount; i++)
                        {
                            JSValueRef argValue = arguments[i];

                            // Convert the argument to a string
                            JSStringRef argJS = JavaScriptMethods.JSValueToStringCopy(ctx, argValue, exception);
                            nuint argBufferSize = JavaScriptMethods.JSStringGetMaximumUTF8CStringSize(argJS);
                            byte* argBuffer = (byte*)NativeMemory.Alloc(argBufferSize);
                            nuint argActualLength = JavaScriptMethods.JSStringGetUTF8CString(argJS, argBuffer, argBufferSize);
                            args[i] = System.Text.Encoding.UTF8.GetString(argBuffer, (int)argActualLength - 1); // Subtract 1 to exclude the null terminator

                            // Free the allocated buffer and release the JSStringRef
                            NativeMemory.Free(argBuffer);
                            JavaScriptMethods.JSStringRelease(argJS);
                        }

                        // Invoke the action with the arguments
                        action(args);
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
