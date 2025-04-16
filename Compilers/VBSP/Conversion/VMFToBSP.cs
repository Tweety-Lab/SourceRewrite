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
        public static List<VMFSide> Sides = new List<VMFSide>();

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
                BSPEntities[i].BrushSides = ConvertEntityToBSPPlanes(vmfEntity);
            }

            bsp.SetLumpData<BSPEntity>(BSPLumpType.LUMP_ENTITIES, BSPEntities);
        }

        /// <summary>
        /// Converts a VMF brush entity to its BSP plane representation.
        /// </summary>
        private static BSPPlane[] ConvertEntityToBSPPlanes(VMFEntity vmfEntity)
        {
            List<BSPPlane> planes = new List<BSPPlane>();

            // Only process entities that have brushes (solids)
            if (vmfEntity.Solids == null || vmfEntity.Solids.Count == 0)
            {
                return planes.ToArray();
            }

            // Process each solid in the entity
            foreach (VMFSolid solid in vmfEntity.Solids)
            {
                // First collect all planes from the solid
                List<Plane> solidPlanes = new List<Plane>();
                foreach (VMFSide side in solid.Sides)
                {
                    solidPlanes.Add(new Plane(side.Plane.Point1, side.Plane.Point2, side.Plane.Point3));
                }

                // Process each side in the solid
                foreach (VMFSide side in solid.Sides)
                {
                    // Calculate side geometry using CSG approach
                    float[] sideVertices = CalculateSideVertices(side, solidPlanes);

                    // Get UV Axis
                    BSPUVAxis UAxis = new BSPUVAxis()
                    {
                        Axis = side.UAxis.Vector,
                        Scale = side.UAxis.Scale
                    };

                    BSPUVAxis VAxis = new BSPUVAxis()
                    {
                        Axis = side.VAxis.Vector,
                        Scale = side.VAxis.Scale
                    };

                    // Create the Plane struct for BSP
                    BSPPlane bspPlane = new BSPPlane
                    {
                        MaterialName = side.Material,
                        ID = side.ID,
                        Vertices = sideVertices,
                        Indices = GenerateIndicesForVertices(sideVertices),
                        UAxis = UAxis,
                        VAxis = VAxis
                    };

                    planes.Add(bspPlane);
                }
            }

            return planes.ToArray();
        }

        /// <summary>
        /// Converts a VMF entity to its BSP string representation.
        /// </summary>
        private static string ConvertEntityToBspString(VMFEntity vmfEntity)
        {
            string position = $"\"{vmfEntity.Origin.X} {vmfEntity.Origin.Y} {vmfEntity.Origin.Z}\"";
            string rotation = $"\"{vmfEntity.Angles.Z} {vmfEntity.Angles.X} {vmfEntity.Angles.Y}\"";

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
                ProcessSolid(solid, bspSides);
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
            // First collect all planes from the solid
            List<Plane> solidPlanes = new List<Plane>();
            foreach (VMFSide side in solid.Sides)
            {
                solidPlanes.Add(new Plane(side.Plane.Point1, side.Plane.Point2, side.Plane.Point3));
                Sides.Add(side);
            }

            // Process each side
            foreach (VMFSide side in solid.Sides)
            {
                // Calculate side geometry using CSG approach
                float[] sideVertices = CalculateSideVertices(side, solidPlanes);

                // Get UV Axis
                BSPUVAxis UAxis = new BSPUVAxis()
                {
                    Axis = side.UAxis.Vector,
                    Scale = side.UAxis.Scale
                };

                BSPUVAxis VAxis = new BSPUVAxis()
                {
                    Axis = side.VAxis.Vector,
                    Scale = side.VAxis.Scale
                };

                // Create the Side struct for BSP
                BSPPlane bspSide = new BSPPlane
                {
                    MaterialName = side.Material,
                    ID = side.ID,
                    Vertices = sideVertices,
                    Indices = GenerateIndicesForVertices(sideVertices),

                    UAxis = UAxis,
                    VAxis = VAxis
                };

                bspSides.Add(bspSide);
            }
        }

        /// <summary>
        /// Generates triangle indices for the given vertices (assuming convex).
        /// </summary>
        private static uint[] GenerateIndicesForVertices(float[] vertices)
        {
            int vertexCount = vertices.Length / 3;
            List<uint> indices = new List<uint>();

            // Simple fan triangulation
            for (int i = 1; i < vertexCount - 1; i++)
            {
                indices.Add(0);
                indices.Add((uint)i);
                indices.Add((uint)i + 1);
            }

            return indices.ToArray();
        }

        /// <summary>
        /// Calculates the vertices for a brush side using CSG plane clipping.
        /// </summary>
        private static float[] CalculateSideVertices(VMFSide side, List<Plane> solidPlanes)
        {
            // Create the plane for this side
            Plane sidePlane = new Plane(side.Plane.Point1, side.Plane.Point2, side.Plane.Point3);

            // Create a large polygon on this plane
            List<Vector3> polygon = Plane.CreateBasePolygon(sidePlane);

            // Clip the polygon against all other planes in the solid
            foreach (Plane clipPlane in solidPlanes)
            {
                // Skip the current plane (don't clip against ourselves)
                if (clipPlane.Equals(sidePlane))
                    continue;

                polygon = Plane.ClipPolygonAgainstPlane(polygon, clipPlane);
                if (polygon.Count < 3)
                    break; // Polygon is completely clipped away
            }

            // Convert the polygon vertices to a flat float array
            if (polygon.Count < 3)
                return new float[0];

            float[] vertices = new float[polygon.Count * 3];
            for (int i = 0; i < polygon.Count; i++)
            {
                vertices[i * 3] = polygon[i].X;
                vertices[i * 3 + 1] = polygon[i].Y;
                vertices[i * 3 + 2] = polygon[i].Z;
            }

            return vertices;
        }
    }
}