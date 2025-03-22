using SourceRewrite.Attributes;
using SourceRewrite.Editor;
using SourceRewrite.Maps;
using SourceRewrite.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SourceRewrite.Entities
{
    [AlwaysExecute]
    public class PointLight : BaseEntity
    {
        // Light Properties
        [EntityProperty("_light")]
        public Vector4 Color = new Vector4(255.0f, 255.0f, 255.0f, 200.0f);

        // Attenuation Properties
        [EntityProperty("_constant_attn")]
        public float ConstantAttenuation = 1.0f;

        [EntityProperty("_linear_attn")]
        public float LinearAttenuation = 0.09f;

        [EntityProperty("_quadratic_attn")]
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
                shader.SetParameter("light_position", Transform.Position);
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

#if EDITOR
        public override void DrawGizmos()
        {
            Gizmos.Color = new Vector4(255, 255, 255, 1);
            Gizmos.DrawCube(Transform.Position, new Vector3(32.0f, 32.0f, 32.0f));
        }
#endif
    }
}
