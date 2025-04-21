using MessagePack;

namespace EngineFormats.BSP
{
    // Represents the structure of a BSP file
    public class BSPFormat
    {
        public BSPHeader Header;

        public T[] GetLumpData<T>(BSPLumpType type) where T : struct
        {
            if (Header.Lumps.TryGetValue(type, out var data))
            {
                return (T[])data;
            }
            return null;
        }

        public void SetLumpData<T>(BSPLumpType type, T[] data) where T : struct
        {
            // Ensure the Lumps dictionary exists
            Header.Lumps ??= new Dictionary<BSPLumpType, object>();

            // Serialize the array of structs into a byte array
            var serializedData = MessagePackSerializer.Serialize(data);

            // Set or add the lump data as a byte array
            if (Header.Lumps.ContainsKey(type))
            {
                Header.Lumps[type] = serializedData;
            }
            else
            {
                Header.Lumps.Add(type, serializedData);
            }
        }
    }
}
