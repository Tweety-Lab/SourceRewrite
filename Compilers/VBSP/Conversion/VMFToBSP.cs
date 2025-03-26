using FileFormats.BSP;
using FileFormats.KeyValues;
using FileFormats.VMF;
using System.Numerics;

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

                // Convert entities to Entity strings
                int i = 0;
                foreach (Entity vmfEntity in VMF.Entities)
                {
                    // Get their properties
                    string entityPropertiesString = string.Empty;
                    foreach (KeyValue kvProperty in vmfEntity.Properties)
                    {
                        entityPropertiesString += $" {kvProperty.Key} \"{kvProperty.Value}\"\n";
                    }

                    foreach (KeyValue kvConnection in vmfEntity.Connections)
                    {
                        entityPropertiesString += $" @{kvConnection.Key} \"{kvConnection.Value}\"\n";
                    }

                    string entityString = @$"{vmfEntity.TargetName} {{
position ""{-vmfEntity.Origin.Y} {vmfEntity.Origin.Z} {-vmfEntity.Origin.X}""
rotation ""{vmfEntity.Angles.X} {vmfEntity.Angles.Y} {vmfEntity.Angles.Z}""
{entityPropertiesString}
                    }}";

                    entities[i] = entityString;

                    i++;
                }

                outputBSP.SetLumpData(LumpType.LUMP_ENTITIES, entities);
            }

            // Handle solids only if they exist
            if (VMF.World.Solids != null && VMF.World.Solids.Count > 0)
            {
                List<float> vertices = new List<float>(); // Store vertex positions
                List<uint> indices = new List<uint>(); // Store triangle indices
                List<string> materials = new List<string>(); // Store material names

                uint indexOffset = 0; // Tracks the index of the next vertex

                // Loop through all solids
                foreach (Solid solid in VMF.World.Solids)
                {
                    Console.WriteLine($"Compiling solid with id of {solid.ID}...");

                    // Loop through all solid sides
                    foreach (Side side in solid.Sides)
                    {
                        // Add the material to the list of materials
                        materials.Add(side.Material);

                        // Find intersections with other geometry
                        List<Vector3> intersectionPoints = CalculateSide(side);

                        // Add vertex positions
                        for (int i = 0; i < intersectionPoints.Count; i++)
                        {
                            vertices.Add(intersectionPoints[i].X);
                            vertices.Add(intersectionPoints[i].Y);
                            vertices.Add(intersectionPoints[i].Z);
                        }

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

            return outputBSP;
        }

        // Returns a list of intersection points between a plane and other geometry
        private static List<Vector3> CalculateSide(Side side)
        {
            List<Vector3> output = new List<Vector3>();

            // Create a Plane for brush processing

            // Get corners and convert them to Y up coordinate system
            Vector3 corner1 = new Vector3(-side.plane.Point1.Y, side.plane.Point1.Z, -side.plane.Point1.X);
            Vector3 corner2 = new Vector3(-side.plane.Point2.Y, side.plane.Point2.Z, -side.plane.Point2.X);
            Vector3 corner3 = new Vector3(-side.plane.Point3.Y, side.plane.Point3.Z, -side.plane.Point3.X);

            // Calculate the fourth corner
            Vector3 corner4 = corner1 + (corner3 - corner2);

            // Add the corners to the output list
            output.Add(corner1);
            output.Add(corner2);
            output.Add(corner3);
            output.Add(corner4);

            return output;
        }
    }
}
