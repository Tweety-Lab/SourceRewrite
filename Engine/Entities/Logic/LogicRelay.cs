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
        public bool Enabled { get; set; } = true;

        [Input]
        public void Trigger()
        {
            if (!Enabled) return;

            // Fire the trigger output
            FireOutput("OnTrigger");
        }

        [Input]
        public void Disable()
        {
            Enabled = false;
        }

        [Input]
        public void Enable()
        {
            Enabled = true;
        }
    }
}
