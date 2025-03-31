using SourceRewrite.Attributes;
using SourceRewrite.Editor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SourceRewrite.Entities.Point
{
    [Entity("point_servercommand")]
    public class PointServerCommand : BaseEntity
    {
        [Input]
        public void Command(string command)
        {
            DeveloperConsole.EvaluateCommand(command);
        }

#if EDITOR
        public override void DrawGizmos()
        {
            Gizmos.Color = new Vector4(205, 20, 205, 1);
            Gizmos.DrawCube(Transform.Position, new Vector3(24f, 24f, 24f), this);
        }

        public override void DrawGizmosSelected()
        {
            base.DrawGizmosSelected();
        }
#endif
    }
}
