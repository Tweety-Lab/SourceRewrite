using MessagePack;

namespace EngineFormats.BSP.IO
{
    public class BSPWriter
    {
        private readonly string _outputPath;
        private readonly BSPFormat _bsp;

        public BSPWriter(string outputPath, BSPFormat bsp)
        {
            _outputPath = outputPath;
            _bsp = bsp;
        }

        public void WriteToFile()
        {
            // Prepare the lumps dictionary with concrete types
            var lumps = new Dictionary<BSPLumpType, object>();

            // Convert all lumps to their specific array types
            foreach (var lump in _bsp.Header.Lumps)
            {
                if (lump.Value != null)
                {
                    lumps[lump.Key] = lump.Value;
                }
            }

            // Create a serializable header
            var header = new BSPHeader
            {
                Identifier = _bsp.Header.Identifier,
                Version = _bsp.Header.Version,
                Lumps = lumps,
                MapRevision = _bsp.Header.MapRevision
            };

            // Serialize and write to file
            byte[] serializedData = MessagePackSerializer.Serialize(header);
            File.WriteAllBytes(_outputPath, serializedData);
        }
    }
}
