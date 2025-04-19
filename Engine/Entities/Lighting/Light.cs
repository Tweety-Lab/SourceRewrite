using SourceRewrite.Attributes;
using System.Numerics;
using SourceRewrite.AssetTypes;


#if EDITOR
#endif

namespace SourceRewrite.Entities
{
    [AlwaysExecute]
    [Entity("light")]
    public class Light : PointEntity, ILight
    {
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
            modifiedColor.W *= 4000.0f;
            Vector4 normalizedColor = modifiedColor / 255.0f;

            shader.SetParameter($"{lightPrefix}.position", Transform.Position);
            shader.SetParameter($"{lightPrefix}.color", normalizedColor);
            shader.SetParameter($"{lightPrefix}.attenuation",
                new Vector3(ConstantAttenuation, LinearAttenuation, QuadraticAttenuation));
            shader.SetParameter($"{lightPrefix}.lightType", 0); // 0 = point light
            shader.SetParameter($"{lightPrefix}.direction", Vector3.Zero);
            shader.SetParameter($"{lightPrefix}.cutOff", 0.0f);
            shader.SetParameter($"{lightPrefix}.outerCutOff", 0.0f);
        }

#if EDITOR
        public override void DrawGizmos()
        {
            Gizmos.Color = new Vector4(255, 255, 255, 1);
            Gizmos.DrawSprite(Transform.Position, "sprites/point_light", 2f, this);
        }
#endif
    }

    public interface ILight
    {
        // Handle applying the light to the shader
        void ApplyToShader(Shader shader, int lightIndex);
    }
}
