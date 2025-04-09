using SourceRewrite.PhysicsSystem;
using SourceRewrite.Windowing;
using SourceRewrite.Windowing.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SourceRewrite.Entities
{
    public class PointEntity : BaseEntity
    {
        // Entity Transform
        public Transform Transform { get; set; } = new Transform();

        // Name Constructor
        public PointEntity(string name) : base(name) { }

        // Nameless Constructor
        public PointEntity() : base() { }

        public void PhysicsInitNormal(PhysicsBody body)
        {
            // Register this entity with the physics context
            Physics.InitPhysicsEntity(this, body);
        }

        public void SetAbsVelocity(Vector3 velocity)
        {
            Physics.SetEntityAbsVelocity(this, velocity);
        }
    }
}
