using SourceRewrite.AssetTypes;
using SourceRewrite.Attributes;
using System.Numerics;

namespace SourceRewrite.Entities.Lighting
{
    [AlwaysExecute]
    [Entity("light_directional")]
    public class LightDirectional : PointEntity, ILight
    {
        // Light Properties
        [EntityProperty("_light")]
        public Vector4 Color { get; set; } = new Vector4(255.0f, 255.0f, 255.0f, 200.0f);

        public void ApplyToShader(Shader shader, int lightIndex)
        {
            string lightPrefix = $"lights[{lightIndex}]";

            Vector4 normalizedColor = Color / 255.0f;

            shader.SetParameter($"{lightPrefix}.color", normalizedColor);
            shader.SetParameter($"{lightPrefix}.lightType", 2); // 2 = directional light
            shader.SetParameter($"{lightPrefix}.direction", Transform.Forward);

            // Set unused parameters
            shader.SetParameter($"{lightPrefix}.position", Vector3.Zero);
            shader.SetParameter($"{lightPrefix}.attenuation", Vector3.One);
            shader.SetParameter($"{lightPrefix}.cutOff", 0.0f);
            shader.SetParameter($"{lightPrefix}.outerCutOff", 0.0f);
        }

#if EDITOR
        public override void DrawGizmos()
        {
            Gizmos.Color = new Vector4(255, 255, 255, 1);
            Gizmos.DrawSprite(Transform.Position, "sprites/directional_light", 2f, this);
        }
#endif
    }
}
