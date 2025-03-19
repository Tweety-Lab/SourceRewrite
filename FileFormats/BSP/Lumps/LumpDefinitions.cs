namespace FileFormats.BSP
{
    public static class LumpDefinitions
    {
        ////////////////////////////////////////////////////////
        // LUMP DEFINITONS
        ////////////////////////////////////////////////////////
        public static LumpDefinition[] Definitions = {

            //                 Data Type       Data          Lump Type
            new LumpDefinition(typeof(string[]), new string[] { "" }, LumpType.LUMP_SOLID_MATERIALS), // Solid Brush Materials
            new LumpDefinition(typeof(float[]), new float[] {0,0,0}, LumpType.LUMP_VERTEXES), // Vertices
            new LumpDefinition(typeof(uint[]), new uint[] {0,0,0}, LumpType.LUMP_INDICES), // Indices
            new LumpDefinition( typeof(string[]), new string[] { }, LumpType.LUMP_ENTITIES) // Entities

        };
    };

    /// <summary>
    /// Definition of a BSP Lump.
    /// </summary>
    public class LumpDefinition
    {
        public Lump Lump;
        public LumpType Type;

        public LumpDefinition(Type dataType, object data, LumpType lumpType)
        {
            Lump = new Lump(dataType, data);
            Type = lumpType;
        }

    }

    /// <summary>
    /// Lump Definition Type.
    /// </summary>
    public enum LumpType
    {
        LUMP_SOLID_MATERIALS,          // Brush Material Path ( not in < v26 )
        LUMP_VERTEXES,          // Brush Vertices
        LUMP_INDICES,           // Brush Indices ( not in < v26 )
        LUMP_ENTITIES      // Entities
                                // ... continue
    }
}
