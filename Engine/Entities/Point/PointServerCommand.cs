using SourceRewrite.Attributes;
using System.Numerics;

namespace SourceRewrite.Entities.Point
{
    [Entity("point_servercommand")]
    public class PointServerCommand : PointEntity
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
#endif
    }
}
