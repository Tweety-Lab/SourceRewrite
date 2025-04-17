using SourceRewrite.AssetTypes;
using SourceRewrite.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SourceRewrite.PhysicsSystem
{
    public class PhysicsBody
    {
        // Physics
        public float Mass { get; set; } = 1.0f;
        public float Friction { get; set; } = 0.5f;
        public float Restitution { get; set; } = 0.5f;

        // Behaviour
        public BodyType BodyType { get; set; } = BodyType.Dynamic;
        public bool CanCollide { get; set; } = true;

        // Rotation Constraints
        public bool FreezeRotationX { get; set; } = false;
        public bool FreezeRotationY { get; set; } = false;
        public bool FreezeRotationZ { get; set; } = false;

        public bool FreezeAllRotations
        {
            set
            {
                FreezeRotationX = value;
                FreezeRotationY = value;
                FreezeRotationZ = value;
            }
        }

        // Collision
        public Mesh CollisionMesh { get; set; } = null;
        public Vector3 BoundingBox { get; set; } = new Vector3(0f, 0f, 0f);

        // Events
        public Action<BaseEntity>? OnCollisionStart { get; set; } = new Action<BaseEntity>(entity => { });
        public Action<BaseEntity>? OnCollisionEnd { get; set; } = new Action<BaseEntity>(entity => { });
        public Action<BaseEntity>? OnColliding { get; set; } = new Action<BaseEntity>(entity => { });

    }

    public enum BodyType
    {
        Static,
        Dynamic,
        Kinematic
    }
}
