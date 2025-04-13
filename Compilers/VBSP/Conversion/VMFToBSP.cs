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
            }

            bsp.SetLumpData<BSPEntity>(BSPLumpType.LUMP_ENTITIES, BSPEntities);
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
                solidPlanes.Add(new Plane(side.plane.Point1, side.plane.Point2, side.plane.Point3));
                Sides.Add(side);
            }

            // Process each side
            foreach (VMFSide side in solid.Sides)
            {
                // Skip NODRAW Sides
                if (side.Material == "TOOLS/TOOLSNODRAW")
                    continue;

                // Calculate side geometry using CSG approach
                float[] sideVertices = CalculateSideVertices(side, solidPlanes);

                if (sideVertices.Length < 12) // Need at least 4 vertices (3 coordinates each)
                    continue;

                // Create the Side struct for BSP
                BSPPlane bspSide = new BSPPlane
                {
                    MaterialName = side.Material,
                    ID = side.ID,
                    Vertices = sideVertices,
                    Indices = GenerateIndicesForVertices(sideVertices),
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
            Plane sidePlane = new Plane(side.plane.Point1, side.plane.Point2, side.plane.Point3);

            // Create a large polygon on this plane
            List<Vector3> polygon = CreateBasePolygon(sidePlane);

            // Clip the polygon against all other planes in the solid
            foreach (Plane clipPlane in solidPlanes)
            {
                // Skip the current plane (don't clip against ourselves)
                if (clipPlane.Equals(sidePlane))
                    continue;

                polygon = ClipPolygonAgainstPlane(polygon, clipPlane);
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

        /// <summary>
        /// Creates a large initial polygon on the given plane.
        /// </summary>
        private static List<Vector3> CreateBasePolygon(Plane plane)
        {
            // Find the dominant axis of the plane normal
            Vector3 normal = plane.Normal;
            int dominantAxis = 0;
            float max = Math.Abs(normal.X);
            if (Math.Abs(normal.Y) > max)
            {
                dominantAxis = 1;
                max = Math.Abs(normal.Y);
            }
            if (Math.Abs(normal.Z) > max)
            {
                dominantAxis = 2;
            }

            // Create a large polygon in the plane
            Vector3 center = -plane.D * plane.Normal;
            Vector3 v1, v2;

            switch (dominantAxis)
            {
                case 0: // X is dominant
                    v1 = new Vector3(0, 10000, 0);
                    v2 = new Vector3(0, 0, 10000);
                    break;
                case 1: // Y is dominant
                    v1 = new Vector3(10000, 0, 0);
                    v2 = new Vector3(0, 0, 10000);
                    break;
                default: // Z is dominant
                    v1 = new Vector3(10000, 0, 0);
                    v2 = new Vector3(0, 10000, 0);
                    break;
            }

            // Make sure v1 and v2 are perpendicular to the normal
            v1 = Vector3.Cross(plane.Normal, v1);
            v2 = Vector3.Cross(plane.Normal, v1);

            return new List<Vector3>
            {
                center + v1 + v2,
                center + v1 - v2,
                center - v1 - v2,
                center - v1 + v2
            };
        }

        /// <summary>
        /// Clips a polygon against a plane using the Sutherland-Hodgman algorithm.
        /// </summary>
        private static List<Vector3> ClipPolygonAgainstPlane(List<Vector3> polygon, Plane plane)
        {
            List<Vector3> output = new List<Vector3>();
            if (polygon.Count == 0)
                return output;

            Vector3 prevVertex = polygon[polygon.Count - 1];
            float prevDistance = plane.DistanceTo(prevVertex);

            foreach (Vector3 currentVertex in polygon)
            {
                float currentDistance = plane.DistanceTo(currentVertex);

                if (currentDistance >= 0)
                {
                    if (prevDistance < 0)
                    {
                        // Intersection point
                        Vector3 intersection = plane.LineIntersection(prevVertex, currentVertex);
                        output.Add(intersection);
                    }
                    output.Add(currentVertex);
                }
                else if (prevDistance >= 0)
                {
                    // Intersection point
                    Vector3 intersection = plane.LineIntersection(prevVertex, currentVertex);
                    output.Add(intersection);
                }

                prevVertex = currentVertex;
                prevDistance = currentDistance;
            }

            return output;
        }
    }

    /// <summary>
    /// Plane equation.
    /// </summary>
    public class Plane
    {
        public Vector3 Normal;
        public float D;

        public Plane(Vector3 a, Vector3 b, Vector3 c)
        {
            Normal = Vector3.Normalize(Vector3.Cross(b - a, c - a));
            D = -Vector3.Dot(Normal, a);
        }

        public float DistanceTo(Vector3 point)
        {
            return Vector3.Dot(Normal, point) + D;
        }

        public Vector3 LineIntersection(Vector3 start, Vector3 end)
        {
            Vector3 direction = end - start;
            float denominator = Vector3.Dot(Normal, direction);

            if (Math.Abs(denominator) < float.Epsilon)
            {
                // Line is parallel to plane
                return start;
            }

            float t = -(Vector3.Dot(Normal, start) + D) / denominator;
            return start + direction * t;
        }

        public bool Equals(Plane other)
        {
            return Normal.Equals(other.Normal) && Math.Abs(D - other.D) < float.Epsilon;
        }
    }
}