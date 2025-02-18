using System;
using System.Collections.Generic;
using System.Linq;
using SourceRewrite.AssetTypes;
using SourceRewrite.Windowing;

namespace SourceRewrite.Components
{
    /// <summary>
    /// Takes a "Mesh" Asset Type and renders it in 3D space.
    /// </summary>
    public class MeshRenderer : GameComponent
    {
        public dynamic Mesh { get; set; }
        public MeshRenderer(object meshRenderer)
        {
            Mesh = meshRenderer;

            if (Mesh == null)
            {
                Console.WriteLine("Could not initialize MeshRenderer, Mesh is Null!");
            } else
            {
                GameWindow.CurrentWindow.Renderer.InitMesh(Mesh); // Render the mesh
            }
        }
    }
}
