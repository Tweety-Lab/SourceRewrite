using System.Runtime.CompilerServices;
using UltralightNet.JavaScript.Low;

namespace UltralightNet.JavaScript;

public unsafe struct JSStaticValue
{
	public byte* name;
	public delegate* unmanaged[Cdecl]<void*, void*, void*, void**, void*> getProperty;
	public delegate* unmanaged[Cdecl]<void*, void*, void*, void*, void**, bool> setProperty;
	private uint _Attributes;
	public JSPropertyAttributes Attributes { readonly get => Methods.BitCast<uint, JSPropertyAttributes>(_Attributes); set => _Attributes = Methods.BitCast<JSPropertyAttributes, uint>(value); }
}
public unsafe struct JSStaticValueEx
{
	public byte* name;
	public delegate* unmanaged[Cdecl]<void*, void*, void*, void*, void**, void*> getPropertyEx;
	public delegate* unmanaged[Cdecl]<void*, void*, void*, void*, void*, void**, bool> setPropertyEx;
	private uint _Attributes;
	public JSPropertyAttributes Attributes { readonly get => Methods.BitCast<uint, JSPropertyAttributes>(_Attributes); set => _Attributes = Methods.BitCast<JSPropertyAttributes, uint>(value); }
}
