using SourceRewrite.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SourceRewrite.Entities
{
    [Entity("trigger_multiple")]
    public class TriggerMultiple : BaseTrigger
    {
        public override void Start()
        {
            base.Start();

            PhysicsBody.OnCollisionStart += (entity) => { FireOutput("OnStartTouch"); };
            PhysicsBody.OnCollisionEnd += (entity) => { FireOutput("OnEndTouch"); };
        }
    }
}
