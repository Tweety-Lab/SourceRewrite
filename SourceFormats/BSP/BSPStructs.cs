using MessagePack;

namespace EngineFormats.BSP
{
    [MessagePackObject]
    public struct BSPHeader
    {
        [Key(0)] public string Identifier;
        [Key(1)] public int Version;
        [Key(2)] public Dictionary<BSPLumpType, object> Lumps;
        [Key(3)] public int MapRevision;
    }


    [MessagePackObject]
    public struct BSPLump<T> where T : struct
    {
        [Key(0)] public BSPLumpType Type;
        [Key(1)] public T[] Data;
    }
}
