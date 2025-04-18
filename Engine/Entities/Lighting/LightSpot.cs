using SourceRewrite.AssetTypes;
using SourceRewrite.Attributes;
using System.Numerics;

namespace SourceRewrite.Entities.Lighting
{
    [AlwaysExecute]
    [Entity("light_spot")]
    public class LightSpot : PointEntity, ILight
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

        public void ApplyToShader(Shader shader, int lightIndex)
        {
            string lightPrefix = $"lights[{lightIndex}]";

            Vector4 modifiedColor = Color;
            modifiedColor.W *= 90000.0f;
            Vector4 normalizedColor = modifiedColor / 255.0f;

            shader.SetParameter($"{lightPrefix}.position", Transform.Position);
            shader.SetParameter($"{lightPrefix}.color", normalizedColor);
            shader.SetParameter($"{lightPrefix}.attenuation",
                new Vector3(ConstantAttenuation, LinearAttenuation, QuadraticAttenuation));
            shader.SetParameter($"{lightPrefix}.lightType", 1); // 1 = spot light
            shader.SetParameter($"{lightPrefix}.direction", Transform.Forward);
            shader.SetParameter($"{lightPrefix}.cutOff", MathF.Cos(EngineMaths.DegreesToRadians(InnerConeAngle)));
            shader.SetParameter($"{lightPrefix}.outerCutOff", MathF.Cos(EngineMaths.DegreesToRadians(OuterConeAngle)));
        }

#if EDITOR
        public override void DrawGizmos()
        {
            Gizmos.Color = new Vector4(255, 255, 255, 1);
            Gizmos.DrawSprite(Transform.Position, "sprites/spot_light", 2f, this);
        }
#endif
    }
}
