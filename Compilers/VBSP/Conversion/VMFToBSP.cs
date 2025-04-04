using FileFormats.BSP;
using FileFormats.KeyValues;
using FileFormats.VMF;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace VBSP.Conversion
{
    /// <summary>
    /// Convert from Valve Map Format (VMF) to our BSP format.
    /// </summary>
    public static class VMFToBSP
    {
        /// <summary>
        /// Compiles a VMF file content into a BSP format object.
        /// </summary>
        /// <param name="contents">The VMF file contents as a string</param>
        /// <returns>A compiled BSPFormat object</returns>
        public static BSPFormat CompileVMF(string contents)
        {
            // Parse the VMF file
            VMFFormat vmf = new VMFFormat(contents);
            BSPFormat outputBsp = new BSPFormat();

            // Set the BSP header version from VMF
            outputBsp.Header.mapRevision = vmf.VersionInfo.MapVersion;

            // Process entities
            ProcessEntities(vmf, outputBsp);

            // Process world geometry
            ProcessWorldGeometry(vmf, outputBsp);

            return outputBsp;
        }

        /// <summary>
        /// Processes VMF entities and stores them in the BSP format.
        /// </summary>
        private static void ProcessEntities(VMFFormat vmf, BSPFormat bsp)
        {
            if (vmf.Entities == null || vmf.Entities.Count == 0)
            {
                return;
            }

            string[] entities = new string[vmf.Entities.Count];

            for (int i = 0; i < vmf.Entities.Count; i++)
            {
                Entity vmfEntity = vmf.Entities[i];
                entities[i] = ConvertEntityToBspString(vmfEntity);
            }

            bsp.SetLumpData(LumpType.LUMP_ENTITIES, entities);
        }

        /// <summary>
        /// Converts a VMF entity to its BSP string representation.
        /// </summary>
        private static string ConvertEntityToBspString(Entity vmfEntity)
        {
            // Convert coordinate system: Source uses Y-forward, Z-up, but BSP uses different convention
            string position = $"\"{-vmfEntity.Origin.Y} {vmfEntity.Origin.Z} {-vmfEntity.Origin.X}\"";
            string rotation = $"\"{vmfEntity.Angles.X} {vmfEntity.Angles.Y} {vmfEntity.Angles.Z}\"";

            // Build properties string using StringBuilder
            StringBuilder propertiesBuilder = new StringBuilder();

            // Add regular properties
            foreach (KeyValue kvProperty in vmfEntity.Properties)
            {
                // Skip origin and angles since we handle them specially
                if (kvProperty.Key != "origin" && kvProperty.Key != "angles")
                {
                    propertiesBuilder.AppendLine($" {kvProperty.Key} \"{kvProperty.Value}\"");
                }
            }

            // Add connections with connection_ prefix
            foreach (KeyValue kvConnection in vmfEntity.Connections)
            {
                propertiesBuilder.AppendLine($" connection_{kvConnection.Key} \"{kvConnection.Value}\"");
            }

            // Format the complete entity string
            StringBuilder entityBuilder = new StringBuilder();
            entityBuilder.AppendLine($"{vmfEntity.TargetName} {{");
            entityBuilder.AppendLine($"position {position}");
            entityBuilder.AppendLine($"rotation {rotation}");
            entityBuilder.Append(propertiesBuilder);
            entityBuilder.Append("}");

            return entityBuilder.ToString();
        }

        /// <summary>
        /// Processes world geometry from VMF and stores it in the BSP format.
        /// </summary>
        private static void ProcessWorldGeometry(VMFFormat vmf, BSPFormat bsp)
        {
            if (vmf.World.Solids == null || vmf.World.Solids.Count == 0)
            {
                return;
            }

            List<float> vertices = new List<float>();
            List<uint> indices = new List<uint>();
            List<string> materials = new List<string>();

            uint indexOffset = 0;

            foreach (Solid solid in vmf.World.Solids)
            {
                Console.WriteLine($"Compiling solid with id of {solid.ID}...");
                ProcessSolid(solid, vertices, indices, materials, ref indexOffset);
            }

            // Store compiled geometry in BSP lumps
            bsp.SetLumpData(LumpType.LUMP_VERTEXES, vertices.ToArray());
            bsp.SetLumpData(LumpType.LUMP_INDICES, indices.ToArray());
            bsp.SetLumpData(LumpType.LUMP_SOLID_MATERIALS, materials.ToArray());

            Console.WriteLine($"Compiled {vmf.World.Solids.Count} solids.");
        }

        /// <summary>
        /// Processes a single solid and adds its geometry to the vertex and index lumps.
        /// </summary>
        private static void ProcessSolid(Solid solid, List<float> vertices, List<uint> indices,
                                        List<string> materials, ref uint indexOffset)
        {
            foreach (Side side in solid.Sides)
            {
                // Add material
                materials.Add(side.Material);

                // Calculate side geometry
                List<Vector3> sideVertices = CalculateSideVertices(side);

                // Add vertex positions to the buffer
                foreach (Vector3 vertex in sideVertices)
                {
                    vertices.Add(vertex.X);
                    vertices.Add(vertex.Y);
                    vertices.Add(vertex.Z);
                }

                // Create triangles from the quad (two triangles)
                indices.Add(indexOffset);
                indices.Add(indexOffset + 1);
                indices.Add(indexOffset + 2);

                indices.Add(indexOffset);
                indices.Add(indexOffset + 2);
                indices.Add(indexOffset + 3);

                // Move to the next set of vertices
                indexOffset += 4;
            }
        }

        /// <summary>
        /// Calculates the vertices for a brush side, converting from VMF to BSP coordinate system.
        /// </summary>
        private static List<Vector3> CalculateSideVertices(Side side)
        {
            // Convert from VMF to BSP coordinate system (Y-up)
            Vector3 corner1 = ConvertToYUpCoordSystem(side.plane.Point1);
            Vector3 corner2 = ConvertToYUpCoordSystem(side.plane.Point2);
            Vector3 corner3 = ConvertToYUpCoordSystem(side.plane.Point3);

            // Calculate the fourth corner to complete the quad
            Vector3 corner4 = corner1 + (corner3 - corner2);

            return new List<Vector3> { corner1, corner2, corner3, corner4 };
        }

        /// <summary>
        /// Converts a point from VMF coordinate system to BSP's Y-up coordinate system.
        /// </summary>
        private static Vector3 ConvertToYUpCoordSystem(Vector3 point)
        {
            return new Vector3(-point.Y, point.Z, -point.X);
        }
    }
}