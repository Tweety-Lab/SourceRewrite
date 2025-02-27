using SourceRewrite.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SourceRewrite.Components.Lighting
{
    public class PointLight : GameComponent
    {
        public Vector3 Color { get; set; } = new Vector3(1, 1, 1);
        public float Intensity { get; set; } = 1.0f;

        // Update Shader Uniforms
        public override void Update(float deltaTime)
        {
            // Animate the light moving in a circle
            GameObject.Transform.Position = new Vector3(
                (float)Math.Cos(Environment.TickCount / 1000.0f) * 5, // X
                GameObject.Transform.Position.Y,                      // Y
                (float)Math.Sin(Environment.TickCount / 1000.0f) * 5  // Z
            );

            // Update Uniforms
            foreach (Shader shader in Shader.Shaders)
            {
                shader.SetParameter("light_position", GameObject.Transform.Position);
                shader.SetParameter("light_color", Color);
                shader.SetParameter("light_intensity", Intensity);
            }
        }
    }
}
