using System.Runtime.CompilerServices;

namespace UltralightNet;

public struct ULRenderBuffer
{
	public uint TextureId;
	public uint Width;
	public uint Height;

	private byte _HasStencilBuffer;
	public bool HasStencilBuffer
	{
		readonly get => Methods.BitCast<byte, bool>(_HasStencilBuffer);
		set => _HasStencilBuffer = Methods.BitCast<bool, byte>(value);
	}
	private byte _HasDepthBuffer;
	public bool HasDepthBuffer
	{
		readonly get => Methods.BitCast<byte, bool>(_HasDepthBuffer);
		set => _HasDepthBuffer = Methods.BitCast<bool, byte>(value);
	}
}
