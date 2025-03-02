using FileFormats.BSP;
using FileFormats.KeyValues;
using FileFormats.VMF;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using System.Xml.Linq;

namespace VBSP.Conversion
{
    public static class VMFToBSP
    {
        /// <summary>
        /// Returns a BSPFormat object compiled from a VMF file.
        /// </summary>
        public static BSPFormat CompileVMF(string contents)
        {
            // Load the VMF file
            VMFFormat VMF = new VMFFormat(contents);

            BSPFormat outputBSP = new BSPFormat();

            // Set the BSP header to the VMF MapVersion
            outputBSP.Header.mapRevision = VMF.VersionInfo.MapVersion;

            // Handle entities only if they exist
            if (VMF.Entities != null && VMF.Entities.Count > 0)
            {
                string[] entities = new string[VMF.Entities.Count];

                // Convert entities to GameObject strings
                int i = 0;
                foreach (Entity vmfEntity in VMF.Entities)
                {
                    // Get their properties
                    string entityPropertiesString = string.Empty;
                    foreach (KeyValue kvProperty in vmfEntity.Properties)
                    {
                        entityPropertiesString += $" {kvProperty.Key} \"{kvProperty.Value}\"\n";
                    }

                    string entityString = @$" GameObject{i} {{
                        position ""{-vmfEntity.Origin.Y} {vmfEntity.Origin.Z} {-vmfEntity.Origin.X}""
                        rotation ""0 0 0""
                        scale ""1 1 1""
                        GameComponents {{
                            {ClassConversion.ClassMap[$"{vmfEntity.ClassName}"]} {{
                                {entityPropertiesString}
                            }}
                        }}
                    }}";

                    entities[i] = entityString;

                    i++;
                }

                outputBSP.SetLumpData(LumpType.LUMP_GAME_OBJECTS, entities);
            }

            // Handle solids only if they exist
            if (VMF.World.Solids != null && VMF.World.Solids.Count > 0)
            {
                List<float> vertices = new List<float>(); // Store vertex positions
                List<uint> indices = new List<uint>(); // Store triangle indices
                List<string> materials = new List<string>(); // Store material names

                uint indexOffset = 0; // Tracks the index of the next vertex

                // Add vertices (Position, Normal, UV)
                void AddVertex(Vector3 pos, Vector3 norm, Vector2 uv)
                {
                    // Normalize the normal vector
                    norm = Vector3.Normalize(norm);

                    vertices.AddRange(new float[]
                    {
                    pos.X, pos.Y, pos.Z,     // Position (3 floats)
                    norm.X, norm.Y, norm.Z,  // Normal (3 floats)
                    uv.X, uv.Y               // UV (2 floats)
                    });
                }

                // Loop through all solids
                foreach (Solid solid in VMF.World.Solids)
                {
                    Console.WriteLine($"Compiling solid with id of {solid.ID}...");

                    // Loop through all solid sides
                    foreach (Side side in solid.Sides)
                    {
                        // Add the material to the list of materials
                        materials.Add(side.Material);

                        // Get corners and convert them to Y up coordinate system
                        Vector3 corner1 = new Vector3(-side.Plane.Corner1.Y, side.Plane.Corner1.Z, -side.Plane.Corner1.X);
                        Vector3 corner2 = new Vector3(-side.Plane.Corner2.Y, side.Plane.Corner2.Z, -side.Plane.Corner2.X);
                        Vector3 corner3 = new Vector3(-side.Plane.Corner3.Y, side.Plane.Corner3.Z, -side.Plane.Corner3.X);

                        // Calculate the fourth corner
                        Vector3 corner4 = corner1 + (corner3 - corner2);

                        // Compute the face normal
                        Vector3 normal = Vector3.Normalize(Vector3.Cross(corner2 - corner1, corner3 - corner1));

                        // Assuming the side contains UVData
                        UVAxis uvData = side.UAxis;

                        // The UV scale is directly fetched from UVData.UVScale
                        float textureScale = 1f / (uvData.UVScale * 500f); // Invert textureScale to make lower values result in more tiling

                        // Calculate UVs based on world-space position
                        Vector2 uv1 = new Vector2(corner1.X * textureScale, corner1.Z * textureScale);
                        Vector2 uv2 = new Vector2(corner2.X * textureScale, corner2.Z * textureScale);
                        Vector2 uv3 = new Vector2(corner3.X * textureScale, corner3.Z * textureScale);
                        Vector2 uv4 = new Vector2(corner4.X * textureScale, corner4.Z * textureScale);


                        // Populate the vertices
                        AddVertex(corner1, normal, uv1);
                        AddVertex(corner2, normal, uv2);
                        AddVertex(corner3, normal, uv3);
                        AddVertex(corner4, normal, uv4);

                        // Add indices for two triangles (forming a quad)
                        indices.Add(indexOffset);
                        indices.Add(indexOffset + 1);
                        indices.Add(indexOffset + 2);

                        indices.Add(indexOffset);
                        indices.Add(indexOffset + 2);
                        indices.Add(indexOffset + 3);

                        indexOffset += 4; // Move to the next set of indices
                    }
                }

                // Store vertices and indices in the BSP lumps
                outputBSP.SetLumpData(LumpType.LUMP_VERTEXES, vertices.ToArray());
                outputBSP.SetLumpData(LumpType.LUMP_INDICES, indices.ToArray());

                // Store side materials in the BSP lump
                outputBSP.SetLumpData(LumpType.LUMP_SOLID_MATERIALS, materials.ToArray());

                Console.WriteLine($"Compiled {VMF.World.Solids.Count} solids.");
            }

            outputBSP.SetLumpData(LumpType.LUMP_ENTITIES, new string[] { "prop_static", "light" });

            return outputBSP;
        }
    }
}
