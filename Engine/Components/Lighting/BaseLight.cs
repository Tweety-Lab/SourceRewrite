using SourceRewrite.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using SourceRewrite.Windowing;

namespace SourceRewrite.Components.Lighting
{
    public class BaseLight : GameComponent
    {
        // Light
        public Vector3 Color { get; set; } = new Vector3(1.0f, 1.0f, 1.0f);
        public float Intensity { get; set; } = 2.0f;

        // Attenuation
        public float ConstantAttenuation { get; set; } = 1.0f;
        public float LinearAttenuation { get; set; } = 0.09f;
        public float QuadraticAttenuation { get; set; } = 0.032f;

        // Animation
        private int circleRadius = 7;
        private int verticalAmplitude = 3;
        private int verticalSpeed = 3;
        private float animationTime = 0.0f; // Time accumulator for smooth animation

        // Update Shader Uniforms
        public override void Update(float deltaTime)
        {
            // Accumulate time passed to ensure smooth animation
            animationTime += deltaTime;

            // Animate the light moving in a sphere (smooth animation)
            GameObject.Transform.Position = new Vector3(
                (float)Math.Cos(animationTime) * circleRadius,  // X
                (float)Math.Sin(animationTime * verticalSpeed) * verticalAmplitude, // Y Vertical oscillation
                (float)Math.Sin(animationTime) * circleRadius   // Z
            );

            // Update Uniforms
            foreach (Shader shader in Shader.Shaders)
            {
                shader.SetParameter("light_position", GameObject.Transform.Position);
                shader.SetParameter("light_color", Color * Intensity);
                shader.SetParameter("light_attenuation", new Vector3(ConstantAttenuation, LinearAttenuation, QuadraticAttenuation));
            }
        }
    }
}
