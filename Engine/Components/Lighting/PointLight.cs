using SourceRewrite.Rendering;
using System.Numerics;
using SourceRewrite.Maps;

namespace SourceRewrite.Components
{
    public class PointLight : GameComponent
    {
        // Light Properties
        [MapProperty("_light")]
        public Vector4 Color = new Vector4(255.0f, 255.0f, 255.0f, 200.0f);

        // Attenuation Properties
        [MapProperty("_constant_attn")]
        public float ConstantAttenuation = 1.0f;

        [MapProperty("_linear_attn")]
        public float LinearAttenuation = 0.09f;

        [MapProperty("_quadratic_attn")]
        public float QuadraticAttenuation = 0.032f;

        // Update Shader Uniforms
        public override void Update()
        {
            // Multiply the intensity (4th component of Color) by arbitrary adjustment factor for our unit system
            Vector4 modifiedColor = Color;
            modifiedColor.W *= 90000.0f;

            // Update Uniforms
            foreach (Shader shader in Shader.Shaders)
            {
                shader.SetParameter("light_position", GameObject.Transform.Position);
                shader.SetParameter("light_color", modifiedColor / 255.0f); // Convert Color from 1-255 range to 0-1 range
                shader.SetParameter("light_attenuation", new Vector3(ConstantAttenuation, LinearAttenuation, QuadraticAttenuation));
            }
        }

        public override void OnDestroy()
        {
            // Update Uniforms
            foreach (Shader shader in Shader.Shaders)
            {
                shader.SetParameter("light_color", new Vector4(0.0f, 0.0f, 0.0f, 0.0f));
            }
        }
    }
}
