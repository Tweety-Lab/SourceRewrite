using SourceRewrite.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SourceRewrite.Components.Lighting
{
    public class BaseLight : GameComponent
    {
        // Light
        public Vector3 Color { get; set; } = new Vector3(1f, 1f, 1);
        public float Intensity { get; set; } = 1.0f;

        // Attenuation
        public float ConstantAttenuation { get; set; } = 1.0f;
        public float LinearAttenuation { get; set; } = 0.09f;
        public float QuadraticAttenuation { get; set; } = 0.032f;

        // Animation
        private int circleRadius = 10;

        // Update Shader Uniforms
        public override void Update(float deltaTime)
        {
            // Animate the light moving in a sphere
            GameObject.Transform.Position = new Vector3(
                (float)Math.Cos(Environment.TickCount / 700.0f) * circleRadius, // X
                (float)Math.Cos(Environment.TickCount / 700.0f) * circleRadius, // Y
                (float)Math.Sin(Environment.TickCount / 700.0f) * circleRadius  // Z
            );

            // Update Uniforms
            foreach (Shader shader in Shader.Shaders)
            {
                shader.SetParameter("light_position", GameObject.Transform.Position);
                shader.SetParameter("light_color", Color);
                shader.SetParameter("light_intensity", Intensity);
                shader.SetParameter("light_attenuation", new Vector3(ConstantAttenuation, LinearAttenuation, QuadraticAttenuation));
            }
        }
    }
}
