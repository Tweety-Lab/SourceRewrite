using SourceRewrite.Attributes;
using SourceRewrite.Editor;
using System.Numerics;

namespace SourceRewrite.Entities.Lighting
{
    [AlwaysExecute]
    [Entity("light_directional")]
    public class LightDirectional : BaseEntity
    {
        // Light Properties
        [EntityProperty("_light")]
        public Vector4 Color { get; set; } = new Vector4(255.0f, 255.0f, 255.0f, 200.0f);

#if EDITOR
        public override void DrawGizmos()
        {
            Gizmos.Color = new Vector4(255, 255, 255, 1);
            Gizmos.DrawSprite(Transform.Position, "sprites/directional_light", 2f, this);
        }
#endif
    }
}
