using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace VBSP.Conversion
{
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
    

        /// <summary>
        /// Creates a large initial polygon on the given plane.
        /// </summary>
        public static List<Vector3> CreateBasePolygon(Plane plane)
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
        public static List<Vector3> ClipPolygonAgainstPlane(List<Vector3> polygon, Plane plane)
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
}
