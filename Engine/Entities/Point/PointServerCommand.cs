using SourceRewrite.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
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
    }
}
