using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SourceRewrite.Objects;
using SourceRewrite.Rendering;
using SourceRewrite.Windowing;

namespace SourceRewrite.Components
{
    public class MeshRenderer : GameComponent
    {
        public Texture texture;
        public Shader shader;
        public MeshRenderer(Texture inputTexture, Shader inputShader)
        {
            shader = inputShader;
            texture = inputTexture;

            GameWindow.CurrentWindow.Renderer.InitMesh(this); // Render our mesh
        }
        // Placeholder Array of vertex positions
        public float[] Vertices =
        {
            // Position          // Texture Coordinates
            // Front face
            0.5f,  0.5f,  0.5f, 1f, 0f, // Top-right-front
            0.5f, -0.5f,  0.5f, 1f, 1f, // Bottom-right-front
            -0.5f, -0.5f,  0.5f, 0f, 1f, // Bottom-left-front
            -0.5f,  0.5f,  0.5f, 0f, 0f, // Top-left-front

            // Back face
            0.5f,  0.5f, -0.5f, 1f, 0f, // Top-right-back
            0.5f, -0.5f, -0.5f, 1f, 1f, // Bottom-right-back
            -0.5f, -0.5f, -0.5f, 0f, 1f, // Bottom-left-back
            -0.5f,  0.5f, -0.5f, 0f, 0f, // Top-left-back

            // Left face
            -0.5f,  0.5f,  0.5f, 1f, 0f, // Top-left-front
            -0.5f, -0.5f,  0.5f, 1f, 1f, // Bottom-left-front
            -0.5f, -0.5f, -0.5f, 0f, 1f, // Bottom-left-back
            -0.5f,  0.5f, -0.5f, 0f, 0f, // Top-left-back

            // Right face
            0.5f,  0.5f,  0.5f, 1f, 0f, // Top-right-front
            0.5f, -0.5f,  0.5f, 1f, 1f, // Bottom-right-front
            0.5f, -0.5f, -0.5f, 0f, 1f, // Bottom-right-back
            0.5f,  0.5f, -0.5f, 0f, 0f, // Top-right-back

            // Top face
            0.5f,  0.5f,  0.5f, 1f, 0f, // Top-right-front
            -0.5f,  0.5f,  0.5f, 0f, 0f, // Top-left-front
            -0.5f,  0.5f, -0.5f, 0f, 1f, // Top-left-back
            0.5f,  0.5f, -0.5f, 1f, 1f, // Top-right-back

            // Bottom face
            0.5f, -0.5f,  0.5f, 1f, 0f, // Bottom-right-front
            -0.5f, -0.5f,  0.5f, 0f, 0f, // Bottom-left-front
            -0.5f, -0.5f, -0.5f, 0f, 1f, // Bottom-left-back
            0.5f, -0.5f, -0.5f, 1f, 1f, // Bottom-right-back
        };

        // Placeholder Array of indices
        public uint[] Indices =
        {
            // Front face
            0, 1, 3,
            1, 2, 3,

            // Back face
            4, 5, 7,
            5, 6, 7,

            // Left face
            8, 9, 11,
            9, 10, 11,

            // Right face
            12, 13, 15,
            13, 14, 15,

            // Top face
            16, 17, 19,
            17, 18, 19,

            // Bottom face
            20, 21, 23,
            21, 22, 23
        };
    }
}
