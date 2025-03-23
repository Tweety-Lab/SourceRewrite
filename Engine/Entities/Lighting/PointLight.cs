using SourceRewrite.Attributes;
using SourceRewrite.Maps;
using SourceRewrite.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

#if EDITOR
using SourceRewrite.Editor;
#endif

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

        // LIGHT RENDERING IS APPLIED IN LIGHTING RENDER PASS!

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
            Gizmos.Color = new Vector4(255, 0, 0, 1);
            MeshEntity sphereGizmo = Gizmos.DrawSphere(Transform.Position, 32, 16);

            sphereGizmo.Parent = this;
        }
#endif
    }
}
