using SourceRewrite.Files;
using System.Numerics;

namespace SourceRewrite.AssetTypes
{
    /// <summary>
    /// Base Mesh, contains Vertices, Indices, Normals, UVs and a Material.
    /// </summary>
    public class Mesh
    {
        // Array of Vertex positions (x, y, z)
        public float[] Vertices = { };

        // Array of Vertex normals (nx, ny, nz)
        public float[] Normals = { };

        // Array of Texture coordinates (u, v)
        public float[] UVs = { };

        // Array of Indices
        public uint[] Indices = { };

        // Material
        public Material Material = FileSystem.GetMaterial("dev/error");

        /// <summary>
        /// Calculates vertex normals based on the face normals of the triangles.
        /// Each vertex normal is the average of all face normals of triangles sharing the vertex.
        /// </summary>
        public void CalculateNormals()
        {
            // Initialize normals array if not already created
            if (Normals == null || Normals.Length != Vertices.Length)
            {
                Normals = new float[Vertices.Length];
            }
            else
            {
                // Reset existing normals to zero
                Array.Clear(Normals, 0, Normals.Length);
            }

            // Temporary dictionary to track accumulated normals for each vertex
            Dictionary<int, Vector3> vertexNormals = new Dictionary<int, Vector3>();

            // For each triangle in the mesh
            for (int i = 0; i < Indices.Length; i += 3)
            {
                // Get triangle vertex indices
                int indexA = (int)Indices[i];
                int indexB = (int)Indices[i + 1];
                int indexC = (int)Indices[i + 2];

                // Get vertex positions
                Vector3 vertexA = new Vector3(
                    Vertices[indexA * 3],
                    Vertices[indexA * 3 + 1],
                    Vertices[indexA * 3 + 2]
                );

                Vector3 vertexB = new Vector3(
                    Vertices[indexB * 3],
                    Vertices[indexB * 3 + 1],
                    Vertices[indexB * 3 + 2]
                );

                Vector3 vertexC = new Vector3(
                    Vertices[indexC * 3],
                    Vertices[indexC * 3 + 1],
                    Vertices[indexC * 3 + 2]
                );

                // Calculate face normal using cross product
                Vector3 edge1 = vertexB - vertexA;
                Vector3 edge2 = vertexC - vertexA;
                Vector3 faceNormal = Vector3.Normalize(Vector3.Cross(edge1, edge2));

                // Add the face normal to all three vertices
                AddNormalToVertex(vertexNormals, indexA, faceNormal);
                AddNormalToVertex(vertexNormals, indexB, faceNormal);
                AddNormalToVertex(vertexNormals, indexC, faceNormal);
            }

            // Normalize accumulated normals and store in the Normals array
            foreach (var kvp in vertexNormals)
            {
                int vertexIndex = kvp.Key;
                Vector3 normalizedVector = Vector3.Normalize(kvp.Value);

                // Store the normalized normal in the Normals array
                Normals[vertexIndex * 3] = normalizedVector.X;
                Normals[vertexIndex * 3 + 1] = normalizedVector.Y;
                Normals[vertexIndex * 3 + 2] = normalizedVector.Z;
            }
        }

        /// <summary>
        /// Helper method to add a normal vector to the specified vertex in the dictionary.
        /// </summary>
        private void AddNormalToVertex(Dictionary<int, Vector3> normalsDictionary, int vertexIndex, Vector3 normal)
        {
            if (normalsDictionary.ContainsKey(vertexIndex))
            {
                normalsDictionary[vertexIndex] += normal;
            }
            else
            {
                normalsDictionary[vertexIndex] = normal;
            }
        }

        /// <summary>
        /// Calculates the normals for flat shading (each triangle has consistent normals).
        /// This is useful for hard-edged objects or for debugging.
        /// </summary>
        public void CalculateFlatNormals()
        {
            // For flat shading, we need separate vertices for each triangle face
            // This means we need to expand our vertices and create new normals
            float[] expandedVertices = new float[Indices.Length * 3];
            float[] expandedNormals = new float[Indices.Length * 3];
            float[] expandedUVs = new float[Indices.Length * 2];
            uint[] expandedIndices = new uint[Indices.Length];

            for (int i = 0; i < Indices.Length; i += 3)
            {
                // Get triangle vertex indices
                int indexA = (int)Indices[i];
                int indexB = (int)Indices[i + 1];
                int indexC = (int)Indices[i + 2];

                // Get vertex positions
                Vector3 vertexA = new Vector3(
                    Vertices[indexA * 3],
                    Vertices[indexA * 3 + 1],
                    Vertices[indexA * 3 + 2]
                );

                Vector3 vertexB = new Vector3(
                    Vertices[indexB * 3],
                    Vertices[indexB * 3 + 1],
                    Vertices[indexB * 3 + 2]
                );

                Vector3 vertexC = new Vector3(
                    Vertices[indexC * 3],
                    Vertices[indexC * 3 + 1],
                    Vertices[indexC * 3 + 2]
                );

                // Calculate face normal using cross product
                Vector3 edge1 = vertexB - vertexA;
                Vector3 edge2 = vertexC - vertexA;
                Vector3 faceNormal = Vector3.Normalize(Vector3.Cross(edge1, edge2));

                // For each vertex in the triangle, copy position and assign the same normal
                for (int j = 0; j < 3; j++)
                {
                    int originalIndex = (int)Indices[i + j];
                    int newVertexIndex = i + j;

                    // Copy position
                    expandedVertices[newVertexIndex * 3] = Vertices[originalIndex * 3];
                    expandedVertices[newVertexIndex * 3 + 1] = Vertices[originalIndex * 3 + 1];
                    expandedVertices[newVertexIndex * 3 + 2] = Vertices[originalIndex * 3 + 2];

                    // Assign the same normal for all three vertices
                    expandedNormals[newVertexIndex * 3] = faceNormal.X;
                    expandedNormals[newVertexIndex * 3 + 1] = faceNormal.Y;
                    expandedNormals[newVertexIndex * 3 + 2] = faceNormal.Z;

                    // Copy UV if available
                    if (UVs != null && UVs.Length > 0)
                    {
                        expandedUVs[newVertexIndex * 2] = UVs[originalIndex * 2];
                        expandedUVs[newVertexIndex * 2 + 1] = UVs[originalIndex * 2 + 1];
                    }

                    // Set the new index
                    expandedIndices[i + j] = (uint)(i + j);
                }
            }

            // Replace the mesh data with the expanded data
            Vertices = expandedVertices;
            Normals = expandedNormals;
            UVs = expandedUVs;
            Indices = expandedIndices;
        }

        /// <summary>
        /// Inverts all the normals of the mesh.
        /// </summary>
        public void InvertNormals()
        {
            if (Normals != null)
            {
                for (int i = 0; i < Normals.Length; i++)
                {
                    Normals[i] = -Normals[i];
                }
            }
        }
    }
}
