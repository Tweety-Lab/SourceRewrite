using System.Runtime.CompilerServices;

namespace UltralightNet;

public unsafe ref struct ULVertexBuffer
{
	private byte format;
	public uint size;
	public void* data;

	public ULVertexBufferFormat Format
	{
		readonly get => Methods.BitCast<byte, ULVertexBufferFormat>(format);
		set => format = Methods.BitCast<ULVertexBufferFormat, byte>(value);
	}
}
