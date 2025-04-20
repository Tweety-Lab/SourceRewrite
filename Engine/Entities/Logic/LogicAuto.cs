using SourceRewrite.Attributes;
using System.Numerics;

namespace SourceRewrite.Entities
{
    [Entity("logic_auto")]
    public class LogicAuto : PointEntity
    {
        public override void Start()
        {
            FireOutput("OnMapSpawn");
        }

#if EDITOR
        public override void DrawGizmos()
        {
            Gizmos.Color = new Vector4(255, 255, 255, 0);
            Gizmos.DrawSprite(Transform.Position, "editor/logic_auto", 1f, this);
        }
#endif
    }
}
