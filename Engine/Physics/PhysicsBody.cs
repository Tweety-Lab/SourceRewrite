using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SourceRewrite.Physics
{
    public class PhysicsBody

    {
        public float Mass { get; set; } = 1.0f;
        public float Friction { get; set; } = 0.5f;
        public float Restitution { get; set; } = 0.5f;
        public bool IsStatic { get; set; } = false;

        public Vector3 BoundingBox { get; set; } = new Vector3(0f, 0f, 0f);

    }
}
