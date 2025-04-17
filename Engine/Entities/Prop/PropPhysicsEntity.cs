using SourceRewrite.Attributes;
using SourceRewrite.InputSystem;
using SourceRewrite.PhysicsSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
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

            PhysicsBody.CollisionMesh = Mesh;
            PhysicsBody.Mass = 1f;
            PhysicsInitNormal();
        }

        public override void Update()
        {
            base.Update();
        }
    }
}
