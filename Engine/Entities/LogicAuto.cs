using SourceRewrite.Attributes;
using SourceRewrite.Editor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SourceRewrite.Entities
{
    [Entity("logic_auto")]
    public class LogicAuto : BaseEntity
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

        public override void DrawGizmosSelected()
        {
            base.DrawGizmosSelected();
        }
#endif
    }
}
