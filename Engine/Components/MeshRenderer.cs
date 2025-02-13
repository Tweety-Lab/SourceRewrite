using System;
using System.Collections.Generic;
using System.Linq;
using SourceRewrite.Assets;
using SourceRewrite.Windowing;

namespace SourceRewrite.Components
{
    public class MeshRenderer : GameComponent
    {
        public Mesh Mesh { get; set; }
        public MeshRenderer(string filePath, Material material)
        {
            Mesh = new Mesh(filePath, material);
            GameWindow.CurrentWindow.Renderer.InitMesh(Mesh); // Render the mesh
        }
    }
}
