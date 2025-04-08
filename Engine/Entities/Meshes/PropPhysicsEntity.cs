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
            base.Start();

            PhysicsBody = new Physics.PhysicsBody();
            PhysicsBody.CollisionMesh = Mesh;
            PhysicsBody.Mass = 100f;
            PhysicsInitNormal();
        }

        public override void Update()
        {
            base.Update();
        }
    }
}
