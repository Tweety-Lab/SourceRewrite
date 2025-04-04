using SourceRewrite.Attributes;
using System.Numerics;

namespace SourceRewrite.Entities.Lighting
{
    [AlwaysExecute]
    [Entity("light_spot")]
    public class LightSpot : BaseEntity
    {
        // Spot Light Properties
        [EntityProperty("_cone")]
        public int OuterConeAngle { get; set; } = 45;

        [EntityProperty("_inner_cone")]
        public int InnerConeAngle { get; set; } = 30;

        // Light Properties
        [EntityProperty("_light")]
        public Vector4 Color { get; set; } = new Vector4(255.0f, 255.0f, 255.0f, 200.0f);

        // Attenuation Properties
        [EntityProperty("_constant_attn")]
        public float ConstantAttenuation { get; set; } = 1.0f;

        [EntityProperty("_linear_attn")]
        public float LinearAttenuation { get; set; } = 0.09f;

        [EntityProperty("_quadratic_attn")]
        public float QuadraticAttenuation { get; set; } = 0.032f;

#if EDITOR
        public override void DrawGizmos()
        {
            Gizmos.Color = new Vector4(255, 255, 255, 1);
            Gizmos.DrawSprite(Transform.Position, "sprites/spot_light", 2f, this);
        }
#endif
    }
}
