// JSBase.h

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UltralightNet.JavaScript.Low;
using UltralightNet.LowStuff;

namespace UltralightNet.JavaScript
{
	namespace Low
	{
		[SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Those are 1:1 definitions")]
		[SuppressMessage("Interoperability", "CA1401:P/Invokes should not be visible", Justification = "Compatibility")]
		public static unsafe partial class JavaScriptMethods
		{
			static JavaScriptMethods() => Methods.Preload();

			const string LibWebCore = "WebCore";

			internal static void ThrowUnsupportedConstructor() => throw new NotSupportedException("Constructor is unsupported");
			internal static Exception UnsupportedMethodException => new NotSupportedException("Method is unsupported");

			[LibraryImport(LibWebCore)]
			public static partial JSValueRef JSEvaluateScript(JSContextRef context, JSStringRef script, JSObjectRef thisObject, JSStringRef sourceURL, int startingLineNumber, JSValueRef* exception = null);

			[LibraryImport(LibWebCore)]
			[return: MarshalAs(UnmanagedType.U1)]
			public static partial bool JSCheckScriptSyntax(JSContextRef context, JSStringRef script, JSStringRef sourceURL, int startingLineNumber, JSValueRef* exception = null);

			[LibraryImport(LibWebCore)]
			public static partial void JSGarbageCollect(JSContextRef context);
		}
	}
	namespace LowStuff
	{
		public abstract unsafe class JSNativeContainer<NativeHandle> : NativeContainer where NativeHandle : unmanaged
		{
			public NativeHandle JSHandle
			{
				get => Methods.BitCast<nuint, NativeHandle>((nuint)Handle);
				protected init => Handle = (void*)Methods.BitCast<NativeHandle, nuint>(value);
			}
		}
	}
}
