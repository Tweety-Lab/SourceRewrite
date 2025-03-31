using SourceRewrite.Attributes;
using System.Numerics;
using SourceRewrite.AssetTypes;


#if EDITOR
using SourceRewrite.Editor;
#endif

namespace SourceRewrite.Entities
{
    [AlwaysExecute]
    [Entity("light")]
    public class Light : BaseEntity
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
            Gizmos.Color = new Vector4(255, 255, 255, 1);
            Gizmos.DrawSprite(Transform.Position, "sprites/point_light", 2f, this);
            Gizmos.DrawWireframeCube(Transform.Position, new Vector3(32f, 32f, 32f), this);
        }
#endif
    }
}
