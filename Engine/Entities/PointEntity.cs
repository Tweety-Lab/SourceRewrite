using SourceRewrite.Physics;
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

        // Entity Rigid Body
        public PhysicsBody PhysicsBody { get; set; } = null;

        // Name Constructor
        public PointEntity(string name) : base(name) { }

        // Nameless Constructor
        public PointEntity() : base() { }

        public void PhysicsInitNormal()
        {
            // Initialize the physics body if it doesn't exist
            if (PhysicsBody == null)
                PhysicsBody = new PhysicsBody();

            // Register this entity with the physics context
            GameWindow.CurrentWindow.Modules.GetModule<PhysicsModule>().Context.InitPhysicsEntity(this);
        }

        public void SetAbsVelocity(Vector3 velocity)
        {
            GameWindow.CurrentWindow.Modules.GetModule<PhysicsModule>().Context.SetEntityAbsVelocity(this, velocity);
        }
    }
}
