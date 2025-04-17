using SourceRewrite.Attributes;
using SourceRewrite.PhysicsSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SourceRewrite.Entities.Meshes
{
    [Entity("prop_static")]
    public class PropStaticEntity : PropEntity
    {
        public override void Start()
        {
            base.Start();

            // Create Static Physics
            PhysicsBody.BodyType = BodyType.Static;
            PhysicsBody.CollisionMesh = Mesh;

            PhysicsInitNormal();
        }
    }
}
