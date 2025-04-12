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
            outputBsp.Header.MapRevision = vmf.VersionInfo.MapVersion;

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

            BSPEntity[] BSPEntities = new BSPEntity[vmf.Entities.Count];

            for (int i = 0; i < vmf.Entities.Count; i++)
            {
                VMFEntity vmfEntity = vmf.Entities[i];
                BSPEntities[i].KeyValuesString = ConvertEntityToBspString(vmfEntity);
            }

            bsp.SetLumpData<BSPEntity>(BSPLumpType.LUMP_ENTITIES, BSPEntities);
        }

        /// <summary>
        /// Converts a VMF entity to its BSP string representation.
        /// </summary>
        private static string ConvertEntityToBspString(VMFEntity vmfEntity)
        {
            string position = $"\"{vmfEntity.Origin.X} {vmfEntity.Origin.Y} {vmfEntity.Origin.Z}\"";
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

            List<BSPPlane> bspSides = new List<BSPPlane>();

            uint indexOffset = 0;

            foreach (VMFSolid solid in vmf.World.Solids)
            {
                Console.WriteLine($"Compiling solid with id of {solid.ID}...");
                ProcessSolid(solid, bspSides);  // Updated to pass bspSides
            }

            // Store compiled geometry in BSP lumps
            bsp.SetLumpData<BSPPlane>(BSPLumpType.LUMP_PLANES, bspSides.ToArray());

            Console.WriteLine($"Compiled {vmf.World.Solids.Count} solids.");
        }

        /// <summary>
        /// Processes a single solid and adds its geometry to the vertex and index lumps.
        /// </summary>
        private static void ProcessSolid(VMFSolid solid, List<BSPPlane> bspSides)
        {
            foreach (VMFSide side in solid.Sides)
            {
                // Skip NODRAW Sides
                if (side.Material == "TOOLS/TOOLSNODRAW")
                    continue;

                // Calculate side geometry
                float[] sideVertices = CalculateSideVertices(side);

                // Create the Side struct for BSP
                BSPPlane bspSide = new BSPPlane
                {
                    MaterialName = side.Material,  // Assuming the side has a material property
                    ID = side.ID,
                    Vertices = sideVertices,  // Store vertices for the side
                    Indices = new uint[] { 0, 1, 2, 0, 2, 3 },
                };

                bspSides.Add(bspSide);
            }
        }

        /// <summary>
        /// Calculates the vertices for a brush side.
        /// </summary>
        private static float[] CalculateSideVertices(VMFSide side)
        {
            Vector3 p1 = side.plane.Point1;
            Vector3 p2 = side.plane.Point2;
            Vector3 p3 = side.plane.Point3;

            // Calculate fourth point to form a planar quad
            Vector3 p4 = p3 + (p1 - p2);

            // Populate Vertices
            return new float[]
            {
    p4.X, p4.Y, p4.Z,
    p3.X, p3.Y, p3.Z,
    p2.X, p2.Y, p2.Z,
    p1.X, p1.Y, p1.Z
            };
        }
    }
}