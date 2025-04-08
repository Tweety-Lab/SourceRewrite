using SourceRewrite.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SourceRewrite.Entities.Meshes
{
    [Entity("prop_physics")]
    public class PropPhysicsEntity : PropEntity
    {
        public override void Start()
        {
            PhysicsBody = new Physics.PhysicsBody();
            PhysicsBody.BoundingBox = new System.Numerics.Vector3(128, 64, 128);
            PhysicsInitNormal();
            base.Start();
        }

        public override void Update()
        {
            base.Update();
        }
    }
}
