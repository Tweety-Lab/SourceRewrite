using System;
using System.Collections.Generic;
using System.Linq;
using SourceRewrite.Assets;
using SourceRewrite.Windowing;

namespace SourceRewrite.Components
{
    public class MeshRenderer : GameComponent
    {
        public dynamic Mesh { get; set; }
        public MeshRenderer(object meshRenderer)
        {
            Mesh = meshRenderer;
            GameWindow.CurrentWindow.Renderer.InitMesh(Mesh); // Render the mesh
        }
    }
}
