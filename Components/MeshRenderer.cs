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
            //X    Y      Z     U   V
             0.5f,  0.5f, 0.0f, 1f, 0f,
             0.5f, -0.5f, 0.0f, 1f, 1f,
            -0.5f, -0.5f, 0.0f, 0f, 1f,
            -0.5f,  0.5f, 0.0f, 0f, 0f
        };

        // Placeholder Array of indices
        public uint[] Indices =
        {
            0, 1, 3,
            1, 2, 3
        };
    }
}
