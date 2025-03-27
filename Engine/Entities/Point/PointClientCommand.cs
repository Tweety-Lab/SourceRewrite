using SourceRewrite.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
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
    }
}
