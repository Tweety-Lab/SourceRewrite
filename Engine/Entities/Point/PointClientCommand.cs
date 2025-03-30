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
    [Entity("point_clientcommand")]
    public class PointClientCommand : BaseEntity
    {
        [Input]
        public void Command(string command)
        {
            DeveloperConsole.EvaluateCommand(command);
        }

#if EDITOR
        public override void DrawGizmos()
        {
            Gizmos.Color = new Vector4(205, 205, 205, 1);
            Gizmos.DrawCube(Transform.Position, new Vector3(128f, 128f, 128f), this);
        }
#endif
    }
}
