using SourceRewrite.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SourceRewrite.Entities.Logic
{
    [Entity("logic_relay")]
    public class LogicRelay : PointEntity
    {
        [Input]
        public void Trigger()
        {
            // Fire the trigger output
            FireOutput("OnTrigger");
        }
    }
}
