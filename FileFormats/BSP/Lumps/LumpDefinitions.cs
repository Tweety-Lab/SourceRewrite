namespace FileFormats.BSP
{
    public static class LumpDefinitions
    {
        ////////////////////////////////////////////////////////
        // LUMP DEFINITONS
        ////////////////////////////////////////////////////////
        public static LumpDefinition[] Definitions = {

            //                 Data Type       Data          Lump Type
            new LumpDefinition(typeof(string), "metal_sheet.vmt", LumpType.LUMP_MATERIAL), // Materials
new LumpDefinition(typeof(float[]), new float[] {
    // Front face - Normal: (0, 0, -1)
    -0.5f, -0.5f, -0.5f,  0.0f, 0.0f, -1.0f,  0.0f, 0.0f,  // Bottom-left
     0.5f, -0.5f, -0.5f,  0.0f, 0.0f, -1.0f,  1.0f, 0.0f,  // Bottom-right
     0.5f,  0.5f, -0.5f,  0.0f, 0.0f, -1.0f,  1.0f, 1.0f,  // Top-right
    -0.5f,  0.5f, -0.5f,  0.0f, 0.0f, -1.0f,  0.0f, 1.0f,  // Top-left

    // Back face - Normal: (0, 0, 1)
    -0.5f, -0.5f,  0.5f,  0.0f, 0.0f, 1.0f,  0.0f, 0.0f,  // Bottom-left
     0.5f, -0.5f,  0.5f,  0.0f, 0.0f, 1.0f,  1.0f, 0.0f,  // Bottom-right
     0.5f,  0.5f,  0.5f,  0.0f, 0.0f, 1.0f,  1.0f, 1.0f,  // Top-right
    -0.5f,  0.5f,  0.5f,  0.0f, 0.0f, 1.0f,  0.0f, 1.0f,  // Top-left

    // Left face - Normal: (-1, 0, 0)
    -0.5f, -0.5f, -0.5f, -1.0f, 0.0f, 0.0f,  0.0f, 0.0f,  // Bottom-left
    -0.5f,  0.5f, -0.5f, -1.0f, 0.0f, 0.0f,  1.0f, 0.0f,  // Top-left
    -0.5f,  0.5f,  0.5f, -1.0f, 0.0f, 0.0f,  1.0f, 1.0f,  // Top-right
    -0.5f, -0.5f,  0.5f, -1.0f, 0.0f, 0.0f,  0.0f, 1.0f,  // Bottom-right

    // Right face - Normal: (1, 0, 0)
     0.5f, -0.5f, -0.5f,  1.0f, 0.0f, 0.0f,  0.0f, 0.0f,  // Bottom-left
     0.5f,  0.5f, -0.5f,  1.0f, 0.0f, 0.0f,  1.0f, 0.0f,  // Top-left
     0.5f,  0.5f,  0.5f,  1.0f, 0.0f, 0.0f,  1.0f, 1.0f,  // Top-right
     0.5f, -0.5f,  0.5f,  1.0f, 0.0f, 0.0f,  0.0f, 1.0f,  // Bottom-right

    // Top face - Normal: (0, 1, 0)
    -0.5f,  0.5f, -0.5f,  0.0f, 1.0f, 0.0f,  0.0f, 0.0f,  // Bottom-left
     0.5f,  0.5f, -0.5f,  0.0f, 1.0f, 0.0f,  1.0f, 0.0f,  // Bottom-right
     0.5f,  0.5f,  0.5f,  0.0f, 1.0f, 0.0f,  1.0f, 1.0f,  // Top-right
    -0.5f,  0.5f,  0.5f,  0.0f, 1.0f, 0.0f,  0.0f, 1.0f,  // Top-left

    // Bottom face - Normal: (0, -1, 0)
    -0.5f, -0.5f, -0.5f,  0.0f, -1.0f, 0.0f,  0.0f, 0.0f,  // Bottom-left
     0.5f, -0.5f, -0.5f,  0.0f, -1.0f, 0.0f,  1.0f, 0.0f,  // Bottom-right
     0.5f, -0.5f,  0.5f,  0.0f, -1.0f, 0.0f,  1.0f, 1.0f,  // Top-right
    -0.5f, -0.5f,  0.5f,  0.0f, -1.0f, 0.0f,  0.0f, 1.0f   // Top-left
}, LumpType.LUMP_VERTEXES), // Vertices

new LumpDefinition(typeof(uint[]), new uint[] {
    // Front face
    0, 1, 2,  0, 2, 3,
    // Back face
    4, 5, 6,  4, 6, 7,
    // Left face
    8, 9, 10,  8, 10, 11,
    // Right face
    12, 13, 14,  12, 14, 15,
    // Top face
    16, 17, 18,  16, 18, 19,
    // Bottom face
    20, 21, 22,  20, 22, 23
}, LumpType.LUMP_INDICES), // Indices
            new LumpDefinition(
    typeof(string[]),
    new string[]
    {
            """
    GameObject01 {
    position   "0,3,0"
    scale "1,2,1"
    rotation    "0,45,0"
        GameComponents {
            SourceRewrite_Components_MeshRenderer {
                MaterialPath    "metal_sheet.vmt"
                MeshPath   "cube.model"
            }
        }
    }
    """,
            """
    GameObject02 {
    position   "3,5,0"
    scale   "0.2,0.2,0.2"
    rotation    "0,0,0"
        GameComponents {
            SourceRewrite_Components_MeshRenderer {
                MaterialPath    "dev/error.vmt"
                MeshPath   "cube.model"
            }

            SourceRewrite_Components_Lighting_PointLight {

            }
        }
    }
    """
    },
    LumpType.LUMP_GAME_OBJECTS
) // Global Map Components

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
        LUMP_MATERIAL,          // Brush Material Path ( not in < v26 )
        LUMP_VERTEXES,          // Brush Vertices
        LUMP_INDICES,           // Brush Indices ( not in < v26 )
        LUMP_GAME_OBJECTS      // Game Objects and their associated Game Components ( not in < v26 )
                                  // ... continue
    }
}
