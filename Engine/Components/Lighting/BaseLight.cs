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
        public Vector3 Color { get; set; } = new Vector3(1f, 1f, 1);
        public float Intensity { get; set; } = 1.0f;

        private int circleRadius = 7;

        // Update Shader Uniforms
        public override void Update(float deltaTime)
        {
            // Animate the light moving in a circle
            GameObject.Transform.Position = new Vector3(
                (float)Math.Cos(Environment.TickCount / 700.0f) * circleRadius, // X
                (float)Math.Cos(Environment.TickCount / 700.0f) * circleRadius, // Y
                (float)Math.Sin(Environment.TickCount / 700.0f) * circleRadius  // Z
            );

            // Animate the light moving in a sphere

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
