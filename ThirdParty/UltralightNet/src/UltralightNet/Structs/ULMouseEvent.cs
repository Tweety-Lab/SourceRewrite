using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace UltralightNet;

public static unsafe partial class Methods
{
	[DllImport(LibUltralight)]
	public static extern ULMouseEvent* ulCreateMouseEvent(ULMouseEventType type, int x, int y, ULMouseEventButton button);

	[DllImport(LibUltralight)]
	public static extern void ulDestroyMouseEvent(ULMouseEvent* evt);
}

/// <summary>
/// Mouse Event
/// </summary>
public struct ULMouseEvent : IEquatable<ULMouseEvent>
{
	private int _Type;
	public ULMouseEventType Type { readonly get => Methods.BitCast<int, ULMouseEventType>(_Type); set => _Type = Methods.BitCast<ULMouseEventType, int>(value); }
	public int X;
	public int Y;
	private int _Button;
	public ULMouseEventButton Button { readonly get => Methods.BitCast<int, ULMouseEventButton>(_Button); set => _Button = Methods.BitCast<ULMouseEventButton, int>(value); }

	public readonly bool Equals(ULMouseEvent other) => Type == other.Type && X == other.X && Y == other.Y && Button == other.Button;
}
