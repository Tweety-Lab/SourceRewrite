using SourceRewrite.Attributes;
using SourceRewrite.Editor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SourceRewrite.Entities.Lighting
{
    [AlwaysExecute]
    [Entity("light_spot")]
    public class LightSpot : BaseEntity
    {
        // Spot Light Properties
        [EntityProperty("_cone")]
        public int OuterConeAngle = 45;

        [EntityProperty("_inner_cone")]
        public int InnerConeAngle = 30;

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

#if EDITOR
        public override void DrawGizmos()
        {
            Gizmos.Color = new Vector4(255, 255, 255, 1);
            Gizmos.DrawSprite(Transform.Position, "sprites/spot_light", 2f, this);
        }

        public override void DrawGizmosSelected()
        {
            base.DrawGizmosSelected();
        }
#endif
    }
}
