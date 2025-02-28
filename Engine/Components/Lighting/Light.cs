using SourceRewrite.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using SourceRewrite.Windowing;

namespace SourceRewrite.Components
{
    public class Light : GameComponent
    {
        // Light
        public Vector3 Color { get; set; } = new Vector3(1.0f, 1.0f, 1.0f);
        public float Intensity { get; set; } = 2.0f;

        // Attenuation
        public float ConstantAttenuation { get; set; } = 1.0f;
        public float LinearAttenuation { get; set; } = 0.09f;
        public float QuadraticAttenuation { get; set; } = 0.032f;

        // Update Shader Uniforms
        public override void Update(float deltaTime)
        {
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
